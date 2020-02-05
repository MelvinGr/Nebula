using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
	
public class Achievement
{
	public int maxValue { get; private set; }		
	
	private int _value;
	public int value
	{ 
		get { return _value; }
		set
		{
			if(value <= maxValue)
				_value = value;			
			else if(OnAchievement != null && _value == maxValue)
			{
				OnAchievement(this);
				OnAchievement = null;
			}
		}
	}
	
	public bool IsComplete
	{
		get { return value >= maxValue; }
	}
	
	public override string ToString() 
	{ 
		return achievementType + " (" + value +  "/" + maxValue + ")";
	}
	
	public AchievementTypes achievementType { get; private set; }	
	public OnAchievementsDelegate OnAchievement;
	
	public Achievement(AchievementTypes _achievementType, int _maxValue, int __value = 0, OnAchievementsDelegate callback = null)
	{
		achievementType = _achievementType;
		maxValue = _maxValue;
		value = __value;
		OnAchievement = callback;
	}
}
	
[Serializable]
public class AchievementSave
{	
	public int scrapSlayer = 0;
	public int masterOfTheSkies = 0;
	public List<PickUpType> fullyEquipped = new List<PickUpType>();
	//public int protonBlaster = 0;
	public int downToEarth = 0;
	public int dieHard = 0;
	public int invincible = 0;
	//public int overAchiever = 0;		
}