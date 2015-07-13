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
		if (transform.position.y < 0) {
			if(gameObject.name.Contains("Clone")){
				Destroy(gameObject);
			}
			else{
				gameObject.SetActive (false);
			}
		}
        
    }

    void OnCollisionEnter(Collision col) {
        aud.volume = rb.velocity.magnitude;
        aud.Play();
    }
}