using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {
	public Texture cubeTexture;
	public Texture triangleTexture;
	public Texture coneTexture;
	public Texture sphereTexture;
	public Texture cylinderTexture;
	public Texture rectangleTexture;

	public GameObject sphere;
	public GameObject cube;
	public GameObject triangle;
	public GameObject cone;
	public GameObject cylinder;
	public GameObject rectangle;

	private Color[] colors = {Color.black, Color.blue, Color.red, Color.yellow, Color.green, new Color(1.0f, 0.5f, 0f)};

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		GUI.backgroundColor = Color.clear;
		if (GUI.Button(new Rect (Screen.width / 2 - 50 * 3, Screen.height - 50, 50, 50), cubeTexture)) {
			if (GUI.Button(new Rect (Screen.width / 2 - 50 * 3, Screen.height - 100, 50, 50), cubeTexture)) {
				print ("LOL");
			}
			//instantiateObject(Screen.width / 2 - 50 * 3, cube);
        	//Instantiate(cube, new Vector3(0f, 5f, 0f), Quaternion.identity);
		}

		if (GUI.Button(new Rect (Screen.width / 2 - 50 * 2, Screen.height - 50, 50, 50), triangleTexture)) {
			Instantiate(triangle, new Vector3(0f, 5f, 0f), Quaternion.identity);
		}

		if (GUI.Button(new Rect (Screen.width / 2 - 50 * 1, Screen.height - 50, 50, 50), coneTexture)) {
			Instantiate(cone, new Vector3(0f, 5f, 0f), Quaternion.identity);
		}

		if (GUI.Button(new Rect (Screen.width / 2, Screen.height - 50, 50, 50), sphereTexture)) {
			Instantiate(sphere, new Vector3(0f, 5f, 0f), Quaternion.identity);
		}

		if (GUI.Button(new Rect (Screen.width / 2 + 50 * 1, Screen.height - 50, 50, 50), cylinderTexture)) {
			Instantiate(cylinder, new Vector3(0f, 5f, 0f), Quaternion.identity);
		}

		if (GUI.Button(new Rect (Screen.width / 2 + 50 * 2, Screen.height - 50, 50, 50), rectangleTexture)) {
			Instantiate(rectangle, new Vector3(0f, 5f, 0f), Quaternion.identity);
		}
	}

	private void instantiateObject(float left, GameObject gameObject){
		/*int count = 0;
		foreach (Color color in colors) {
			GUI.backgroundColor = color;
			if(GUI.Button(new Rect(left + 50 * count, Screen.height - 100, 50, 50), "")){
				gameObject.GetComponent<Renderer>().material.SetColor("_Color", color);
				Instantiate(gameObject, new Vector3(0f, 5f, 0f), Quaternion.identity);
			}
			count++;
		}*/
		GUI.backgroundColor = Color.yellow;
		if(GUI.Button(new Rect(left + 50 * 0, Screen.height - 100, 50, 50), "LOL")){
			gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
			Instantiate(gameObject, new Vector3(0f, 5f, 0f), Quaternion.identity);
		}
	}
}
