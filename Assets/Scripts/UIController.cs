using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
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
	private Material selectedMaterial;
	private string selectedVariation;
	private string selectedSize;

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

	// Use this for initialization
	void Start () {
		panels = new GameObject[]{panelShape, panelVariation, panelSize, panelColor};
		foreach (GameObject panel in panels) {
			panel.SetActive(false);
			assignBtnListener(panel);
		}
		btnBin.onClick.AddListener (delegate {
			panelShape.SetActive (true);
		});

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
						break;
					case "Variation":
						previousPanel = panelShape;
						break;
					case "Size":
						if(panelVariation.activeSelf){
							previousPanel = panelVariation;
						}
						else{
							previousPanel = panelShape;
						}
						break;
					case "Color":
						previousPanel = panelSize;
						break;
					}
					enablePanel(previousPanel);
					previousPanel.SetActive(true);
					currentPanel.SetActive(false);
				});
			}
			else{
				string shapeName = btn.name.Substring(3);
			
				btn.onClick.AddListener(delegate {
					switch (currentPanel.name.Substring (5)){
					case "Shape":			
						if(shapeName == "Rectangle" || shapeName == "Triangle"){
							nextPanel = panelVariation;
							changeVariationPanel(shapeName == "Rectangle");
							movePanelsToOriginal(true);	
						}
						else{
							nextPanel = panelSize;
							movePanelsToOriginal(false);	
							selectedShape = shapeName;
							showSizePanel();		
						}

						break;
					case "Variation":
						nextPanel = panelSize;
						selectedShape = shapeName;
						showSizePanel();
						break;
					case "Size":
						nextPanel = panelColor;
						break;
					case "Color":
						break;
					}
					disablePanel(currentPanel);
					nextPanel.SetActive(true);
				});
			}
		}
	}

	private void changeVariationPanel(bool rectangle){
		Button[] buttons = panelVariation.GetComponentsInChildren<Button> (true);
		for (int i = 0; i < buttons.Length - 1; i++) {
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
		Color panelColor = previousPanel.GetComponent<Image> ().color;
		panelColor.a = 0f;
		Button[] buttons = previousPanel.GetComponentsInChildren<Button> (true);
		foreach (Button btn in buttons) {
			btn.interactable = true;
		}
		previousPanel.GetComponent<Image> ().color = panelColor;

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
