using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public class BaseTimer : MonoBehaviour
{
	public delegate void SubTimerCallback();
			
	private class SubTimer
	{
		public String name;
		
		float originalTime;
		public float time;
		
		public SubTimerCallback callBack;
		
		public bool shouldDestory;
		public bool repeat;
		public bool atStart;
		
		public SubTimer(String _name, float _time, bool _repeat, bool _atStart, SubTimerCallback _callBack)
		{
			name = _name;		
			time = _time;
			originalTime = _time;		
			repeat = _repeat;			
			atStart = _atStart;		
			callBack = _callBack;
			
			if(atStart && callBack != null)
				callBack();	
		}

		public void Update(float diff)
		{
			if(shouldDestory)
				return;
				
			time -= diff;		
			if(time <= 0)
			{			
				if(callBack != null)
					callBack();
					
				if(repeat)
					time = originalTime;
				else					
					shouldDestory = true;
			}
		}
		
		public override String ToString()
		{
			return name + "(" + String.Format("{0:0.00}", originalTime - time) + "/" + originalTime + ")";
		}
	}
	
	Dictionary<String, SubTimer> timerDict = new Dictionary<String, SubTimer>();	
	
	String guiString;
	
	/*object SyncRoot
	{
		get { return ((ICollection)timerDict).SyncRoot; }
	}*/
	
	/*private static BaseTimer _instance = null;
    public static BaseTimer instance 
	{
        get
		{
            if (_instance == null)
                _instance = (BaseTimer)FindObjectOfType(typeof(BaseTimer));
    
            if (_instance == null)
            {
				GameObject objectToAdd = null;				
				//if(FindObjectOfType(typeof(GameManager)) != null)
				//	objectToAdd = ((GameManager)FindObjectOfType(typeof(GameManager))).gameObject;
				//else
					objectToAdd = new GameObject("BaseTimer");
			
				_instance = objectToAdd.AddComponent<BaseTimer>();
			}
			
            return _instance;
        }
    }*/
	
	public bool Add(String _name, float _time, bool _repeat, bool _atStart, SubTimerCallback _callBack)
	{
		if(!timerDict.ContainsKey(_name))
		{
			timerDict.Add(_name, new SubTimer(_name, _time, _repeat, _atStart, _callBack));
			return true;
		}
		
		return false;
	}
	
	public bool Remove(String _name)
	{
		if(timerDict.ContainsKey(_name))
			return timerDict.Remove(_name);
		else
			return false;
	}

	public int RemoveContaining(String _name)
	{		
		int removedCount = 0;
		List<String> toRemove = new List<String>();
		foreach(KeyValuePair<String, SubTimer> keyValue in timerDict)
		{
			if(keyValue.Key.Contains(_name))
			{
				toRemove.Add(keyValue.Key);
				removedCount++;
			}
		}
		
		foreach(string key in toRemove)
			timerDict.Remove(key);
		
		return removedCount;
	}
	
	public bool Contains(String _name)
	{
		return timerDict.ContainsKey(_name);
	}

	void Update()
	{
		if(Time.timeScale <= 0)
			return;
		
		try
		{
			Dictionary<String, SubTimer> timerDictCopy = new Dictionary<String, SubTimer>(timerDict); // voor sync issues, moet iets beters voor zijn...

			List<String> toRemove = new List<String>();
			
			guiString = "BaseTimer timers:\n";
			foreach(KeyValuePair<String, SubTimer> keyValue in timerDictCopy)
			{				
				guiString += keyValue.Value + "\n";	
				
				if(!keyValue.Value.shouldDestory)
					keyValue.Value.Update(Time.deltaTime);		
				else
					toRemove.Add(keyValue.Key);
			}
			
			foreach(String key in toRemove)
				timerDict.Remove(key);
		}
		catch(Exception e)
		{
			Debug.Log(e);
		}
	}
	
	void OnGUI()
	{
		GUIText guiText = (GameObject.Find("GUI/Debug/Timert_Text") ? GameObject.Find("GUI/Debug/Timert_Text").guiText : null);		
		if(guiText)
			guiText.text = guiString;
	}
}
