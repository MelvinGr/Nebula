using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode]
public class TranslateChildGUITexts : MonoBehaviour
{
	void Start() 
	{		
		//WriteDefaultValues();
		//return;
		
		if(!GameManager.instance)
			return;
			
		if(GameManager.instance.config.gameOptions.language == LanguageEnum.COM)
			return;
		
		Dictionary<string, string> translatedDict = Functions.FileToDictionary("Translations/" +
			GameManager.instance.config.gameOptions.language + "/" + Application.loadedLevelName);
		
		GUIText[] guiTexts = gameObject.GetComponentsInChildren<GUIText>();		
		foreach(GUIText guiText in guiTexts)
		{			
			if(translatedDict.ContainsKey(guiText.name))
				guiText.text = translatedDict[guiText.name].Replace("\\n", "\n");				
		}
	}
	
	void WriteDefaultValues()
	{
		string dir = "C:/Translations/" + GameManager.instance.config.gameOptions.language;
		string txtFile = dir + "/" + Application.loadedLevelName + ".txt";
		
		Directory.CreateDirectory(dir);
		
		StreamWriter sw = new StreamWriter(txtFile, false);  
		sw.WriteLine("// " + Application.loadedLevelName);
		
		List<string> usedKeys = new List<string>();
		GUIText[] guiTexts = gameObject.GetComponentsInChildren<GUIText>();			
		foreach(GUIText guiText in guiTexts)
		{
			if(!usedKeys.Contains(guiText.name))
				sw.WriteLine(guiText.name + "=" + guiText.text);
				
			usedKeys.Add(guiText.name);
		}
		
		sw.Close();
	}
}
