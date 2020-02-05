using UnityEngine;

//[ExecuteInEditMode]
public class LoadButton : MonoBehaviour
{
	public string levelName = "";
	public int levelID = 0;
	
	public bool loadDirect = true;
	public bool dontFade = false;
	
	static bool clicked = false;
	
	public AudioClip rolloverAudio;
	public AudioClip clickAudio;
	
	//public bool addBrackets = true;
	string originalText;
	
	void Awake()
	{
		clicked = false;
		
		if(!guiText.text.StartsWith("["))
			guiText.text = "[" + guiText.text;
		if(!guiText.text.EndsWith("]"))
			guiText.text = guiText.text + "]";
		
		guiText.text = guiText.text.ToUpper();
		
		originalText = guiText.text;
		
		gameObject.AddComponent<AudioSource>();	
		gameObject.AddComponent<AdjustSoundEffectLevel>();
	}
	
	/*void Update()
	{
		foreach(Touch touch in Input.touches)
		{
		   if(touch.phase == TouchPhase.Began)
		   {
				GameManager.instance.DisplayLoadScreen("Level");
		   }
		}
	}*/
	
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
		if(clicked)
			return;
			
		//if(addBrackets)
			//guiText.text = originalText;
			
		guiText.material.color = Color.white;
	}

	void OnMouseDown()
	{	
		if(clicked)
			return;
		
		audio.PlayOneShot(clickAudio);
		
		if(dontFade)
		{
			FadeOutCompleted();
		}
		else
		{
			CameraFade cam = Camera.main.GetComponent<CameraFade>();
			cam.OnFadeOutCompleted += FadeOutCompleted;		
			cam.FadeOut();
		}
		
		clicked = true;
	}

	void FadeOutCompleted()
	{				
		GameManager.instance.levelID = levelID;
		
		if(loadDirect)
			Application.LoadLevel(levelName);
		else
			GameManager.instance.DisplayLoadScreen(levelName);
	}
}