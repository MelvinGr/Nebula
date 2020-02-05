using UnityEngine;
using System.Collections;

public class PlaySoundSolo : MonoBehaviour 
{
	void Start() 
	{
		PlaySoundSolo[] players = (PlaySoundSolo[])FindObjectsOfType(typeof(PlaySoundSolo));
		if(players.Length > 1)
		{
			foreach(PlaySoundSolo player in players)
			{
				if(player.transform.name == transform.name)
				{
					Destroy(gameObject);
					return;
				}
			}
		}
		
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}
}