using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
	public GameObject cube;
	public GameObject cone;
	public GameObject cylinder;
	public GameObject triangleEquilateral;
	public GameObject triangleAcute;
	public GameObject triangleObtuse;
	public GameObject triangleScalene;
	public GameObject rectangleCube;
	public GameObject rectangleFlat;

	private GameObject selectedShape;
	private Material selectedMaterial;
	private string selectedVariation;
	private string selectedSize;

	public GameObject panelShape;
	public GameObject panelVariation;
	public GameObject panelSize;
	public GameObject panelColor;

	private GameObject[] panels;

	public Button btnBin;

	// Use this for initialization
	void Start () {
		panels = new GameObject[]{panelShape, panelVariation, panelSize, panelColor};
		foreach (GameObject panel in panels) {
			panel.SetActive(false);
			assignBtnListen(panel);
		}
		btnBin.onClick.AddListener (delegate {
			panelShape.SetActive (true);
		});


	}

	private void assignBtnListen(GameObject currentPanel){
		GameObject nextPanel = new GameObject ();
		Button[] buttons = currentPanel.GetComponentsInChildren<Button> (true);
		foreach (Button btn in buttons) {
			
			btn.onClick.AddListener(delegate {
				switch (currentPanel.name.Substring (5)) {
				case "Shape":
					if(btn.name.Substring (3) == "RectangleCube" || btn.name.Substring (3) == "TriangleEquilateral"){
						nextPanel = panelVariation;
						movePanels(true);
					}
					else{
						nextPanel = panelSize;
						movePanels(false);
					}
					break;
				case "Variation":
					nextPanel = panelSize;
					break;
				case "Size":
					nextPanel = panelColor;
					break;
				case "Color":
					break;
				}
				nextPanel.SetActive(true);
			});
		}

	}

	private void movePanels(bool variation){
		Vector3 currentSizePosition = panelSize.GetComponent<Transform> ().position;
		Vector3 currentColorPosition = panelColor.GetComponent<Transform> ().position;
		
		Vector3 upSizePosition = currentSizePosition;
		Vector3 downSizePosition = currentSizePosition;

		Vector3 upColorPosition = currentColorPosition;
		Vector3 downColorPosition = currentColorPosition;

		upSizePosition.y += 50;
		downSizePosition.y -= 50;

		upColorPosition.y += 50;
		downColorPosition.y -= 50;

		if (variation) {
			panelSize.transform.position = downSizePosition;
			panelColor.transform.position = downColorPosition;		
		} else {
			panelSize.transform.position = upSizePosition;
			panelColor.transform.position = upColorPosition;		
		}
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
