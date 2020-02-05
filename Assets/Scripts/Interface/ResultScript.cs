using UnityEngine;
using System;
using System.Collections;

public class ResultScript : MonoBehaviour
{
	GameObject pointText;
		
	void Start()
	{
		pointText = GameObject.Find("Elements/Text/PointCount");
		
		string toHide = (GameManager.instance.playerDied ? "Elements/Text/Won" : "Elements/Text/Lost");		
		if(GameObject.Find(toHide) != null)
			GameObject.Find(toHide).transform.position = Vector3.one;

		if(GameManager.instance.playerScore > GameManager.instance.config.highScore)
			GameManager.instance.config.highScore = GameManager.instance.playerScore;
	}
	
	void OnGUI()
	{					
		pointText.guiText.text = String.Format("{0:0000000}", GameManager.instance.playerScore);	
	}
}