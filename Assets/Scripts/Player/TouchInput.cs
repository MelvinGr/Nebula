using UnityEngine;
using System.Collections;

public class TouchInput : MonoBehaviour 
{		
	public Vector3 touchMovementVector(Camera camera, Touch touch) 
	{
		float zDistanceFromCamera = Vector3.Distance(camera.transform.position, gameObject.transform.position);

		Vector3 screenPosition = new Vector3(touch.position.x, touch.position.y, zDistanceFromCamera);
		Vector3 lastScreenPosition = new Vector3(touch.position.x - touch.deltaPosition.x,touch.position.y - touch.deltaPosition.y,zDistanceFromCamera);

		Vector3 cameraWorldPosition = camera.ScreenToWorldPoint(screenPosition);
		Vector3 lastCameraWorldPosition = camera.ScreenToWorldPoint(lastScreenPosition);

		return cameraWorldPosition - lastCameraWorldPosition;
	}
		
	/*public void Update()
	{	
		//Debug.Log(iPhoneInput.touchCount);
		Input.multiTouchEnabled = true;
		
		for (int i = 0; i < iPhoneInput.touchCount; i++) 
		{			
			iPhoneTouch touch = iPhoneInput.GetTouch(i);
			//Vector3 screenPosition = new Vector3(touch.position.x, touch.position.y, 0.0f);			
			
			if(i == 0)
			{
				Vector3 movement = touchMovementVector(Camera.main, touch);
				//transform.Translate(movement, Space.World);
			}
			else
			{
				GameManager.player.Shoot();
			}
		}
	}*/
	
	/*void OnGUI () {
		// Make a background box
		GUI.Box(new Rect(10,10,100,90), "Loader Menu");

		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if(GUI.Button(new Rect(20,40,80,20), "Level 1")) 
		{
			Debug.Log("LALA");
		}

		// Make the second button.
		if(GUI.Button(new Rect(20,70,80,20), "Level 2")) 
		{
			Destroy(gameObject);
		}
	}*/
	
	void Update() 
	{
		Input.multiTouchEnabled = true;
		
		for (int i = 0; i < Input.touchCount; i++) 
		{
			  Touch touch = Input.GetTouch(i);
			  Debug.Log(touch);
			  //Do whatever you want with the current touch.
		}
    }
}
