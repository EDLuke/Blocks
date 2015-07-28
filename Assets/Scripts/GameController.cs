﻿using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	private const float nudgeForce = 1.5f;
    private ArrayList touchInstances;
    Vector2 maxXZ = new Vector2(Screen.width * 1f, Screen.height * 1f);

    public Shader silhouette;

    public static bool objectDrag = false;

    private class TouchInstance{
        private ArrayList fingerIds;
        private int lastFingerId;
        private Transform transform;
        private Vector3 position;
        private Vector2 touchPosition;
        private Transform closest;
		private Quaternion originalRotation;
		private float originalHeight;

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

        public Vector2 GetDistance() {
            Vector3 distance = (closest.position - transform.position);
            return new Vector2(distance.x, distance.z);
        }
    }

	// Use this for initialization
	void Start () {
        touchInstances = new ArrayList();
		NewPlayerController.silhouette = silhouette;
	}
	
	// Update is called once per frame
	void Update () {
        foreach (Touch touch in Input.touches) {     
            switch (touch.phase) {
                case TouchPhase.Began:
                    TouchBegan(touch);
                    break;

                case TouchPhase.Moved:
                    TouchMoved(touch);
                    break;

                case TouchPhase.Ended:
                    TouchEnded(touch);
                    break;
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
        objectDrag = false;
    }

    private void TouchMoved(Touch touch) {
        Ray rayMove = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hitMove;
		
        foreach (TouchInstance instance in touchInstances) {
            if (instance.ContainsId(touch.fingerId)) {
				print(touch.deltaPosition.magnitude);
                objectDrag = true;
                Transform tf = instance.GetTransform();
                Rigidbody rb = tf.GetComponent<Rigidbody>();

				rb.mass = 10;
				rb.freezeRotation = true;
                Vector3 previousPos = tf.position;
                Vector3 currentPos = new Vector3();

                if (instance.TouchCount() == 1) { 
					if(touch.deltaPosition.magnitude >= 5){ //nudges
						/*Vector3 force = new Vector3();
						force.y = tf.position.y;
						if(Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y)){
							force.x = nudgeForce;
							force.z = -nudgeForce;
						}
						else{
							force.x = -nudgeForce;
							force.z = nudgeForce;
						}
						rb.velocity = force;*/
						IgnoreLayer();
						
						if (Physics.Raycast(rayMove, out hitMove)) {
							currentPos = hitMove.point;
							currentPos.y = instance.GetHeight(); //change Y to be the current height so when objects are stacked bottom ones won't be touched        	
						}
						
						Vector3 force = currentPos - previousPos;
						if(force.magnitude > 1){
							force = force.normalized * (touch.deltaPosition.magnitude / 15);
						}
						rb.velocity = force;
						
						ResetLayer();
					}
					else if(touch.deltaPosition.magnitude >= 1){	//Free Moving 2D
						IgnoreLayer();
						
						if (Physics.Raycast(rayMove, out hitMove)) {
							currentPos = hitMove.point;
							currentPos.y = instance.GetHeight(); //change Y to be the current height so when objects are stacked bottom ones won't be touched        	
						}
						
						Vector3 force = currentPos - previousPos;
						force *= 15f;
						if(force.magnitude > 10){
                            force = force.normalized * 10;
						}
						rb.velocity = force;
						
						ResetLayer();
					}
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
		//planeClone.SetActive (true);
		//planeClone.GetComponent<Collider> ().enabled = false;
		if (Physics.Raycast(ray, out hit) && hit.transform.tag != "Plane" && hit.transform.tag != "Background") {
            objectDrag = true;
			print (hit.transform.gameObject.name);
            Transform tf = hit.transform.gameObject.GetComponent<AuraController>().Parent.transform;
			//Transform tf = hit.transform;
            //Renderer shade = tf.GetComponent<Renderer>();
            //shade.material.shader = silhouette;
            bool contains = false;
            foreach(TouchInstance instance in touchInstances){
                if (instance.GetTransform() == tf) {
                    contains = true;
                    instance.AddFingerId(touch.fingerId);
                }
            }

            if (!contains) {
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
				tf.RotateAround(tf.position, Vector3.up, 90);
			}
			else{
				tf.rotation = closest.rotation;
			}
		}
		else if(touch.tapCount == 3){
			if(tf.rotation == closest.rotation){
				tf.RotateAround(tf.position, Vector3.right, 90);
			}
			else{
				tf.rotation = closest.rotation;
			}	
		}
	}

    public void Reset() {
        Application.LoadLevel(0);
        touchInstances = new ArrayList();
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

		//print (closest.name);
        return (gameObjectGroup.Length <= 2) ? tf : closest.GetComponent<Transform>();
    }

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
