using UnityEngine;
using System.Collections;

public class AdjustMusicLevel : MonoBehaviour 
{
	void Start() 
	{
		Adjust();
	}
	
	public void Adjust()
	{
		AudioSource audioSource = gameObject.GetComponent<AudioSource>();
		if(audioSource)
			audioSource.volume = (float)GameManager.instance.config.gameOptions.musicVolume * 0.2f;
	}
}