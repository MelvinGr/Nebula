using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadLevel : MonoBehaviour
{
	public GUITexture loadingGUITexture;
	public float loadingGUITextureMaxWidth;

	GameObject sound;
	GameObject loadingText;
	
	/*object async;

	void Start()
	{		
	  var gameManager = gameObject.FindObjectOfType(GameManager);
	  async = Application.LoadLevelAsync(gameManager.sceneToLoad);
		
		yield async;
				
	  Debug.Log("Loading complete");
	}*/

	float loadPrecentage = 0;
	void Awake() 
	{
		InvokeRepeating("UpdateLoading", 0, 0.002f);
		sound = GameObject.Find("Sound");
		loadingText = GameObject.Find("Elements/LoadingPrecentage");
	}
	
	void UpdateLoading()
	{	
		if(loadPrecentage < 100)
			loadPrecentage++;
		else
		{
			if(sound != null)
				Destroy(sound);
				
			Application.LoadLevel(GameManager.instance.sceneToLoad != null ? GameManager.instance.sceneToLoad : "level");
		}
	}

	void Update()
	{
		loadingGUITexture.pixelInset = new Rect
		(
			loadingGUITexture.pixelInset.x, 
			loadingGUITexture.pixelInset.y, 
			loadingGUITextureMaxWidth / 100f * loadPrecentage, 
			loadingGUITexture.pixelInset.height
		);
	}
	
	void OnGUI()
	{
		loadingText.guiText.text = "Loading.. " + (int)loadPrecentage + "%";
	}
}