using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    private ArrayList touchInstances;

    public Shader silhouette;
    private Shader standard;

    public static bool objectDrag = false;

    private class TouchInstance{
        private ArrayList fingerIds;
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

        public Vector2 GetNormalizedDistance() {
            Vector3 normalized = (closest.position - transform.position).normalized;
            return new Vector2(normalized.x, normalized.z);
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
                    //print(rb.name + "\t" + force);
                    //print(force);

                    rb.velocity = force;
                    //instance.SetPosition(currentPos);
                    ResetLayer();
                    instance.SetTouchPosition(touch.position);

                }
                else { //Clipped to the nearest object
                    //print(touch.deltaPosition);
                    Vector2 forceXZ = instance.GetNormalizedDistance() / 3f;
                    Vector2 touchPosition = instance.GetTouchPosition();

                    forceXZ *= (touchPosition.x - touch.position.x);
                    Vector3 force = new Vector3(forceXZ.x, (touch.position.y - touchPosition.y) / 10f, forceXZ.y) * 2f;
                    
                    print(force);
                    rb.velocity = force;
                    instance.SetTouchPosition(touch.position);

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
