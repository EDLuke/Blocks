using UnityEngine;
using System.Collections;

public class AuraController : MonoBehaviour {
	public GameObject parent;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (transform != null) {
			transform.position = parent.transform.position;
			transform.rotation = parent.transform.rotation;
		}

	}
}
