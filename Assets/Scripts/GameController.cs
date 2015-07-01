using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    private Hashtable touchCoords;  //Key: fingerId(s) - Arraylist / Value: tranform
    private Hashtable objectCoords; //Key: tranform / Value: initial position

    private ArrayList touchInstances;

    public Shader silhouette;
    private Shader standard;

    public static bool objectDrag = false;

    private class TouchInstance{
        private ArrayList fingerIds;
        private Transform transform;
        private Vector3 position;
        private Transform closest;

        public TouchInstance(int initialId, Transform transform, Vector3 initialPosition) {
            fingerIds = new ArrayList();
            fingerIds.Add(initialId);
            this.transform = transform;
            this.position = initialPosition;
        }

        public void AddFingerId(int fingerId) {
            fingerIds.Add(fingerId);
        }

        public void RemoveFingerId(int fingerId) {
            fingerIds.Remove(fingerId);
        }

        public Transform GetTransform() {
            return transform;
        }

        public bool ContainsId(int fingerId) {
            return fingerIds.Contains(fingerId);
        }

        public Vector3 GetPosition() {
            return position;
        }

        public void SetPosition(Vector3 position) {
            this.position = position;
        }

        public int TouchCount() {
            return fingerIds.Count;
        }
    }

	// Use this for initialization
	void Start () {
        touchCoords = new Hashtable();
        objectCoords = new Hashtable();
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

        foreach (TouchInstance instance in touchInstances) {
            if (instance.TouchCount() > 1 && instance.ContainsId(touch.fingerId)) {
                instance.RemoveFingerId(touch.fingerId);
                newList.Add(instance);
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
                objectDrag = true;
                Transform tf = instance.GetTransform();
                Rigidbody rb = tf.GetComponent<Rigidbody>();
                Vector3 previousPos = instance.GetPosition();
                Vector3 currentPos = new Vector3();

                if (instance.TouchCount() == 1) { //Free Moving 2D
                    IgnoreLayer();
                    if (Physics.Raycast(rayMove, out hitMove)) {
                        currentPos = hitMove.point;
                        print(hitMove.point);
                        currentPos.y = 1;
                    }
                    Vector3 force = currentPos - previousPos;

                    force *= 15f;
                    //print(rb.name + "\t" + force);

                    rb.velocity = force;
                    instance.SetPosition(currentPos);
                    ResetLayer();
                }
                else { //Clipped to the nearest object

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
                touchInstances.Add(new TouchInstance(touch.fingerId, tf, tf.position));
            }
        }
    }

    public void Reset() {
        Application.LoadLevel(0);
        touchCoords = new Hashtable();
        objectCoords = new Hashtable();
    }

    GameObject FindClosestCube(Rigidbody rb) {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("3DCube");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = rb.position;
        foreach (GameObject go in gos) {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance) {
                if (go.name != rb.name && go.name != "3D Cubes") {
                    closest = go;
                    distance = curDistance;
                } 
            }
        }
        return closest;
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
