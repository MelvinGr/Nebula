using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class OptionsScript : MonoBehaviour 
{
	GameObject soundVolumeButtons;
	GameObject musicVolumeButtons;
	GameObject effectsButtons;
	GameObject antiAliasingButtons;

	public AudioClip rolloverAudio;
	public AudioClip clickAudio;

	Dictionary<string, GameObject> selected = new Dictionary<string, GameObject>();
	
	void Awake()
	{
		gameObject.AddComponent<AudioSource>();	
		gameObject.AddComponent<AdjustSoundEffectLevel>();
	}
	
	void Start()
	{
		soundVolumeButtons = GameObject.Find("Elements/Option Buttons/SoundVolume Buttons");
		musicVolumeButtons = GameObject.Find("Elements/Option Buttons/MusicVolume Buttons");
		effectsButtons = GameObject.Find("Elements/Option Buttons/Effects Buttons");
		antiAliasingButtons = GameObject.Find("Elements/Option Buttons/AntiAliasing Buttons");
		
		selected.Add("SoundVolume Buttons", null);
		selected.Add("MusicVolume Buttons", null);
		selected.Add("Effects Buttons", null);
		selected.Add("AntiAliasing Buttons", null);

			
		foreach(Transform child in soundVolumeButtons.transform)
		{
			if(child.name == GameManager.instance.config.gameOptions.soundVolume.ToString())
			{
				child.GetComponent<GUIText>().material.color = Color.red;
				selected["SoundVolume Buttons"] = child.gameObject;
			}
		}
		
		foreach(Transform child in musicVolumeButtons.transform)
		{
			if(child.name == GameManager.instance.config.gameOptions.musicVolume.ToString())
			{
				child.GetComponent<GUIText>().material.color = Color.red;
				selected["MusicVolume Buttons"] = child.gameObject;
			}
		}
		
		foreach(Transform child in effectsButtons.transform)
		{
			if(child.name == (GameManager.instance.config.gameOptions.effectEnabled ? 1 : 0).ToString())
			{
				child.GetComponent<GUIText>().material.color = Color.red;
				selected["Effects Buttons"] = child.gameObject;
			}
		}
		
		foreach(Transform child in antiAliasingButtons.transform)
		{
			if(child.name == GameManager.instance.config.gameOptions.antiAliasingLevel.ToString())
			{
				child.GetComponent<GUIText>().material.color = Color.red;
				selected["AntiAliasing Buttons"] = child.gameObject;
			}
		}
	}
	
	void DoChildOverDetection(GameObject obj, Vector2 mousPos)
	{
		if(GetComponent<AudioSource>().isPlaying)
			return;
		
		foreach(Transform child in obj.transform)
		{								
			if(selected[obj.name] != child.gameObject)
			{
				child.GetComponent<GUIText>().material.color = Color.white;
				//child.guiText.text = child.guiText.text.Replace("[", "").Replace("]", "");
			}
				
			if(!Functions.RectangleContains(child.GetComponent<GUIText>().GetScreenRect(), mousPos))				
				continue;
		
			child.GetComponent<GUIText>().material.color = Color.red;
			
			//if(!child.guiText.text.Contains("["))
				//child.guiText.text = "[" + child.guiText.text + "]";
				
			if(Input.GetMouseButtonDown(0))
			{
				selected[obj.name] = child.gameObject;
				
				switch(obj.name)
				{
					case "SoundVolume Buttons":
					{		
						GameManager.instance.config.gameOptions.soundVolume = Int32.Parse(child.name);// * 0.20f;
						
						break;
					}
					
					case "MusicVolume Buttons":
					{
						GameManager.instance.config.gameOptions.musicVolume = Int32.Parse(child.name);// * 0.20f;
						
						break;
					}
					
					case "Effects Buttons":
					{					
						GameManager.instance.config.gameOptions.effectEnabled = (Int32.Parse(child.name) == 0 ? false : true);
						
						break;
					}
					
					case "AntiAliasing Buttons":
					{
						GameManager.instance.config.gameOptions.antiAliasingLevel = Int32.Parse(child.name);
						GameManager.instance.config.gameOptions.reload();
						
						break;
					}
					
					default:
					{
						Debug.Log(obj.name);
						
						break;
					}
				}
				
				GetComponent<AudioSource>().PlayOneShot(clickAudio);
				
				AdjustMusicLevel[] musicAdjusters = (AdjustMusicLevel[])FindObjectsOfType(typeof(AdjustMusicLevel));
				foreach(AdjustMusicLevel adjuster in musicAdjusters)
					adjuster.Adjust();
					
				AdjustSoundEffectLevel[] effectAdjusters = (AdjustSoundEffectLevel[])FindObjectsOfType(typeof(AdjustSoundEffectLevel));
				foreach(AdjustSoundEffectLevel adjuster in effectAdjusters)
					adjuster.Adjust();
			}
		}
	}
	
	void Update()
	{
		Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		
		DoChildOverDetection(soundVolumeButtons, mousePos);
		DoChildOverDetection(musicVolumeButtons, mousePos);
		DoChildOverDetection(effectsButtons, mousePos);
		DoChildOverDetection(antiAliasingButtons, mousePos);
	}
}