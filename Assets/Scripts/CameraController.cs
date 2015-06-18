using UnityEngine;
using System.Collections;

public class CameraController: MonoBehaviour
{
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 0.0001F;
	public float sensitivityY = 0.0001F;
	
	public float minimumX = -360F;
	public float maximumX = 360F;
	
	public float minimumY = 0F;
	public float maximumY = 180F;
	
    float rotationX = 0F;
    float rotationY = 0F;

	public static float currentRotationX;
    public static float currentRotationY;

	public Vector2 mousePosStart;
	public Vector2 mousePosEnd;

	public bool drag;

	void Update ()
	{
		//For Editor
        //if(!PlayerController.objectDrag){
        //    if(Input.GetMouseButtonDown(0)){ //Drag starts
        //        mousePosStart = Input.mousePosition;
        //        drag = true;
        //    }
        //    if(Input.GetMouseButton(0) && drag){ //Dragging
        //        mousePosEnd = Input.mousePosition;
        //        rotationX = Input.GetAxis("Mouse X") * 15;
        //        rotationX = Mathf.Clamp (rotationX, minimumY, maximumY);
        //        transform.localEulerAngles = new Vector3(-rotationX, transform.localEulerAngles.y, 0);
        //        transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, rotationX);
        //        transform.LookAt(Vector3.zero);

        //        currentRotationX += rotationX;
        //        currentRotationX %= 360;
        //        //Debug.Log ("LOL " + currentRotation); 
        //    }
        //    if(Input.GetMouseButtonUp(0) && drag){ //Drag ends
        //        drag = false;
        //    }
        //}

		//For in-game
		if(!PlayerController.objectDrag){
			if(Input.touches.Length > 0 && !drag){
				mousePosStart = Input.touches[0].position;
				drag = true;
			}
			if(Input.touches.Length > 0 && drag && Input.touches[0].phase == TouchPhase.Moved){
				mousePosEnd = Input.touches[0].position;
				//print ("Moving " + mousePosEnd);
				rotationX = (mousePosEnd.x > mousePosStart.x ? 1 : -1)  * (Mathf.Abs(mousePosEnd.x - mousePosStart.x)) / 100;
                rotationY = (mousePosEnd.y > mousePosStart.y ? 1 : -1)  * (Mathf.Abs(mousePosEnd.y - mousePosStart.y)) / 100;
                
				rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                

                //transform.localEulerAngles = new Vector3(-rotationX, -rotationY, 0);
                
                transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, rotationX);

                //if (transform.localEulerAngles.x >= 15 && transform.localEulerAngles.x <= 80) {
                //    transform.RotateAround(new Vector3(0, 0, 0), Vector3.left, rotationY);
                //}
                //else {
                //    transform.RotateAround(new Vector3(0, 0, 0), Vector3.right, rotationY);
                //}


                print(rotationX);


				transform.LookAt(Vector3.zero);
				
				currentRotationX += rotationX;
				currentRotationX %= 360;

                currentRotationY += rotationY;
                currentRotationY %= 360;
			}
			if(Input.touches.Length <= 0){ //Drag ends
				drag = false;
			}

		}

//		int fingerCount = 0;
//		foreach (Touch touch in Input.touches) {
//			if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled){
//				print("User has " + touch.position);
//				fingerCount++;
//			}
//			
//		}
//		if (fingerCount > 0)
//			print("User has " + fingerCount + " finger(s) touching the screen");

	}
	
	void Start ()
	{
		drag = false;
	}
}