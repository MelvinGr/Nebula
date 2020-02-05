using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class CameraConfig
{
	public Vector3 rotation;
	public float heightOffset = 6;
	public float cameraSpeed = 3;
	public bool followPlayer = true;
}

public class SceneCamera : MonoBehaviour
{	
	public CameraConfig config;
	
	public Vector3 cameraOffset;
	
	public float originalSpeed { get; private set; }
	
	//float originalGlowThreshold;
	public GlowThresholdEffect glowThresholdEffect
	{
		get { return gameObject.GetComponent<GlowThresholdEffect>(); }
	}
	
	public Rect Constrains
	{
		get
		{
			Vector3 min = GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0, 0, config.heightOffset)); // links onder
			Vector3 max = GetComponent<Camera>().ViewportToWorldPoint(new Vector3(1, 1, config.heightOffset)); // rechts boven
			return new Rect(min.x, min.z, max.x, max.z);
		}
	}

	void Start()
	{
		config = Functions.DeserializeFromResourcesXML<CameraConfig>("Configs/cameraConfig");

		originalSpeed = config.cameraSpeed;	

		//originalGlowThreshold = glowThresholdEffect.glowThreshold;
	}

	void Update()
	{
		if(GameManager.player != null)
		{
			if(GameManager.player.config.onRails)
			{
				transform.position = new Vector3
				(
					transform.position.x, 
					transform.position.y,// + config.heightOffset), 
					transform.position.z + (config.cameraSpeed * Time.deltaTime)
				) + cameraOffset;
			}
			else
			{
				transform.position = new Vector3
				(
					transform.position.x, 
					GameManager.player.transform.position.y + config.heightOffset, 
					(config.followPlayer ? GameManager.player.transform.position.z : transform.position.z)
				) + cameraOffset;
			}
		}
		
		transform.rotation = Quaternion.Euler(config.rotation);
	}
}