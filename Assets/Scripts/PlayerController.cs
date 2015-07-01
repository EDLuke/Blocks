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

    //void FixedUpdate () {

    //}
		

    //void OnMouseDown(){
    //    //This grabs the position of the object in the world and turns it into the position on the screen
    //    gameObjectSreenPoint = Camera.main.ViewportToWorldPoint(transform.position);

    //    //Sets the mouse pointers vector3
    //    mousePreviousLocation = new Vector3(Input.mousePosition.x, Input.mousePosition.y, gameObjectSreenPoint.z);
    //    objectDrag = true;
    //}

    //void DoubleClick() {

    //}

    //public Vector3 gameObjectSreenPoint;
    //public Vector3 mousePreviousLocation;
    //public Vector3 mouseCurLocation;
	
    //public Vector3 force;
    //public Vector3 objectCurrentPosition;
    //public Vector3 objectTargetPosition;
    //public float topSpeed = 500;
	
    
    //void OnMouseDrag()
    //{
    //    mouseCurLocation = new Vector3(Input.mousePosition.x, Input.mousePosition.y, gameObjectSreenPoint.z);
    //    force = mouseCurLocation - mousePreviousLocation;//Changes the force to be applied
    //    mousePreviousLocation = mouseCurLocation;

    //    if (force.magnitude > topSpeed){
    //        force = force.normalized * topSpeed;
    //    }

    //    force = Quaternion.AngleAxis(CameraController.currentRotationX, Vector3.up) * force;
    //    //force.y *= 2.5f;
    //    //force.x *= 2;
    //    rb.velocity = force;
    //}
	
    //public void OnMouseUp()
    //{
    //    objectDrag = false;
    //}
}
