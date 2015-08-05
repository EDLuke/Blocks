using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class StateController : MonoBehaviour {

	private static Stack states;

	// Use this for initialization
	void Start () {
		states = new Stack ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	struct ObjectInfo{
		public string name;
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 scale;
		public Color color;
	}

	public static void Push(){
		ArrayList state = new ArrayList();
		foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag ("3DCube")) {
			ObjectInfo info = new ObjectInfo();
			info.name = gameObject.name;
			info.position = gameObject.transform.position;
			info.rotation = gameObject.transform.rotation;
			info.scale = gameObject.transform.localScale;
			info.color = gameObject.GetComponent<Renderer> ().material.color;
			state.Add (info);
		}
		states.Push (state);
	}

	public static void Pop(){
		GameObject[] currentObjects = GameObject.FindGameObjectsWithTag ("3DCube");


		//First detect if the up-most state is the same as the current state
		ArrayList statePeek = states.Peek () as ArrayList;
		bool matchCurrentState = true;
		foreach (GameObject currentObject in currentObjects) {
			ObjectInfo info = new ObjectInfo();
			info.name = currentObject.name;
			info.position = currentObject.transform.position;
			info.rotation = currentObject.transform.rotation;
			info.scale = currentObject.transform.localScale;
			info.color = currentObject.GetComponent<Renderer> ().material.color;
			if(!statePeek.Contains(info)){
				matchCurrentState = false;
			}
		}

		if (matchCurrentState) {
			states.Pop();
			print (states.Count);
		}
		
		if (states.Count == 0) {
			return;
		}
		
		ArrayList state = states.Pop () as ArrayList;
		foreach (ObjectInfo gameObject in state) {
			bool found = false;
			foreach(GameObject currentObject in currentObjects){
				if(currentObject.name == gameObject.name){
					found = true;
					print (currentObject.name + "\t" + gameObject.name);
					print (currentObject.transform.position + "\t" + gameObject.position);
					currentObject.transform.position = gameObject.position;
					currentObject.transform.rotation = gameObject.rotation;
					break;
				}
			}
			if(!found){
				//Instantiate(gameObject, gameObject.position, gameObject.rotation);
			}
		}
	}
}
