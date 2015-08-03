using UnityEngine;
using System.Collections;

public class NewPlayerController : MonoBehaviour {
	public static Vector3 onScreenPosition;

	private const float auraScale = 1.5f;


	// Use this for initialization
	void Start () {
		CreateAura ();
		onScreenPosition = Camera.main.WorldToScreenPoint (transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (10*Time.deltaTime,25*Time.deltaTime,15*Time.deltaTime);
		transform.position = Camera.main.ScreenToWorldPoint (onScreenPosition);
//		print (Camera.main.WorldToScreenPoint(transform.position));
	}

	void CreateAura(){
		GameObject aura = gameObject;
		aura.transform.position = transform.position;
		GameObject auraCreated = Instantiate (aura, aura.transform.position, Quaternion.identity) as GameObject;
		auraCreated.GetComponent<MeshRenderer> ().enabled = false;
		auraCreated.GetComponent<Collider> ().enabled = true;
		auraCreated.transform.localScale = auraCreated.transform.localScale * auraScale;
		auraCreated.AddComponent<AuraController> ();
		auraCreated.GetComponent<AuraController> ().parent = gameObject;
		auraCreated.tag = "Aura";
		Destroy (auraCreated.GetComponent<NewPlayerController> ());
		auraCreated.layer = LayerMask.NameToLayer( "AuraLayer" );
	}
}
