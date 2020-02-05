using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
	
[Serializable]
public class GameOptions
{
	public int soundVolume = 3;
	public int musicVolume = 4;
	
	public bool effectEnabled = true;
		
	public LanguageEnum language = LanguageEnum.COM;
	
	public int antiAliasingLevel = 4;	
	/*public int antiAliasingLevel
	{
		get { return QualitySettings.antiAliasing; }
		set 
		{
			if((value % 2) == 0 && value <= 8)
				QualitySettings.antiAliasing = value;
		}
	}*/
	
	public void reload()
	{
		QualitySettings.antiAliasing = antiAliasingLevel;
	}
}
	
[Serializable]
public class GlobalConfig
{	
	public int highScore = 0;

	public GameOptions gameOptions = new GameOptions();
	public AchievementSave achievementSave = new AchievementSave();
	
	[XmlIgnoreAttribute]
	public Dictionary<AchievementTypes, Achievement> achievements = new Dictionary<AchievementTypes, Achievement>();
	
	public void LoadAchievement()
	{
		OnAchievementsDelegate del = GameManager.instance.OnAchievementsDelegate;
		
		int pickupCount = Enum.GetValues(typeof(PickUpType)).Length; // 7

		achievements[AchievementTypes.ScrapSlayer] = new Achievement(AchievementTypes.ScrapSlayer, 100, achievementSave.scrapSlayer, del);
		achievements[AchievementTypes.MasterOfTheSkies] = new Achievement(AchievementTypes.MasterOfTheSkies, 250, achievementSave.masterOfTheSkies, del);
		achievements[AchievementTypes.FullyEquipped] = new Achievement(AchievementTypes.FullyEquipped, pickupCount, achievementSave.fullyEquipped.Count, del);
		//achievements[AchievementTypes.ProtonBlaster] = new Achievement(AchievementTypes.ProtonBlaster, 50, achievementSave.protonBlaster, del);
		achievements[AchievementTypes.DownToEarth] = new Achievement(AchievementTypes.DownToEarth, 1, achievementSave.downToEarth, del);
		achievements[AchievementTypes.DieHard] = new Achievement(AchievementTypes.DieHard, 1, achievementSave.dieHard, del);
		achievements[AchievementTypes.Invincible] = new Achievement(AchievementTypes.Invincible, 1, achievementSave.invincible, del);			
		//achievements[AchievementTypes.OverAchiever] = new Achievement(AchievementTypes.OverAchiever, 7, achievementSave.overAchiever, del);
	}
	
	public void SaveAchievement()
	{
		achievementSave.scrapSlayer = achievements[AchievementTypes.ScrapSlayer].value;
		achievementSave.masterOfTheSkies = achievements[AchievementTypes.MasterOfTheSkies].value;
		//achievementSave.protonBlaster = achievements[AchievementTypes.ProtonBlaster].value;
		achievementSave.downToEarth = achievements[AchievementTypes.DownToEarth].value;
		achievementSave.dieHard = achievements[AchievementTypes.DieHard].value;
		achievementSave.invincible = achievements[AchievementTypes.Invincible].value;		
		//achievementSave.overAchiever = achievements[AchievementTypes.OverAchiever].value;
	}
}