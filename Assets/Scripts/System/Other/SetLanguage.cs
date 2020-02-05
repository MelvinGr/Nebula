using UnityEngine;
using System.Collections;

public class SetLanguage : MonoBehaviour 
{
	void Awake() 
	{	
		string domain = (Application.absoluteURL != "" ? new System.Uri(Application.absoluteURL).Host.Split('.')[1] : "");
		
		try
		{
			GameManager.instance.config.gameOptions.language = (LanguageEnum)System.Enum.Parse(typeof(LanguageEnum), domain, true);
		}
		catch
		{
			GameManager.instance.config.gameOptions.language = LanguageEnum.COM;
		}
	}
}