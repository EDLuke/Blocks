﻿using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {
	private const float floatHeight = 5f;
	private const float collisionTolerance = 0.001f;
	private Transform topTransform;

	public Rigidbody rb;
	public static bool objectDrag = false;
    public AudioSource aud;
	public bool created;
	public bool touchingOtherObject;

	// Use this for initialization
	void Start () {
		touchingOtherObject = false;
		topTransform = null;
		rb = GetComponent<Rigidbody>();
        aud = GetComponent<AudioSource>();
		created = false;
	}


	// Update is called once per frame
	void Update () {
		if (transform.position.y < 0) {
			if (gameObject.name.Contains ("Clone")) {
				Destroy (gameObject);
			} else {
				gameObject.SetActive (false);
			}
		}        
    }

    void OnCollisionEnter(Collision col) {
		//If touching other object then the 2D free movement change to use force instead of moving transform position
		touchingOtherObject = true;

		//If the touching object is on top of this object, then assign topTransform to the touching object
		Transform touchingTransform = col.transform;
		if (Mathf.Abs (touchingTransform.position.y - (transform.position.y + transform.lossyScale.y / 2)) <= collisionTolerance) {
			topTransform = touchingTransform;
		}
		
		//Play Audio
        aud.volume = rb.velocity.magnitude;
        aud.Play();
    }

	void OnCollisionExit(Collision col) {
		touchingOtherObject = false;

		//If topTransform is no longer touching this object then assign topTransform to null
		if (Mathf.Abs (topTransform.position.y - (transform.position.y + transform.lossyScale.y / 2)) > collisionTolerance) {
			topTransform = null;
		}
	}

	public Transform TopTransform(){ //Get the object that is stacked at the top
		//if there is not topTransform, return the current transform, otherwise use recursion to get the top-most transform
		return (topTransform == null) ? transform : topTransform.gameObject.GetComponent<PlayerController> ().TopTransform ();
	}


}