using UnityEngine;

//[ExecuteInEditMode]
public class Button : MonoBehaviour
{	
	bool clicked = false;
	static string clickedName = "";
	CameraFade cam;
	GUIScript guiScript;
	
	public AudioClip rolloverAudio;
	public AudioClip clickAudio;	
	
	//public bool addBrackets = true;
	string originalText;
	
	void Start()
	{
		cam = Camera.main.GetComponent<CameraFade>();
		cam.OnFadeOutCompleted += FadeOutCompleted;		
		
		if(!guiText.text.StartsWith("["))
			guiText.text = "[" + guiText.text;
		if(!guiText.text.EndsWith("]"))
			guiText.text = guiText.text + "]";
		
		guiText.text = guiText.text.ToUpper();
		
		originalText = guiText.text;
		
		guiScript = Functions.GetComponentInGameObject<GUIScript>("GUI");		
		
		gameObject.AddComponent<AudioSource>();	
		gameObject.AddComponent<AdjustSoundEffectLevel>();
		
		clicked = false;
	}

	/*void OnUpdate()
	{
		foreach(Touch t in Input.touches)
		{
		   if(t.phase == TouchPhase.Began)
		   {
				Vector3 point = new Vector3(t.position.x, t.position.y, 0);
				Ray ray = Camera.main.ViewportPointToRay(point);
				RaycastHit hit;
				
				if(Physics.Raycast(ray, out hit))
				{
					hit.collider.SendMessage("OnMouseDown", null, SendMessageOptions.DontRequireReceiver);
				}
		   }
		}
	}*/
	
	void FadeOutCompleted()
	{		
		if(clickedName == "QuitConfirm" || clickedName == "Menu")
			Application.LoadLevel("MainMenu");
		else if(clickedName == "Retry")
			GameManager.instance.DisplayLoadScreen("Level");
			
		clickedName = "";
	}
	
	void OnMouseEnter()
	{
		if(clicked)
			return;
			
		guiText.material.color = Color.red;
		
		//if(addBrackets)
			//guiText.text = "[" + originalText + "]";
		
		audio.PlayOneShot(rolloverAudio);
	}

	void OnMouseExit()
	{
		clicked = false;
			
		//if(addBrackets)
			//guiText.text = originalText;
			
		guiText.material.color = Color.white;
	}

	void OnMouseDown()
	{	
		if(clicked)
			return;
		
		audio.PlayOneShot(clickAudio);
		
		clickedName = transform.name;
		switch(transform.name)
		{
			case "Resume":
			{
				guiScript.TogglePauseMenu();
				
				break;
			}
			case "QuitCancel":
			{
				guiScript.SetQuitMenuEnabled(false);
				
				break;
			}
			case "QuitConfirm":
			{
				Time.timeScale = 1;
				cam.FadeOut();
				
				GameManager.instance.levelID = 0;
				
				break;
			}
			case "InGameQuit":
			{
				guiScript.SetQuitMenuEnabled(true);
				
				break;
			}
			case "Quit":
			{			
				guiScript.TogglePauseMenu();
				
				break;
			}
			case "Menu":
			{		
				GameManager.instance.levelID = 0;
				Application.LoadLevel("MainMenu");
				
				break;
			}
			
			case "Continue":
			{
				GameManager.instance.levelID = 0;
				Application.LoadLevel("MainMenu"); 
				
				break;
			}
			
			case "Retry":
			{						
				GameManager.instance.levelID = 0;
				cam.FadeOut();
				
				break;
			}
			
			case "Submit":
			{						
				Application.LoadLevel("HighScores");
				
				break;
			}
			
			case "Skip":
			{						
				Application.LoadLevel("MainMenu");
				
				break;
			}
		}
		
		clicked = true;
	}
}