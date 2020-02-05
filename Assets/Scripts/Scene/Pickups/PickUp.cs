using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[Serializable]
public class PickUpSpawn
{
	public string name;
	public Vector3 startPosition;
}

public class PickUp : LivingObject 
{	
	public OnLivingObjectDelegate OnPickUp = null;
	public OnLivingObjectDelegate OnPickUpFailed = null;
	
	public PickUpType pickUpType;	
	
	public AudioClip pickupSound;

	void Start() 
	{
		if(GameManager.level != null)
			transform.position = new Vector3(transform.position.x, GameManager.level.layerHeight, transform.position.z);
			
		transform.rotation = Quaternion.Euler(45, 0, 45);
	}

	void Update() 
	{
		if(pickUpType == PickUpType.Health && GameManager.player.health >= 3 && transform.position.x <= 100)
			transform.position += new Vector3(100, 0, 0);
		
        transform.Rotate(Vector3.right * Time.deltaTime * 100);
	}
	
	public void OnTriggerEnter(Collider otherObject)
	{
		if(otherObject.tag == "Player")
		{					
			if(OnPickUp != null)
				OnPickUp(this);
			
			Die(otherObject.gameObject.GetComponent<Player>());
		}
		
		if(otherObject.tag == "Boss")
			Die(null);
	}
	
	public override void Die(LivingObject sender)
	{		
		//Destroy(gameObject);
		gameObject.SetActive(false);
		
		if(sender == null)
		{
			if(OnPickUpFailed != null)
				OnPickUpFailed(this);
		}
		else
		{
			//Instantiate(explosion, transform.position, transform.rotation);
			Functions.PlayAudioClip(transform.position, pickupSound);
		}
	}
}