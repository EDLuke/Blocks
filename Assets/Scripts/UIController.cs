using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
	public GameObject sphere;
	public GameObject cube;
	public GameObject triangle;
	public GameObject cone;
	public GameObject cylinder;
	public GameObject rectangle;

	private GameObject selectedShape;
	private Material selectedMaterial;
	public GameObject panelColor;
	public GameObject panelShape;

	// Use this for initialization
	void Start () {
		panelColor.SetActive (false);
	}


	public void chooseShape(GameObject shape){
		panelColor.SetActive (true);
		selectedShape = shape;
	}

	public void chooseMaterial(Material color){
		selectedMaterial = color;
		createObject ();
		panelColor.SetActive (false);
		
	}

	// Update is called once per frame
	void Update () {

	}
	
	private void createObject(){
		GameObject tempObject = Instantiate(selectedShape, new Vector3(0f, 5f, 0f), Quaternion.identity) as GameObject;
		tempObject.GetComponent<Renderer> ().material = selectedMaterial;
		tempObject.SetActive (true);
	}
}
