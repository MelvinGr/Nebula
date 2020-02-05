using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class GUIScript : MonoBehaviour 
{	
	GameObject pauseMenu;
	GameObject pauseMenuConfirmQuit;

	GameObject newBoss;
	GameObject newBossFirst;
	GameObject newAchievement;
	
	GameObject nextStage;
	GameObject nextStageFirst;
	
	GameObject highscoreCount;
	GameObject pointText;
	GameObject lifeCount;
	GameObject newAchievementText;
	
	public AudioClip newStageSound;

	void Start()
	{						
		pauseMenu = GameObject.Find("GUI/PauseMenuScreen");		
		pauseMenuConfirmQuit = GameObject.Find("GUI/QuitConfirmScreen");
		
		newBoss = GameObject.Find("GUI/NewBoss");
		newBossFirst = GameObject.Find("GUI/NewBossFirst");
		newAchievement = GameObject.Find("GUI/NewAchievement");
		
		nextStage = GameObject.Find("GUI/NextStage");
		nextStageFirst = GameObject.Find("GUI/NextStageFirst");

		highscoreCount = GameObject.Find("GUI/Interface/HighscoreCount");
		pointText = GameObject.Find("GUI/Interface/PointsCount");
		lifeCount = GameObject.Find("GUI/Interface/LifeCount");
		newAchievementText = GameObject.Find("GUI/NewAchievement/Text");
	}

	public void TogglePauseMenu()
	{
		Time.timeScale = (Time.timeScale + 1) % 2;
		
		if(Time.timeScale == 0)
		{
			pauseMenu.transform.position = Vector3.zero;
			GameManager.player.GetComponent<AudioSource>().mute = false;
		}
		else
		{
			pauseMenu.transform.position = Vector3.one;
			pauseMenuConfirmQuit.transform.position = Vector3.one;
			GameManager.player.GetComponent<AudioSource>().mute = true;
		}
	}
	
	public void SetQuitMenuEnabled(bool enabled)
	{
		if(enabled)
		{
			pauseMenu.transform.position = Vector3.one;
			pauseMenuConfirmQuit.transform.position = Vector3.zero;
		}
		else
		{
			pauseMenu.transform.position = Vector3.zero;
			pauseMenuConfirmQuit.transform.position = Vector3.one;
		}
	}
	
	public void SetNextStageEnabled(bool enabled, bool first)
	{
		if(enabled)
		{
			Functions.PlayAudioClip(GameManager.player.transform.position, newStageSound);
			
			if(first)
				nextStageFirst.transform.position = new Vector3(0, 0, nextStageFirst.transform.position.z);
			else
				nextStage.transform.position = new Vector3(0, 0, nextStage.transform.position.z);
			
			GameManager.timer.Add("GUI_DisableNextStage-" + Time.time, 5, false, false, delegate(){ SetNextStageEnabled(false, first); });
		}
		else
		{
			if(first)
				nextStageFirst.transform.position = new Vector3(1, 0, nextStageFirst.transform.position.z);
			else
				nextStage.transform.position = new Vector3(1, 0, nextStage.transform.position.z);
		}
	}
	
	public void SetNewBossNotificationEnabled(bool enabled, bool first)
	{
		if(enabled)
		{
			if(first)
				newBossFirst.transform.position = new Vector3(0, 0, newBossFirst.transform.position.z);
			else
				newBoss.transform.position = new Vector3(0, 0, newBoss.transform.position.z);
				
			GameManager.timer.Add("GUI_DisableNewBoss-" + Time.time, 5, false, false, delegate(){ SetNewBossNotificationEnabled(false, first); });
		}
		else
		{
			if(first)
				newBossFirst.transform.position = new Vector3(1, 0, newBossFirst.transform.position.z);
			else
				newBoss.transform.position = new Vector3(1, 0, newBoss.transform.position.z);
		}
	}
	
	public void SetNewAchievementEnabled(bool enabled, string text = null)
	{
		if(enabled)
			newAchievement.transform.position = new Vector3(0, newAchievement.transform.position.y, newAchievement.transform.position.z);
		else
			newAchievement.transform.position = new Vector3(1, newAchievement.transform.position.y, newAchievement.transform.position.z);
			
		if(text != null)
			newAchievementText.GetComponent<GUIText>().text = text;
			
		if(enabled)
			GameManager.timer.Add("GUI_DisableNewAchievement-" + Time.time, 5, false, false, delegate(){ SetNewAchievementEnabled(false); });
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.P))
			TogglePauseMenu();
		else if(Input.GetKeyDown(KeyCode.M))
			AudioListener.pause = !AudioListener.pause;
	}

	void OnGUI()
	{	
		pointText.GetComponent<GUIText>().text = String.Format("{0:0000}", GameManager.instance.playerScore);		
		highscoreCount.GetComponent<GUIText>().text = String.Format("{0:0000}", GameManager.instance.config.highScore);
		lifeCount.GetComponent<GUIText>().text = "X" + String.Format("{0:00}", GameManager.player.health);
	}
}