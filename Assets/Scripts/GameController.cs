using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    private ArrayList touchInstances;
    Vector2 maxXZ = new Vector2(Screen.width * 1f, Screen.height * 1f);

    public Shader silhouette;
    private Shader standard;

    public static bool objectDrag = false;

    private class TouchInstance{
        private ArrayList fingerIds;
        private int lastFingerId;
        private Transform transform;
        private Vector3 position;
        private Vector2 touchPosition;
        private Transform closest;

        public TouchInstance(int initialId, Transform transform, Transform closest, Vector3 initialPosition, Vector2 touchPosition) {
            fingerIds = new ArrayList();
            fingerIds.Add(initialId);
            this.transform = transform;
            this.position = initialPosition;
            this.closest = closest;
            this.touchPosition = touchPosition;
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

        public void RecalculateTouchPosition(Vector2 touchPosition) {
            if (TouchCount() > 1) {
                this.touchPosition = (touchPosition - this.touchPosition) * 0.5f + this.touchPosition;
            }
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

        standard = Shader.Find("Standard");
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
                objectDrag = true;
                Transform tf = instance.GetTransform();
                Rigidbody rb = tf.GetComponent<Rigidbody>();
                Vector3 previousPos = tf.position;
                Vector3 currentPos = new Vector3();

                if (instance.TouchCount() == 1) { //Free Moving 2D
                    IgnoreLayer();
                    if (Physics.Raycast(rayMove, out hitMove)) {
                        currentPos = hitMove.point;
                        //print(hitMove.point);
                        currentPos.y = 1;
                    }
                    Vector3 force = currentPos - previousPos;

                    force *= 15f;

                    rb.velocity = force;

                    ResetLayer();
                    instance.SetTouchPosition(touch.position);

                }
                else { //Clipped to the nearest object
                    //print(instance.GetLastId() + " : " + touch.fingerId);
                    
                    if (touch.fingerId == instance.GetLastId()) {
                        
                        Vector2 forceXZ = instance.GetDistance();
                        print(forceXZ);
                        float ratio = forceXZ.magnitude / maxXZ.x;
                        float maxTouchHeight = maxXZ.y * ratio;
                        float maxTouchWidth = forceXZ.magnitude;
                        Vector2 currentTouchPosition = instance.GetTouchPosition();
                        Vector2 previousTouchPosition = touch.position;
                        Vector2 deltaTouchPosition = currentTouchPosition - previousTouchPosition;

                        


                        if (forceXZ.x > 0) {
                            deltaTouchPosition.x *= -1;
                        }
                        

                        Vector3 force = new Vector3(deltaTouchPosition.x * forceXZ.x, deltaTouchPosition.y * -2, deltaTouchPosition.x * forceXZ.y);
                        //print(force);

                        if (force.magnitude > 10) {
                            force = force.normalized * 10;
                        }

                        //forceXZ *= (touch.position.x - touchPosition.x);
                        //Vector3 force = new Vector3(forceXZ.x / 10f, (touch.position.y - touchPosition.y), forceXZ.y / 10f);


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
        if (Physics.Raycast(ray, out hit) && hit.transform.tag != "Plane") {
            objectDrag = true;

            Transform tf = hit.transform;
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
                touchInstances.Add(new TouchInstance(touch.fingerId, tf, FindClosestCube(tf), tf.position, touch.position));
                print(FindClosestCube(tf).name);
            }
        }
    }

    public void Reset() {
        Application.LoadLevel(0);
        touchInstances = new ArrayList();
    }

    Transform FindClosestCube(Transform tf) {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("3DCube");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = tf.position;
        foreach (GameObject go in gos) {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance) {
                if (go.name != tf.name && go.name != "3D Cubes") {
                    closest = go;
                    distance = curDistance;
                } 
            }
        }
        return closest.GetComponent<Transform>();
    }

    private void IgnoreLayer() {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("3DCube");
        foreach (GameObject cube in cubes) {
            cube.layer = 2;
        }
    }

    private void ResetLayer() {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("3DCube");
        foreach (GameObject cube in cubes) {
            cube.layer = 1;
        }
    }
}
