using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
	private const float large_multi = 1.5f;
	private const float small_multi = 0.5f;
	private Vector3 newPosition = new Vector3(0f, 5f, 0f);

	//GameObjects for creation
	public GameObject cube;
	public GameObject cone;
	public GameObject cylinder;
	public GameObject triangleEquilateral;
	public GameObject triangleAcute;
	public GameObject triangleObtuse;
	public GameObject triangleScalene;
	public GameObject rectangleCube;
	public GameObject rectangleFlat;

	private string selectedShape;
	private string selectedVariation;
	private string selectedSize;
	private string selectedColor;

	public GameObject panelShape;
	public GameObject panelVariation;
	public GameObject panelSize;
	public GameObject panelColor;

	//Array of panels
	private GameObject[] panels;
	
	//Panel positions
	private Vector3 originalSizePosition;
	private Vector3 originalColorPosition;
	private Vector3 changedSizePosition;
	private Vector3 changedColorPosition;

	//The bin button
	public Button btnBin;

	//The button used to show selected choice
	public Button btnSelected;
	private Button[] selectedBtns;

	//Smiley face button used for confirmation
	public Button btnConfirm;

	//Storing infomation for the created object (floating with UI)
	private GameObject createdObject;

	// Use this for initialization
	void Start () {
		btnSelected.gameObject.SetActive (false);
		selectedBtns = new Button[]{Instantiate(btnSelected), Instantiate(btnSelected), Instantiate(btnSelected), Instantiate(btnSelected)};

		panels = new GameObject[]{panelShape, panelVariation, panelSize, panelColor};
		for (int i = 0; i < panels.Length; i++) {
			GameObject panel = panels [i];
			panel.SetActive (false);
			assignBtnListener (panel);
			selectedBtns[i].transform.SetParent(panel.transform);
		}
		btnBin.onClick.AddListener (delegate {
			panelShape.SetActive (true);
		});
		btnConfirm.onClick.AddListener (delegate {
			createdObject.transform.position = newPosition;
			createdObject.GetComponent<Rigidbody>().useGravity = true;
			Destroy(createdObject.GetComponent<NewPlayerController>()); //stop rotation
			createdObject.transform.rotation = Quaternion.identity; //reset rotation
			createdObject = new GameObject ();
			
			btnConfirm.gameObject.SetActive (false);

			//De-active the panels and buttons
			disableUI();
		});
		btnConfirm.gameObject.SetActive (false);
	
		calculatePanelsPosition ();
	}

	private void assignBtnListener(GameObject currentPanel){
		GameObject nextPanel = new GameObject ();
		GameObject previousPanel = new GameObject ();
		Button[] buttons = currentPanel.GetComponentsInChildren<Button> (true);
		foreach (Button btn in buttons) {
		
			if(btn.name.Contains("Back")){
				btn.onClick.AddListener(delegate {
					switch (currentPanel.name.Substring (5)){
					case "Shape":
						movePanelsToOriginal(true);
						selectedBtns[0].gameObject.SetActive(false);
						break;
					case "Variation":
						previousPanel = panelShape;
						selectedBtns[0].gameObject.SetActive(false);
						
						break;
					case "Size":
						if(panelVariation.activeSelf){
							previousPanel = panelVariation;
						}
						else{
							previousPanel = panelShape;
						}
						selectedBtns[0].gameObject.SetActive(false);			
						selectedBtns[1].gameObject.SetActive(false);
						
						break;
					case "Color":
						previousPanel = panelSize;
						selectedBtns[2].gameObject.SetActive(false);
						selectedBtns[3].gameObject.SetActive(false);
						break;
					}
					enablePanel(previousPanel);
					previousPanel.SetActive(true);
					currentPanel.SetActive(false);
					disableObjectShowing();
					
				});
			}
			else{
				string btnName = btn.name.Substring(3);
				Vector2 btnPosition = btn.GetComponent<RectTransform>().anchoredPosition;

				btn.onClick.AddListener(delegate {
					switch (currentPanel.name.Substring (5)){
					case "Shape":			
						if(btnName == "Rectangle" || btnName == "Triangle"){
							nextPanel = panelVariation;
							changeVariationPanel(btnName == "Rectangle");
							movePanelsToOriginal(true);	
						}
						else{
							nextPanel = panelSize;
							movePanelsToOriginal(false);	
							selectedShape = btnName;
							showSizePanel();		
						}

						selectedBtns[0].GetComponent<RectTransform>().anchoredPosition = btnPosition;
						selectedBtns[0].gameObject.SetActive(true);
						break;
					case "Variation":
						nextPanel = panelSize;
						selectedShape = btnName;
						showSizePanel();
						selectedBtns[1].GetComponent<RectTransform>().anchoredPosition = btnPosition;
						selectedBtns[1].gameObject.SetActive(true);
						break;
					case "Size":
						nextPanel = panelColor;
						selectedSize = btnName;
						selectedBtns[2].GetComponent<RectTransform>().anchoredPosition = btnPosition;
						selectedBtns[2].gameObject.SetActive(true);
						break;
					case "Color":
						selectedColor = btnName;
						selectedBtns[3].GetComponent<RectTransform>().anchoredPosition = btnPosition;
						selectedBtns[3].gameObject.SetActive(true);
						disableObjectShowing();
						displayObject();
						break;
					}
					disablePanel(currentPanel);
					nextPanel.SetActive(true);
				});
			}
		}
	}

	private void displayObject(){
		//First choose shape
		GameObject creation = new GameObject ();
		Vector3 scale = new Vector3 ();

		switch (selectedShape) {
		case "Rectangle":
			creation = rectangleCube;
			break;
		case "RectangleFlat":
			creation = rectangleFlat;		
			break;
		case "TriangleIsoscelesAcute":
			creation = triangleAcute;
			break;
		case "TriangleIsoscelesObtuse":
			creation = triangleObtuse;
			break;
		case "TriangleScalene":
			creation = triangleScalene;
			break;
		case "TriangleEquilateral":
			creation = triangleEquilateral;
			break;
		case "Cone":
			creation = cone;
			break;
		case "Cube":
			creation = cube;
			break;
		case "Cylinder":
			creation = cylinder;
			break;
		}

		switch (selectedSize) {
		case "Large":
			scale = creation.transform.localScale * large_multi;
			break;
		case "Small":
			scale = creation.transform.localScale * small_multi;		
			break;
		case "Medium":
			scale = creation.transform.localScale;
			break;
		}

		Color color = Color.white;
		switch (selectedColor) {
		case "Red":
			color = Color.red;
			break;
		case "Orange":
			color = new Color(1f, 0.5f, 0f);
			break;
		case "Yellow":
			color = Color.yellow;
			break;
		case "Green":
			color = Color.green;
			break;
		case "Blue":
			color = Color.blue;
			break;
		case "Violet":
			color = new Color(1f, 0f, 1f);
			break;
		}	

		createdObject = Instantiate(creation, new Vector3 (-5.1f, 12f, -15.34f), Quaternion.identity) as GameObject;
		createdObject.GetComponent<Renderer> ().material = creation.GetComponent<Renderer> ().material;
		createdObject.GetComponent<Renderer> ().material.color = color;
		createdObject.transform.localScale = scale;
		createdObject.GetComponent<Rigidbody> ().useGravity = false;
		createdObject.AddComponent <NewPlayerController>();
		createdObject.SetActive (true);
		btnConfirm.gameObject.SetActive (true);
	}

	private void disableObjectShowing(){
		if (createdObject != null && createdObject.name != "New Game Object") {
			Mesh createdObjectMesh = createdObject.GetComponent<MeshFilter> ().mesh;
			//Storing the mesh for triangles and cones
			Destroy (createdObject);
			switch(createdObject.name){
			case "TriangleIsoscelesAcute":
				triangleAcute.GetComponent<MeshFilter> ().mesh = createdObjectMesh;
				break;
			case "TriangleIsoscelesObtuse":
				triangleObtuse.GetComponent<MeshFilter> ().mesh = createdObjectMesh;
				break;
			case "TriangleScalene":
				triangleScalene.GetComponent<MeshFilter> ().mesh = createdObjectMesh;
				break;
			case "TriangleEquilateral":
				triangleEquilateral.GetComponent<MeshFilter> ().mesh = createdObjectMesh;
				break;
			case "Cone":
				cone.GetComponent<MeshFilter> ().mesh = createdObjectMesh;
				break;
			}
		}

		btnConfirm.gameObject.SetActive(false);
	}

	public void disableUI(){
		foreach (GameObject panel in panels) {
			panel.SetActive(false);
			foreach(Button btn in panel.GetComponentsInChildren<Button>(true)){
				btn.interactable = true;
			}
		}
		foreach (Button btn in selectedBtns) {
			btn.gameObject.SetActive(false);
		}
	}


	private void changeVariationPanel(bool rectangle){
		Button[] buttons = panelVariation.GetComponentsInChildren<Button> (true);
		for (int i = 0; i < buttons.Length - 2; i++) {
			if(i < 4){
				buttons[i].gameObject.SetActive (!rectangle);
			}
			else{
				buttons[i].gameObject.SetActive (rectangle);		
			}
		}

		if (rectangle) { //Two variations
			panelVariation.GetComponent<RectTransform>().anchoredPosition = new Vector2(128, -75);
			panelVariation.GetComponent<RectTransform>().sizeDelta = new Vector2(153.9f, 50);	

			//Position (including back button)
			buttons[4].GetComponent<RectTransform>().anchoredPosition = new Vector2(-51.95f, 0);
			buttons[5].GetComponent<RectTransform>().anchoredPosition = new Vector2(-1.95f, 0);
			buttons[6].GetComponent<RectTransform>().anchoredPosition = new Vector2(48.05f, 0);
			
		} else { //Three variation
			panelVariation.GetComponent<RectTransform>().anchoredPosition = new Vector3(175.7f, -75);
			panelVariation.GetComponent<RectTransform>().sizeDelta = new Vector2(249.3f, 50);

			//Position (including back button)
			buttons[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-99.65f, 0);
			buttons[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-49.65f, 0);
			buttons[6].GetComponent<RectTransform>().anchoredPosition = new Vector2(99.65f, 0);
		}
	}

	private void showSizePanel(){
		print (selectedShape);
		Button[] buttons = panelSize.GetComponentsInChildren<Button> (true);
		buttons[0].image.sprite = Resources.Load<Sprite>("UI/" + selectedShape + "_Small");
		buttons[1].image.sprite = Resources.Load<Sprite>("UI/" + selectedShape);
		buttons[2].image.sprite = Resources.Load<Sprite>("UI/" + selectedShape + "_Large");
	}

	//Do not disable panelColor
	private void disablePanel(GameObject currentPanel){
		if (currentPanel.name != "panelColor") {
			Color panelColor = currentPanel.GetComponent<Image> ().color;
			panelColor.a = 0.5f;
			Button[] buttons = currentPanel.GetComponentsInChildren<Button> (true);
			foreach (Button btn in buttons) {
				btn.interactable = false;
			}
			currentPanel.GetComponent<Image> ().color = panelColor;
		}
	}

	private void enablePanel(GameObject previousPanel){
		if (previousPanel.GetComponent<Image> () == null) { //currentPanel is panelShape
			panelShape.SetActive (false);
		} else {
			Color panelColor = previousPanel.GetComponent<Image> ().color;
			panelColor.a = 0f;
			Button[] buttons = previousPanel.GetComponentsInChildren<Button> (true);
			foreach (Button btn in buttons) {
				btn.interactable = true;
			}
			previousPanel.GetComponent<Image> ().color = panelColor;
		}


	}

	//Find the panels original / with variation positions
	private void calculatePanelsPosition(){
		originalSizePosition = panelSize.GetComponent<Transform> ().position;
		originalColorPosition = panelColor.GetComponent<Transform> ().position;

		changedSizePosition = panelSize.GetComponent<Transform> ().position;
		changedColorPosition = panelColor.GetComponent<Transform> ().position;

		changedSizePosition.y += 50;
		changedColorPosition.y += 50;
	}

	//Move Panels
	private void movePanelsToOriginal(bool original){
		if (original) {
			panelSize.transform.position = originalSizePosition;
			panelColor.transform.position = originalColorPosition;		
		} else {
			panelSize.transform.position = changedSizePosition;
			panelColor.transform.position = changedColorPosition;		
		}
	}
	

	/*public void chooseMaterial(Material color){
		selectedMaterial = color;
		createObject ();
		panelColor.SetActive (false);
		
	}*/

	// Update is called once per frame
	void Update () {

	}
	
	/*private void createObject(){
		GameObject tempObject = Instantiate(selectedShape, new Vector3(0f, 5f, 0f), Quaternion.identity) as GameObject;
		tempObject.GetComponent<Renderer> ().material = selectedMaterial;
		tempObject.SetActive (true);
	}*/
}
