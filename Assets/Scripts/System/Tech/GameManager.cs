using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

public class GameManager : MonoBehaviour
{	
	[HideInInspector]
	public string sceneToLoad;
	
	int _levelID = 0;
	public int levelID
	{
		get { return _levelID; }
		set
		{
			if(value <= maxLevelID)
				_levelID = value;
		}
	}

	public const int maxLevelID = 3;	
	
	//public int highScore = 0;
	
	[HideInInspector]
	public int playerScore = 0;
	
	[HideInInspector]
	public bool playerDied = false;
	
	[HideInInspector]
	public bool pickedUpPowerUp = false;
	
	[HideInInspector]
	public bool lostLife = false;

	public Transform[] blockPrefabs;
	public Transform[] bossPrefabs;
	public Transform[] wavePrefabs;
	//public Transform[] ememyPrefabs;
	public Transform[] pickUpPrefabs;
	
	public GlobalConfig config;
	
	public void OnAchievementsDelegate(Achievement achievement)
	{
		GameManager.level.guiScript.SetNewAchievementEnabled(true, Functions.SplitCamelCase(achievement.ToString()));
		/*GameManager.timer.Add("GameManager_NewAchievement_" + Time.time, 5, false, false, 
			delegate()
			{
				GameManager.level.guiScript.SetNewAchievementEnabled(false);
			}
		);*/
	}

	void Awake()
	{
		if(FindObjectsOfType(typeof(GameManager)).Length > 1)
			Destroy(gameObject);
			
		config = Functions.DeserializeFromXML<GlobalConfig>(PlayerPrefs.GetString("globalConfig"));
		if(config == null)
			config = new GlobalConfig();
			
		_baseTimer = gameObject.AddComponent<BaseTimer>();
		
		config.LoadAchievement();
	}

	//[HideInInspector]
	[HideInInspector]
	public BaseTimer _baseTimer;	
	public static BaseTimer timer
	{
		get 
		{ 
			//if(!instance._baseTimer)
			//	instance._baseTimer = instance.gameObject.AddComponent<BaseTimer>();
			
			return instance._baseTimer;
		}
	}
	
	[HideInInspector]
	public Level _level;	
	public static Level level
	{
		get { return instance._level; }
		set { instance._level = value; }
	}

	[HideInInspector]
	public Player _player;	
	public static Player player
	{
		get { return instance._player; }
		set { instance._player = value; }
	}
	
	private static GameManager _instance = null;
    public static GameManager instance 
	{
        get
		{
            if (_instance == null)
                _instance = (GameManager)FindObjectOfType(typeof(GameManager));
            
            if (_instance == null)
				throw new System.ApplicationException("No GameManager found!");
			
            return _instance;
        }
    }
	
	void Update()
	{
		if(playerScore > config.highScore)
			config.highScore = playerScore;
	}

    void OnApplicationQuit() 
	{			
		config.SaveAchievement();		
		PlayerPrefs.SetString("globalConfig", Functions.SerializeToXML(config));
		
        _instance = null;		
    }
	
	public void DisplayLoadScreen(string levelName)
	{
		sceneToLoad = levelName;
		Application.LoadLevel("LoadLevel");
	}
}