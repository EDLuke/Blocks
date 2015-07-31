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

		//If parent is not newly created and parent is not destoryed, enable the mesh to show the red silhoutte
		if (parent.GetComponent<PlayerController>() != null && parent.GetComponent<PlayerController>().created) {
			if (!WithinBounds()) {
				GetComponent<MeshRenderer>().enabled = true;
				
			}
			else {
				GetComponent<MeshRenderer>().enabled = false;
			}
		}  
	}

	public GameObject Parent {
		get {
			return parent;
		}
	}

	//Check if the parent position is on top of the table
	private bool WithinBounds(){
        return (Mathf.Abs(parent.transform.position.x) <= 15 && Mathf.Abs(parent.transform.position.z) <= 15);
	}
}
