using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {
	private const float floatHeight = 5f;

	public Rigidbody rb;
	public static bool objectDrag = false;
    public AudioSource aud;
	public bool created;
	public bool touchingOtherObject;

	// Use this for initialization
	void Start () {
		touchingOtherObject = false;
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
		touchingOtherObject = true;
		print ("Colliding with " + col.transform.name);
        aud.volume = rb.velocity.magnitude;
        aud.Play();
    }

	void OnCollisionExit(Collision col) {
		touchingOtherObject = false;
		
		print ("Exit colliding with " + col.transform.name);
	}


}