using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VirginMaryChild3 : LivingObject 
{
	public Transform minePrefab;
	
	public AudioClip shootSound;
	public AudioClip exposionSound;
	public AudioClip collisionSound;
	
	public Transform mineDropRef;
	
	public float moveSpeed = 2f;
	public float turnSpeed = 150f;
	
	float distanceTraveled;
	public float maximumTravelDistance;
	
	public Color bulletColor = Color.red;
	
	public override void Awake()
	{
		base.Awake();
		
		//GetComponent<ExplodeMesh>().Explode();
		
		Detonator detonator = explosion.GetComponent<Detonator>();
		if(detonator != null)
		{
			detonator.size = 7;
			detonator.duration = 2f;
		}
		
		mineDropRef = Functions.FindChildTransformsRecruisive(transform, "MineDrop_Ref", true)[0];
	}
	
	void Start()
	{			
		GameManager.timer.Add(RandomName + "-MineDropper", 3, true, true, MineSequence);
	}
	
	void OnTriggerEnter(Collider otherObject)
	{
		if(otherObject.tag == "Player")
		{
			Die(GameManager.player);
		}
		else if(otherObject.tag == "Projectile")
		{
			Projectile bullet = otherObject.GetComponent<Projectile>();
			if(bullet.originObject != null)
			{
				if(bullet.originObject.GetComponent<Player>() != null)
				{
					health -= 1;
					hitIndicator.hitIndicatorRemaining = 0.2f;
					
					Functions.PlayAudioClip(transform.position, collisionSound);
					bullet.Die();
				}
			}			
		}
	}

	public override void Die(LivingObject sender)
	{
		if(sender != null && sender.GetType() == typeof(Player))
		{
			GameManager.instance.playerScore += 50;
			Instantiate(explosion, transform.position, transform.rotation);
		}
		
		Functions.PlayAudioClip(transform.position, exposionSound);
		Destroy(gameObject);

		if(OnObjectDied != null)
			OnObjectDied(this);
	}
	
	void Update() 
	{
		if(Time.timeScale <= 0)
			return;

		if(health <= 0)
		{
			Die(GameManager.player);
			return;
		}
		
		if(distanceTraveled >= maximumTravelDistance)
		{
			Die();
			return;
		}
		
		float amountToMove = moveSpeed * Time.deltaTime;
		transform.Translate(amountToMove, 0, 0);
		
		distanceTraveled += amountToMove;
	}

	void MineSequence()
	{
		Transform inst = (Transform)Instantiate(minePrefab, mineDropRef.position, Quaternion.identity);
		Mine proj = inst.GetComponent<Mine>();
		proj.originObject = transform;
		proj.projectileColor = bulletColor;
	}
	
	void OnDestroy() 
	{
		GameManager.timer.RemoveContaining(RandomName);
	}
}