using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	private const float nudgeForce = 1.5f;
    private ArrayList touchInstances;
    Vector2 maxXZ = new Vector2(Screen.width * 1f, Screen.height * 1f);
	private bool noTouches = false;


    public Shader silhouetteShader;
	private Shader standardShader;

    public static bool objectDrag = false; //Static variable mainly used to tell camera that now movement will be used on objects

    private class TouchInstance{
        private ArrayList fingerIds; //if multiple fingers are touching the same object, we count as one instance
        private int lastFingerId; //if multiple fingers are touching the same object, only the last one will be used for stacking
        private Transform transform; //the transform of the gameobject touched
        private Vector3 position; //the initial position of the gameobject
        private Vector2 touchPosition; //the initial touch position of the gameobject when touched
        private Transform closest; //the closest tranform to the touched gameobject
		private Quaternion originalRotation; //the initial rotation of the gameobject when touched
		private float originalHeight; //the initial height of the gameobject when touched

		public TouchInstance(int initialId, Transform transform, Transform closest, Vector3 initialPosition, Vector2 touchPosition) {
            fingerIds = new ArrayList();
            fingerIds.Add(initialId);
            this.transform = transform;
            this.position = initialPosition;
            this.closest = closest;
            this.touchPosition = touchPosition;
			this.originalRotation = transform.rotation;
			this.originalHeight = transform.position.y;
        }

        public void AddFingerId(int fingerId) {
            fingerIds.Add(fingerId);
            lastFingerId = fingerId;
        }

        public void RemoveFingerId(int fingerId) {
            fingerIds.Remove(fingerId);
        }

        public int GetLastId() {
            return lastFingerId;
        }

        public Transform GetTransform() {
            return transform;
        }

        public Transform GetClosest() {
            return closest;
        }

		public Quaternion GetRotation(){
			return originalRotation;
		}

		public float GetHeight(){
			return originalHeight;
		}

        public bool ContainsId(int fingerId) {
            return fingerIds.Contains(fingerId);
        }

        public Vector3 GetPosition() {
            return position;
        }

        public Vector2 GetTouchPosition() {
            return touchPosition;
        }

        public void SetTouchPosition(Vector2 touchPosition) {
            this.touchPosition = touchPosition;
        }	

        public void SetPosition(Vector3 position) {
            this.position = position;
        }

        public int TouchCount() {
            return fingerIds.Count;
        }


		//return the distance to the topmost object stacked on top of the closest gameobject
        public Vector2 GetDistance() {
			Vector3 distance = (closest.gameObject.GetComponent<PlayerController>().TopTransform().position - transform.position);
            return new Vector2(distance.x, distance.z);
        }
    }

	// Use this for initialization
	void Start () {
        touchInstances = new ArrayList();

		standardShader = Shader.Find ("Standard");
		PlayerController.silhouette = silhouetteShader;
		PlayerController.standard = standardShader;
	}
	
	// Update is called once per frame
	void Update () {
        foreach (Touch touch in Input.touches) {     
            switch (touch.phase) {
                case TouchPhase.Began:
                    TouchBegan(touch);
                    break;
				case TouchPhase.Stationary:
                case TouchPhase.Moved:
                    TouchMoved(touch);
                    break;

                case TouchPhase.Ended:
                    TouchEnded(touch);
                    break;
            }
        }

		VelocityListener ();
	}

	void VelocityListener ()
	{
		if (noTouches) {
			bool allVelocityZero = true;
			foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag ("3DCube")) {
				if (gameObject.GetComponent<Rigidbody> ().velocity.magnitude != 0) {
					allVelocityZero = false;
				}
			}
			if (allVelocityZero) {
				//If the velocity for all objects are zero, record the state and stop the listener
				StateController.Push ();
				noTouches = false;
			}
		}
	}

    private void TouchEnded(Touch touch) {
        ArrayList newList = new ArrayList();

        //Remove the ended touches
        foreach (TouchInstance instance in touchInstances) {
			if(instance.GetTransform() != null){
				instance.GetTransform().GetComponent<Rigidbody>().mass = 10;
				instance.GetTransform().GetComponent<Rigidbody>().freezeRotation = false;
			}
            if (!instance.ContainsId(touch.fingerId)) {
                newList.Add(instance);
            }
        }

        touchInstances = newList;
		if (touchInstances.Count == 0 && objectDrag) { //If no more touches, start the listener for all object's velocity
			noTouches = true;
		}
        objectDrag = false;
    }

    private void TouchMoved(Touch touch) {
        Ray rayMove = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hitMove;
		
        foreach (TouchInstance instance in touchInstances) {
            if (instance.ContainsId(touch.fingerId)) {
                objectDrag = true;
                Transform tf = instance.GetTransform();
				if(tf == null){
					continue;
				}
                Rigidbody rb = tf.GetComponent<Rigidbody>();

				rb.mass = 10;
				rb.freezeRotation = true;
                Vector3 previousPos = tf.position;
                Vector3 currentPos = new Vector3();

                if (instance.TouchCount() == 1) {
					IgnoreLayer();
					if(touch.deltaPosition.magnitude >= 20){ //nudges
						if (Physics.Raycast(rayMove, out hitMove)) {

							currentPos = hitMove.point;
							currentPos.y = instance.GetHeight(); //change Y to be the current height so when objects are stacked bottom ones won't be touched        	
						}
						
						Vector3 force = currentPos - previousPos;
						if(force.magnitude > 1){
							force = force.normalized * (touch.deltaPosition.magnitude / 30);
						}
						rb.velocity = force;
						

					}
					else{	//Free Moving 2D
						if (Physics.Raycast(rayMove, out hitMove)) {
							Vector3 slope = rayMove.direction * -1;
							currentPos = hitMove.point;
							currentPos.x += slope.x * (instance.GetHeight() / slope.y);
							currentPos.z += slope.z * (instance.GetHeight() / slope.y);
							
							currentPos.y = instance.GetHeight(); //change Y to be the current height so when objects are stacked bottom ones won't be touched        	
						}

						if(tf.gameObject.GetComponent<PlayerController>().touchingOtherObject){
							Vector3 force = currentPos - previousPos;
							force *= 15f;
							if(force.magnitude > 10){
								force = force.normalized * 10;
							}
							rb.velocity = force;
						}
						else{
							tf.position = currentPos;
						}


					}
					ResetLayer();
					instance.SetTouchPosition(touch.position);
					
					
				}
				else { //Clipped to the nearest object               
					if (touch.fingerId == instance.GetLastId()) {  
                        Vector2 forceXZ = instance.GetDistance();
                        float ratio = forceXZ.magnitude / maxXZ.x;
                        Vector2 currentTouchPosition = instance.GetTouchPosition();
                        Vector2 previousTouchPosition = touch.position;
                        Vector2 deltaTouchPosition = currentTouchPosition - previousTouchPosition;

						if(Camera.main.WorldToScreenPoint(rb.position).x < Camera.main.WorldToScreenPoint(instance.GetClosest().position).x){
                            deltaTouchPosition.x *= -1;
						}
                        
                        Vector3 force = new Vector3(deltaTouchPosition.x * forceXZ.x, deltaTouchPosition.y * -2, deltaTouchPosition.x * forceXZ.y);

                        if (force.magnitude > 10) {
                            force = force.normalized * 10;
                        }

                        rb.velocity = force;
                        instance.SetTouchPosition(touch.position);
                    }
                }
                
                
            }
        }
		
    }

    private void TouchBegan(Touch touch) {
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;

		if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Aura" && hit.transform.gameObject.GetComponent<AuraController>().Parent.tag == "3DCube") {
            objectDrag = true;
            Transform tf = hit.transform.gameObject.GetComponent<AuraController>().Parent.transform;
            bool instanceExists = false;
            foreach(TouchInstance instance in touchInstances){
                if (instance.GetTransform() == tf) {
                    instanceExists = true;
                    instance.AddFingerId(touch.fingerId);
                }
            }

            if (!instanceExists) {
				Transform closest = FindClosestCube(tf);

				//handle rotation
				TapRotate(touch, tf, closest);

				//if not taping, add to arraylist
                touchInstances.Add(new TouchInstance(touch.fingerId, tf, closest, tf.position, touch.position));
            }
        }
    }

	private void TapRotate(Touch touch, Transform tf, Transform closest){
		if(touch.tapCount == 2){
			if(tf.rotation == closest.rotation){
				//tf.RotateAround(tf.position, Vector3.up, 90);
				if(tf.name.Contains("Cone")){
					tf.rotation = Quaternion.Euler (0, 23.17876f, 0);
				}
				else if(tf.name.Contains("TriangleScalene")){
					tf.rotation = Quaternion.Euler (-0.00003051758f, -121.3617f, 120.9408f);
				}
				else{
					tf.rotation = Quaternion.identity;
				}
			}
			else{
				tf.rotation = closest.rotation;
			}
		}
	}

    Transform FindClosestCube(Transform tf) {
        GameObject[] gameObjectGroup = GameObject.FindGameObjectsWithTag("3DCube");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = tf.position;
        foreach (GameObject go in gameObjectGroup) {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance) {
				if (go.GetComponent<Transform>().position != tf.position && go.name != "3D Cubes") {
                    closest = go;
                    distance = curDistance;
                } 
            }
        }

		//if no closeest object if found, the closest is itself
        return (gameObjectGroup.Length <= 2) ? tf : closest.GetComponent<Transform>();
    }

	//When moving the objects, change all objects to another layer so raycast hits the plane
    private void IgnoreLayer() {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("3DCube");
        foreach (GameObject cube in cubes) {
            cube.layer = 2;
        }

		GameObject[] auras = GameObject.FindGameObjectsWithTag("Aura");
		foreach (GameObject cube in auras) {
			cube.layer = 2;
		}
    }

	//Resets the object layers so touch can register with objects
    private void ResetLayer() {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("3DCube");
        foreach (GameObject cube in cubes) {
			cube.layer = LayerMask.NameToLayer( "ObjectLayer" );
        }

		GameObject[] auras = GameObject.FindGameObjectsWithTag("Aura");
		foreach (GameObject cube in auras) {
			cube.layer = LayerMask.NameToLayer( "AuraLayer" );
			
		}
    }
}
