using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

	private GameObject selected;
	public GameObject panelColor;

	public Button cubeBtn;

	private Color[] colors = {Color.black, Color.blue, Color.red, Color.yellow, Color.green, new Color(1.0f, 0.5f, 0f)};

	// Use this for initialization
	void Start () {
		panelColor.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void chooseShape(GameObject gameObject){
		panelColor.SetActive (true);
		selected = gameObject;

	}

	public void chooseColor(string colorString){
		Color color = Color.white;
		switch (colorString) {
		case "Red":
			color = Color.red;
			break;
		}
		selected.GetComponent<Renderer>().material.SetColor("_Color", color);
		Instantiate(gameObject, new Vector3(0f, 5f, 0f), Quaternion.identity);
		panelColor.SetActive (false);
		
		
	}

	void OnGUI(){
		/*GUI.backgroundColor = Color.clear;
		if (GUI.Button(new Rect (Screen.width / 2 - 50 * 3, Screen.height - 50, 50, 50), cubeTexture)) {
			GameObject button = new GameObject();
			button.transform.parent = this.transform;
			button.AddComponent<RectTransform>();
			button.AddComponent<Button>();
			button.transform.position = new Vector3(0, 0, 0);
			button.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
			button.GetComponent<Button>().onClick.AddListener(() => instantiateObject(0, cube));
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
		}*/


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
