using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {
	public Rigidbody rb;
	public static bool objectDrag = false;
    public AudioSource aud;


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
        aud = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

        
    }

    void OnCollisionEnter(Collision col) {
        aud.volume = rb.velocity.magnitude;
        aud.Play();
    }
}