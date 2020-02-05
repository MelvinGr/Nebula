using UnityEngine;

public class Splash : MonoBehaviour
{
	private CameraFade cam;
	private int imageIndex = 0;
	
	public Texture2D[] backgrounds = null;

	void Awake()
	{
		cam = (CameraFade)Camera.main.GetComponent("CameraFade");
		cam.OnFadeInCompleted = cam.FadeOut;//OnFadeInCompleted;
		cam.OnFadeOutCompleted = OnFadeOutCompleted;
	}

	void Start()
	{	
		OnFadeOutCompleted();
	}

	void OnFadeOutCompleted()
	{
		if(imageIndex >= backgrounds.Length)
		{
			cam.FadeIn();
			Application.LoadLevel("MainMenu");			
			return;
		}
		
		gameObject.GetComponent<UrlButton>().urlToOpen = (imageIndex == 0 ? "www.basegames.nl" : "");//"www.xformgames.com");
			
		GetComponent<GUITexture>().texture = backgrounds[imageIndex];
		imageIndex++;
		
		cam.FadeIn();
	}
}