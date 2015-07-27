using UnityEngine;
using System.Collections;

public class AuraController : MonoBehaviour {
	public GameObject parent;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (parent != null) {
			transform.position = parent.transform.position;
			transform.rotation = parent.transform.rotation;
		} else {
			Destroy(gameObject);
		}

		if (!WithinBounds ()) {
			GetComponent<MeshRenderer> ().enabled = true;
				
		} else {
			GetComponent<MeshRenderer> ().enabled = false;	
		}
	}

	public GameObject Parent {
		get {
			return parent;
		}
	}

	private bool WithinBounds(){
		return (transform.position.x <= 15 && transform.position.y <= 15);
	}
}
