using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class TrophiesScreen : MonoBehaviour 
{
	public Font smallerFont;
	
	void Start() 
	{		
		foreach(Transform trans in Functions.FindChildTransformsRecruisive(transform, "-Points", true))
		{
			Achievement achievement = null;
			
			try
			{
				AchievementTypes achievementType = (AchievementTypes)Enum.Parse(typeof(AchievementTypes), trans.name.Replace("-Points", ""), true);  
				achievement = GameManager.instance.config.achievements[achievementType];
			}
			catch(Exception e)
			{
				//
			}
			
			if(achievement != null)
			{
				bool isNL = (GameManager.instance.config.gameOptions.language != LanguageEnum.COM);
				
				if(achievement.maxValue == 1)
					trans.GetComponent<GUIText>().text = (achievement.value > 0 ? (isNL ? "Ja": "Yes") : (isNL ? "Nee" : "No"));
				else
					trans.GetComponent<GUIText>().text = achievement.value + "/" + achievement.maxValue;
					
				if(achievement.value >= 100 && smallerFont != null)
					trans.GetComponent<GUIText>().font = smallerFont;
			}
		}
	}
}