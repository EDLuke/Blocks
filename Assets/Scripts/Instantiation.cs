using UnityEngine;
using System.Collections;

public class Instantiation : MonoBehaviour {

	public GameObject sphere;
	public GameObject cube;
	public GameObject triangle;
	public GameObject cone;
	public GameObject cylinder;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CreateSphere() {
        Instantiate(sphere, new Vector3(0f, 5f, 0f), Quaternion.identity);
    }

	public void CreateCube() {
        Instantiate(cube, new Vector3(0f, 5f, 0f), Quaternion.identity);
    }

	public void CreateCylinder() {
        Instantiate(cylinder, new Vector3(0f, 5f, 0f), Quaternion.identity);
    }

	public void CreateCone() {
        //MeshFilter filter = cone.GetComponent<MeshFilter>();
        //filter.sharedMesh = cube.GetComponent<MeshFilter>().sharedMesh;
        Instantiate(cone, new Vector3(0f, 5f, 0f), Quaternion.identity);
    }

	public void CreateTriangle() {
        Instantiate(triangle, new Vector3(0f, 5f, 0f), Quaternion.identity);
    }
}
