using UnityEngine;
using System.Collections;

public class NewPlayerController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (10*Time.deltaTime,25*Time.deltaTime,15*Time.deltaTime);
	}
}
