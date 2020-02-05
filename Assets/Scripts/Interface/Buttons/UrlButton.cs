using UnityEngine;
using System.Collections;

public class UrlButton : MonoBehaviour 
{
	public string urlToOpen;
	
	public AudioClip rolloverAudio;
	public AudioClip clickAudio;
	
	public Texture rollOverMouseTexture;
	
	public bool shouldHideCursor = true;
	
	//public bool changeColor = true;
	
	static bool clicked = false;
	
	void Awake()
	{	
		clicked = false;
		
		gameObject.AddComponent<AudioSource>();	
		gameObject.AddComponent<AdjustSoundEffectLevel>();
	}
	
	void OnGUI()
	{
		if(rollOverMouseTexture == null)
			return;
			
		Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);		
		Rect rect = guiTexture.GetScreenRect();				
		if(Functions.RectangleContains(guiTexture.GetScreenRect(), mousePos))
		{
			GUI.DrawTexture(new Rect(mousePos.x - rollOverMouseTexture.width / 2, Screen.height - mousePos.y, rollOverMouseTexture.width, rollOverMouseTexture.height), 
				rollOverMouseTexture, ScaleMode.ScaleToFit, true);
		}
	}
	
	void OnMouseEnter()
	{
		if(shouldHideCursor)
			Screen.showCursor = false;
				
		if(clicked)
			return;
			
		audio.PlayOneShot(rolloverAudio);
	}
	
	void OnMouseExit()
	{
		if(shouldHideCursor)
			Screen.showCursor = true;
	}
	
	void OnMouseDown()
	{
		if(clicked)
			return;

		if(urlToOpen != null)
		{
			if(!urlToOpen.StartsWith("http://"))
				urlToOpen = "http://" + urlToOpen;
				
			audio.PlayOneShot(clickAudio);
			//Application.OpenURL(urlToOpen);
			Application.ExternalEval("window.open('" + urlToOpen + "')");
		}
		
		clicked = true;
	}
}
