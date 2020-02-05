using UnityEngine;
using System.Collections;

public class LimitFPS : MonoBehaviour
{
	void Start() 
	{
		QualitySettings.vSyncCount = 2;
		//Application.targetFrameRate = 30;
	}
}
