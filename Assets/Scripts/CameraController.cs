using UnityEngine;
using System.Collections;

public class CameraController: MonoBehaviour
{
    public float orthoZoomSpeed = 0.01f;

	public float minimumX = -2;
	public float maximumX = 2;
	
	public float minimumY = -0.7F;
	public float maximumY = 0.7F;
	
    float rotationX = 0F;
    float rotationY = 0F;

    private float deltaX;
    private float deltaY;

	public Vector2 mousePosStart;
	public Vector2 mousePosEnd;

	public bool drag;

	private Vector3 cameraOrigin;

	void Update ()
	{
		if(!GameController.objectDrag){ //Disables camera when an game object is being dragged

            if (Input.touches.Length == 1) { //Move camera
                RotateCamera ();
            }
            else if(Input.touches.Length == 2){ //Pinch zoom-in and zoom-out / pan
                ZoomPan ();

			}
			else{ //Drag ends
				drag = false;
			}

		}
	}

	void RotateCamera ()
	{
		if (!drag) {
			mousePosStart = Input.touches [0].position;
			drag = true;
		}
		if (drag && Input.touches [0].phase == TouchPhase.Moved) {
			mousePosEnd = Input.touches [0].position;
			//Delta-s
			deltaX = mousePosEnd.x - mousePosStart.x;
			deltaY = mousePosEnd.y - mousePosStart.y;
			//Get rotation from Mouse Position
			rotationX = deltaX / 130f;
			rotationY = deltaY;
			//Clamp the rotation values
			rotationX = Mathf.Clamp (rotationX, minimumX, maximumX);
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			//Rotate the camera around the center point
			if (Mathf.Abs (deltaX) > Mathf.Abs (deltaY)) {
				transform.RotateAround (cameraOrigin, Vector3.up, rotationX);
			}
			else {
				transform.RotateAround (cameraOrigin, Vector3.right, -rotationY);
			}
			//Restrict the min and max value along the y-axis
			if (transform.localEulerAngles.x < 15) {
				transform.RotateAround (cameraOrigin, Vector3.right, 15 - transform.localEulerAngles.x);
			}
			if (transform.localEulerAngles.x > 45) {
				transform.RotateAround (cameraOrigin, Vector3.right, 45 - transform.localEulerAngles.x);
			}
			//Look at the center point
			transform.LookAt (cameraOrigin);
		}
	}

	void ZoomPan ()
	{
		Touch touchZero = Input.GetTouch (0);
		Touch touchOne = Input.GetTouch (1);
		Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
		Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
		Vector3 currentPosition = Camera.main.transform.position;


		//Determine dominate axis
		if (Mathf.Abs (touchZero.deltaPosition.x) + Mathf.Abs (touchOne.deltaPosition.x) > Mathf.Abs (touchZero.deltaPosition.y) + Mathf.Abs (touchOne.deltaPosition.y)) {
			if (Mathf.Sign (touchZero.deltaPosition.x) == Mathf.Sign (touchOne.deltaPosition.x)) {
				//panning
				if (Mathf.Abs (touchZero.deltaPosition.x) > 3) {
					//horizontal
					Camera.main.transform.position = new Vector3 (currentPosition.x + 0.1f * Mathf.Sign (touchZero.deltaPosition.x), currentPosition.y, currentPosition.z - 0.1f * Mathf.Sign (touchZero.deltaPosition.x));
					cameraOrigin.x += 0.1f * Mathf.Sign (touchZero.deltaPosition.x);
					cameraOrigin.z -= 0.1f * Mathf.Sign (touchZero.deltaPosition.x);
				}
			}
			else {
				if (Mathf.Abs (touchZero.deltaPosition.x) > 1) {
					float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
					float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
					float deltaMagnitudediff = prevTouchDeltaMag - touchDeltaMag;
					Camera.main.orthographicSize += deltaMagnitudediff * orthoZoomSpeed;
					if (Camera.main.orthographicSize <= 2) {
						Camera.main.orthographicSize = 2;
					}
					Camera.main.orthographicSize = Mathf.Max (Camera.main.orthographicSize, 0.1f);
				}
			}
		}
		else {
			if (Mathf.Sign (touchZero.deltaPosition.y) == Mathf.Sign (touchOne.deltaPosition.y)) {
				//panning
				if (Mathf.Abs (touchZero.deltaPosition.y) > 3) {
					//vertical
					Camera.main.transform.position = new Vector3 (currentPosition.x, currentPosition.y + 0.1f * Mathf.Sign (touchZero.deltaPosition.y), currentPosition.z);
					cameraOrigin.z += 0.1f * Mathf.Sign (touchZero.deltaPosition.y);
				}
			}
			else {
				if (Mathf.Abs (touchZero.deltaPosition.y) > 1) {
					float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
					float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
					float deltaMagnitudediff = prevTouchDeltaMag - touchDeltaMag;
					Camera.main.orthographicSize += deltaMagnitudediff * orthoZoomSpeed;
					if (Camera.main.orthographicSize <= 2) {
						Camera.main.orthographicSize = 2;
					}
					Camera.main.orthographicSize = Mathf.Max (Camera.main.orthographicSize, 0.1f);
				}
			}
		}
	}
	
	void Start ()
	{
		drag = false;
		cameraOrigin = new Vector3 (0f, 0f, 0f);
	}
}