﻿using UnityEngine;
using System.Collections;

public class NewPlayerController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		CreateAura ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (10*Time.deltaTime,25*Time.deltaTime,15*Time.deltaTime);
	}

	void CreateAura(){
		GameObject aura = gameObject;
		aura.transform.position = transform.position;

		
		GameObject auraCreated = Instantiate (aura, aura.transform.position, Quaternion.identity) as GameObject;
		auraCreated.GetComponent<MeshRenderer> ().enabled = false;
		auraCreated.transform.localScale = auraCreated.transform.localScale * 1.1f;
		auraCreated.AddComponent<AuraController> ();
		auraCreated.GetComponent<AuraController> ().parent = gameObject;
		auraCreated.tag = "Aura";
		Destroy (auraCreated.GetComponent<NewPlayerController> ());
		//Destroy (auraCreated.GetComponent<Collider> ());
		auraCreated.layer = LayerMask.NameToLayer( "AuraLayer" );
	}
}
