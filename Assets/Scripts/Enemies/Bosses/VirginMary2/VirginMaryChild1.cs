using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VirginMaryChild1 : LivingObject 
{
	[HideInInspector]
	public List<Transform> gunTransforms = new List<Transform>();

	public Transform bulletPrefab;
	
	public AudioClip shootSound;
	public AudioClip exposionSound;
	public AudioClip collisionSound;
	
	public Transform targetTransform;
	
	public float moveSpeed = 2f;
	public float roundsPerMinute = 60;
	public float fireRange = 20;	
	public float turnSpeed = 150f;
	
	public Color bulletColor = Color.red;

	public BezierCurveManager bezierCurveManager;

	float startTime;
	
	public override void Awake()
	{
		base.Awake();
		
		//GetComponent<ExplodeMesh>().Explode();
		
		gunTransforms = Functions.FindChildTransformsRecruisive(transform, "Turret", true);
			
		Detonator detonator = explosion.GetComponent<Detonator>();
		if(detonator != null)
		{
			detonator.size = 7;
			detonator.duration = 2f;
		}
		
		startTime = Time.time;
	}
	
	void Start()
	{
		if(GameManager.player != null)
			targetTransform = GameManager.player.transform;
		else
			targetTransform = GameObject.Find("TargetCube").transform;
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
		
		transform.position = bezierCurveManager.GetPositionAtTime(Time.time - startTime);
		
		foreach(Transform gun in gunTransforms)
		{
			Vector3 direction = Functions.Direction(gun.position, new Vector3(targetTransform.position.x, gun.position.y, targetTransform.position.z));
			gun.rotation = Quaternion.RotateTowards(gun.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
			
			string timerName = RandomName + "-" + gun.name;
			if(transform.position.z > targetTransform.position.z)			
				GameManager.timer.Add(timerName, 2f, true, true, GunSequence);
			else
				GameManager.timer.Remove(timerName);
		}
	}
	
	void GunSequence()
	{
		foreach(Transform gun in gunTransforms)
		{
			Vector3 simulatedTargetPos = new Vector3(targetTransform.position.x, gun.position.y, targetTransform.position.z);
			Vector3 direction = Functions.RelativePos(gun.position, simulatedTargetPos);					
			Transform inst = (Transform)Instantiate(bulletPrefab, gun.position, Quaternion.LookRotation(direction));
			//inst.localScale *= 1.3f;
			
			Bullet proj = inst.GetComponent<Bullet>();
			proj.originObject = transform;
			proj.bulletSpeed = 12;
			proj.projectileColor = bulletColor;

			if(shootSound != null)
				Functions.PlayAudioClip(inst.position, shootSound);
		}
	}
	
	void OnDestroy() 
	{
		GameManager.timer.RemoveContaining(RandomName);
	}
}