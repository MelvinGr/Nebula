using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

public class Boss0 : LivingObject
{
	public Transform bossBullet;
	public Transform bossLaser;

	public AudioClip shootSound;
	public AudioClip exposionSound;
	public AudioClip collisionSound;
	
	int followSequenceIndex = 0;
	
	int sequenceIndex = 0;
	int[] currentGunIndex = { 0, 0 };
	
	BezierCurveManager bezierCurveManager;
	
	public bool shouldFollowPlayerHeight;
	
	public float bossMoveSpeed = 60;
	public float smooth = 5.0f;
		
	[HideInInspector]
	public bool isTransitioning = false;	
	[HideInInspector]
	public bool isRotatingBack = false;
	
	[HideInInspector] 
	public List<Transform> gunTransforms = new List<Transform>();
	
	[HideInInspector]
	public List<Transform> laserTransforms = new List<Transform>();
	
	public int goingDownEffectsPrecentage = 20;
	GameObject goingDownEffects;
	
	bool PlayerIsInRange()
	{
		return (transform.position.z - GameManager.player.sceneCamera.Constrains.yMin) <= 10;
	}
	
	public override void Awake()
	{
		base.Awake();
		
		goingDownEffects = Functions.FindChildTransformsRecruisive(transform, "DownEffects", true)[0].gameObject;
		goingDownEffects.SetActive(false);
		
		bezierCurveManager = Functions.FindChildTransformsRecruisive(transform, "BezierCurve_Boss0", true)[0].GetComponent<BezierCurveManager>();
		
		gunTransforms = Functions.FindChildTransformsRecruisive(transform, "gun_ref", true);	
		laserTransforms = Functions.FindChildTransformsRecruisive(transform, "laser_ref", true);
	}
	
	void Start()
	{
		if(GameManager.level != null)
			transform.position = new Vector3(transform.position.x, GameManager.level.layerHeight, transform.position.z);
		
		bezierCurveManager.transform.parent = null;
		bezierCurveManager.transform.position = transform.position;//new Vector3(61, transform.position.y, transform.position.z);
	}
	
	void SequenceSwitcher()
	{	
		sequenceIndex = sequenceIndex % 3;
		
		switch(sequenceIndex)
		{
			case 0: 
			{ 
				GameManager.timer.Remove(RandomName + "_Sequence2");
				GameManager.timer.Add(RandomName + "_Sequence0", 0.5f, true, true, Sequence0);
				
				break; 
			}
			case 1:
			{ 
				GameManager.timer.Remove(RandomName + "_Sequence0");
				GameManager.timer.Add(RandomName + "_Sequence1", 1.5f, true, true, Sequence1);
				
				break; 
			}
			case 2:
			{ 
				GameManager.timer.Remove(RandomName + "_Sequence1");
				GameManager.timer.Add(RandomName + "_Sequence2", 2f, true, true, Sequence2);
				
				break; 
			}
		}
		
		sequenceIndex++;
	}
	
	void Sequence0()
	{	
		followSequenceIndex = 0;
		
		currentGunIndex[0] = currentGunIndex[0] % 3;	
		
		int firstIndex = 0;
		int secondIndex = 0;
		
		switch(currentGunIndex[0])
		{	
			case 0: 
			{
				firstIndex = 2;
				secondIndex = 3;
				
				break;
			}
			
			case 1: 
			{
				firstIndex = 1;
				secondIndex = 4;
				
				break;
			}
			
			case 2: 
			{
				firstIndex = 0;
				secondIndex = 5;
				
				break;
			}
		}
		
		currentGunIndex[0]++;
		
		for(int i = 0; i < 2; i++)
		{						
			Transform inst = (Transform)Instantiate(bossBullet, gunTransforms[i == 0 ? firstIndex : secondIndex].position, Quaternion.identity);
			inst.transform.forward = -transform.forward;

			Bullet rocket = inst.GetComponent<Bullet>(); 
			rocket.originObject = transform;
			rocket.projectileColor = Color.gray;
			//rocket.target = player.transform
		}
			
		Functions.PlayAudioClip(transform.position, shootSound);
	}
	
	void Sequence1()
	{
		followSequenceIndex = 1;
		
		GameManager.timer.Add(RandomName + "_SubSequence1-" + Time.time + "_0", 0.0f, false, true, SubSequence1);
		GameManager.timer.Add(RandomName + "_SubSequence1-" + Time.time + "_1", 0.1f, false, false, SubSequence1);
		GameManager.timer.Add(RandomName + "_SubSequence1-" + Time.time + "_2", 0.2f, false, false, SubSequence1);
	}
	
	void SubSequence1()
	{
		for(int i = 0; i < gunTransforms.Count; i++)
		{			
			Transform inst = (Transform)Instantiate(bossBullet, gunTransforms[i].position, Quaternion.identity);
			inst.transform.forward = -transform.forward;

			Bullet rocket = inst.GetComponent<Bullet>(); 
			rocket.originObject = transform;
			rocket.bulletSpeed = 10;
			rocket.projectileColor = Color.gray;
			//rocket.target = player.transform
		}
			
		Functions.PlayAudioClip(transform.position, shootSound);
	}
	
	void Sequence2()
	{
		followSequenceIndex = 0;
		
		for(int i = 0; i < laserTransforms.Count; i++)
		{
			Transform inst = (Transform)Instantiate(bossLaser, laserTransforms[i].position, Quaternion.identity);
			inst.transform.forward = -transform.forward;			
	
			Laser laser = inst.GetComponent<Laser>(); 
			laser.originObject = transform;
			laser.bulletSpeed = 10;
			laser.maxScale = 0.5f;
		}
		
		Functions.PlayAudioClip(transform.position, shootSound);
	}
	
	void OnTriggerEnter(Collider otherObject)
	{
		if(otherObject.tag == "Block")
			Die(GameManager.player);
		else if(otherObject.tag == "Projectile")
		{
			Projectile projectile = otherObject.GetComponent<Projectile>();
			if(projectile != null)
			{
				if(projectile.originObject.GetComponent<Player>() != null)
				{
					health -= 1;//Functions.GetBulletDamage(this, bullet);				
					hitIndicator.hitIndicatorRemaining = 0.2f;
				
					Functions.PlayAudioClip(transform.position, collisionSound);
					projectile.Die();
				}
			}
		}
	}

	public override void Die(LivingObject sender)
	{	
		base.Die(sender);
		
		if(sender != null && sender.GetType() == typeof(Player))
		{
			GameManager.instance.playerScore += 5000;
		}
		
		if(GameManager.instance.config.gameOptions.effectEnabled)
			Instantiate(explosion, transform.position, transform.rotation);
		
		Functions.PlayAudioClip(transform.position, exposionSound);
		
		Destroy(gameObject);
		Destroy(bezierCurveManager.gameObject);
	}
	
	void OnDestroy() 
	{
		GameManager.timer.RemoveContaining(RandomName);
	}

	void Update()
	{		
		if(GameManager.player == null || Time.timeScale <= 0)
			return;			
			
		if(transform.rigidbody.useGravity)
		{
			float t = Mathf.Sin(Time.time * 0.5f);			
			transform.rotation = Quaternion.Euler(t * -45, t * 360, 0);
		}
			
		if(health <= 0)
		{
			GameManager.timer.RemoveContaining(RandomName);
			transform.rigidbody.isKinematic = false;
			transform.rigidbody.useGravity = true;
			return;
		}

		if(PlayerIsInRange())
		{
			GameManager.player.config.onRails = false;			
			GameManager.player.sceneCamera.config.followPlayer = false;	

			GameManager.timer.Add(RandomName + "_SequenceSwitcher", 5f, true, true, SequenceSwitcher);			
		}
		else
			GameManager.timer.Remove(RandomName + "_SequenceSwitcher");

		if(health <= (spawnHealth / 100 * goingDownEffectsPrecentage))
			goingDownEffects.SetActive(true);
			
		followSequenceIndex = 0;
		if(followSequenceIndex == 0 && bezierCurveManager != null)
		{
			transform.position = Vector3.Lerp(transform.position, bezierCurveManager.GetPositionAtTime(Time.time), Time.deltaTime * smooth);
		}
		else if(followSequenceIndex == 1)
		{
			Vector3 simulatedPos = new Vector3(GameManager.player.transform.position.x, transform.position.y, transform.position.z);
			transform.position = Vector3.Lerp(transform.position, simulatedPos, Time.deltaTime * (smooth / 2));
		}
			
		if(health <= (spawnHealth / 100 * goingDownEffectsPrecentage))
		{
			if(GameManager.timer.Remove(RandomName + "_SequenceSwitcher"))
			{
				followSequenceIndex = 1;
				
				GameManager.timer.Remove(RandomName + "_Sequence0");
				GameManager.timer.Remove(RandomName + "_Sequence1");
				GameManager.timer.Remove(RandomName + "_Sequence2");
				
				GameManager.timer.Add(RandomName + "_Sequence0", 0.4f, true, false, Sequence0);
				GameManager.timer.Add(RandomName + "_Sequence2", 0.8f, true, false, Sequence2);
			}
		}
	}
}