using UnityEngine;
using System.Collections;

public class CameraController: MonoBehaviour
{
	
	public float minimumX = -2;
	public float maximumX = 2;
	
	public float minimumY = -0.7F;
	public float maximumY = 0.7F;
	
    float rotationX = 0F;
    float rotationY = 0F;

	public static float currentRotationX;
    public static float currentRotationY;

	public Vector2 mousePosStart;
	public Vector2 mousePosEnd;

	public bool drag;

	void Update ()
	{
		if(!PlayerController.objectDrag){ //Disables camera when an game object is being dragged
            //print(PlayerController.objectDrag);
			if(Input.touches.Length > 0 && !drag){
				mousePosStart = Input.touches[0].position;
				drag = true;
			}
			if(Input.touches.Length > 0 && drag && Input.touches[0].phase == TouchPhase.Moved){
				mousePosEnd = Input.touches[0].position;

                //Get rotation from Mouse Position
                rotationX = (mousePosEnd.x - mousePosStart.x) / 130f;
                rotationY = (mousePosEnd.y - mousePosStart.y);

                //Clamp the rotation values
				rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                //Rotate the camera around the center point
                transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, rotationX);
                transform.RotateAround(new Vector3(0, 0, 0), Vector3.right, -rotationY);
        
                //Restrict the min and max value along the y-axis
                if (transform.localEulerAngles.x < 15) {
                    transform.RotateAround(new Vector3(0, 0, 0), Vector3.right, 15 - transform.localEulerAngles.x);
                }

                if (transform.localEulerAngles.x > 45) {
                    transform.RotateAround(new Vector3(0, 0, 0), Vector3.right, 45 - transform.localEulerAngles.x);
                }

                //Look at the center point

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
	}
	
	void Start ()
	{
		drag = false;
	}
}