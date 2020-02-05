using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VirginMary2 : LivingObject 
{
	List<Transform> spreadCannonTransforms;
	List<Transform> followCannonTransforms;
	List<Transform> mineChildSpawnPoints;
	
	List<Transform> spawnedChildList = new List<Transform>();
	
	public Transform mineChild, shootChild;

	public AudioClip shootSound;
	public AudioClip exposionSound;
	public AudioClip collisionSound;
	
	public List<BezierCurveManager> bezierCurveList = new List<BezierCurveManager>();
	
	int followCannonIndex = 0;
	int spreadCannonIndex = 0;
	int spreadCannonSubIndex = 0;
	
	float pongValue = 25;
	float startX = 0;
	
	public Transform targetTransform;
	public Transform homingRocketPrefab;
	public Transform bulletPrefab;
	
	Transform bezierCurveManagers;

	public int goingDownEffectsPrecentage = 20;
	GameObject goingDownEffects;
	
	bool mineDroperChildLeftAlive, mineDroperChildRightAlive;

	bool spawnedSequence2 = true;
	
	bool PlayerIsInRange()
	{
		return (transform.position.z - GameManager.player.sceneCamera.Constrains.yMin) <= 10;
	}
	
	public override void Awake()
	{
		base.Awake();
		
		goingDownEffects = Functions.FindChildTransformsRecruisive(transform, "DownEffects", true)[0].gameObject;
		goingDownEffects.SetActive(false);
		
		//GetComponent<ExplodeMesh>().Explode();		
		spreadCannonTransforms = Functions.FindChildTransformsRecruisive(transform, "Spread_Ref", true);
		followCannonTransforms = Functions.FindChildTransformsRecruisive(transform, "Follow_Ref", true);

		mineChildSpawnPoints = Functions.FindChildTransformsRecruisive(transform, "MineSpawnPoint_Ref", true);		
		bezierCurveManagers = Functions.FindChildTransformsRecruisive(transform, "BezierCurveManagers", true)[0];
	}

	void Start()
	{
		if(GameManager.level != null)
			transform.position = new Vector3(transform.position.x, GameManager.level.layerHeight, transform.position.z);
			
		if(GameManager.player != null)
			targetTransform = GameManager.player.transform;
		else
			targetTransform = GameObject.Find("TargetCube").transform;

		mineChildSpawnPoints[0].parent.parent = null;
		mineChildSpawnPoints[0].parent.position = transform.position;
		
		bezierCurveManagers.parent = null;				
		bezierCurveManagers.position = transform.position;
		
		foreach(Transform trans in Functions.FindChildTransformsRecruisive(bezierCurveManagers, "BezierCurve", true))
			bezierCurveList.Add(trans.GetComponent<BezierCurveManager>());		
			
		startX = transform.position.x;			
	}
	
	/*void PowerSpawner()
	{
		foreach(Transform d in GameManager.instance.pickUpPrefabs)
		{
			if(d.name.Replace("_Prefab", "") == "HealthPickup")
			{
				Transform spawn = (Transform)Instantiate(d, new Vector3(startX, 0, transform.position.z - 5), Quaternion.identity);		
				//PickUp pickUp = spawn.GetComponent<PickUp>();
				
				break;
			}
		}
	}*/

	/*void SequenceSwitcher()
	{	
		sequenceIndex = 2;//sequenceIndex % 3;
		
		switch(sequenceIndex)
		{
			case 0: 
			{ 
				GameManager.timer.Remove(RandomName + "_Sequence2");
				GameManager.timer.Add(RandomName + "_Sequence0", 1f, true, false, Sequence0);
				
				break; 
			}
			case 1:
			{ 
				GameManager.timer.Remove(RandomName + "_Sequence0");
				GameManager.timer.Add(RandomName + "_Sequence1", 3f, true, false, Sequence1);
				
				break; 
			}
			case 2:
			{ 
				GameManager.timer.Remove(RandomName + "_Sequence1");
				GameManager.timer.Add(RandomName + "_Sequence2", 8f, true, true, Sequence2);
				
				break; 
			}
		}
		
		sequenceIndex++;
	}*/
	
	void Sequence0() // spread
	{	
		spreadCannonIndex = 0;//spreadCannonIndex % 2;
		spreadCannonSubIndex = spreadCannonSubIndex % 2;

		int firstIndex = 0;
		int secondIndex = 0;
		
		switch(spreadCannonSubIndex)
		{	
			case 0: 
			{
				firstIndex = 0;
				secondIndex = 3;
				
				break;
			}
			
			case 1: 
			{
				firstIndex = 1;
				secondIndex = 2;
				
				break;
			}
		}
		
		for(int i = 0; i < 2; i++)
		{		
			if(spreadCannonIndex == 1)
			{
				firstIndex += 4;
				secondIndex += 4;
			}
			
			Quaternion rotation = transform.rotation * Quaternion.AngleAxis(spreadCannonIndex == 0 ? 180 : -180, Vector3.up);
			
			Transform inst = (Transform)Instantiate(bulletPrefab, spreadCannonTransforms[i == 0 ? firstIndex : secondIndex].position, rotation);
			Bullet rocket = inst.GetComponent<Bullet>(); 
			rocket.originObject = transform;	
			//rocket.target = targetTransform;
			rocket.bulletSpeed = 10;
			rocket.ignoreCleaner = true;
		}
			
		Functions.PlayAudioClip(transform.position, shootSound);
		
		spreadCannonIndex++;
		spreadCannonSubIndex++;
	}
	
	void Sequence1() // guided (missle)
	{
		followCannonIndex = followCannonIndex % 2;
		
		for(int i = 0; i < 4; i++)
		{		
			int index = (followCannonIndex == 0 ? i : i + 4);			
			Quaternion rotation = transform.rotation * Quaternion.AngleAxis(followCannonIndex == 0 ? 90 : -90, Vector3.up);
			
			Transform inst = (Transform)Instantiate(homingRocketPrefab, followCannonTransforms[index].position, rotation);
			HomingRocket rocket = inst.GetComponent<HomingRocket>(); 
			rocket.originObject = transform;
			rocket.target = targetTransform;
			rocket.bulletSpeed = 10;
			rocket.ignoreCleaner = true;			
		}
				
		Functions.PlayAudioClip(transform.position, shootSound);		
		followCannonIndex++;
	}
	
	void Sequence2() // spawn bezier attack
	{	
		for(int i = 0; i < 2; i++)
		{					
			Transform inst = (Transform)Instantiate(shootChild, ((BezierWaypoint)bezierCurveList[i].waypointList[0]).transform.position, Quaternion.identity);
			spawnedChildList.Add(inst);
			
			VirginMaryChild1 child = inst.GetComponent<VirginMaryChild1>();	
			child.bezierCurveManager = bezierCurveList[i];
		}		
	}
	
	void mineChildDied(LivingObject sender) 
	{
		if(sender.transform.rotation.y < 0)
			mineDroperChildLeftAlive = false;
		else
			mineDroperChildRightAlive = false;				
	}
	
	void Sequence3(int index) // drop mines
	{
		int mineDropperIndex = index % 2;
		if(mineDropperIndex == 0)
		{
			if(!mineDroperChildLeftAlive)
				mineDroperChildLeftAlive = true;
			else
				return;
		}
		else
		{
			if(!mineDroperChildRightAlive)
				mineDroperChildRightAlive = true;
			else
				return;
		}
		
		Transform inst = (Transform)Instantiate(mineChild, mineChildSpawnPoints[mineDropperIndex].position, Quaternion.AngleAxis(mineDropperIndex == 0 ? 0 : 180, Vector3.up));
		spawnedChildList.Add(inst);
		
		VirginMaryChild3 child = inst.GetComponent<VirginMaryChild3>();	
		//child.moveSpeed = (mineDropperIndex == 0 ? -child.moveSpeed : child.moveSpeed);
		child.OnObjectDied = mineChildDied;	
		child.maximumTravelDistance = mineChildSpawnPoints[1].position.x - mineChildSpawnPoints[0].position.x;
	}
	
	public override void Die(LivingObject sender)
	{
		base.Die(sender);
		
		if(sender != null && sender.GetType() == typeof(Player))
		{
			GameManager.instance.playerScore += 10000;
		}
		
		foreach(Transform trans in spawnedChildList)
		{
			if(trans == null)
				continue;
				
			LivingObject obj = trans.GetComponent<LivingObject>();
			if(obj != null)
				obj.Die(sender);
		}

		if(GameManager.instance.config.gameOptions.effectEnabled)
			Instantiate(explosion, transform.position, transform.rotation);
			
		Functions.PlayAudioClip(transform.position, exposionSound);
		
		Destroy(gameObject);
		Destroy(bezierCurveManagers.gameObject);
		Destroy(mineChildSpawnPoints[0].gameObject);
	}
	
	void OnTriggerEnter(Collider otherObject)
	{
		if(otherObject.tag == "Block")
			Die(GameManager.player);
		else if(otherObject.tag == "Projectile")
		{
			Projectile projectile = otherObject.GetComponent<Projectile>();
			if(projectile.originObject != null)
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
	
	void OnDestroy() 
	{
		GameManager.timer.RemoveContaining(RandomName);
	}

	void Update() 
	{	
		if(health <= (spawnHealth / 100 * goingDownEffectsPrecentage))
			goingDownEffects.SetActive(true);
			
		if(transform.rigidbody.useGravity)
		{
			float t = Mathf.Sin(Time.time * 0.5f);			
			transform.rotation = Quaternion.Euler(t * 360, 0, 0);
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
				
			GameManager.timer.Add(RandomName + "_Sequence0", 1f, true, false, Sequence0);
			GameManager.timer.Add(RandomName + "_Sequence1", 10f, true, false, Sequence1);
			GameManager.timer.Add(RandomName + "_Sequence2", 15f, true, false, Sequence2);		
		}
		else
			GameManager.timer.RemoveContaining(RandomName + "_Sequence");
		
		if(transform.position.x <= startX - (pongValue / 2) + 1)
		{
			Sequence3(1);
			spawnedSequence2 = false;
			//GameManager.timer.Remove(RandomName + "_Sequence1");
		}
		else if(transform.position.x >= startX + (pongValue / 2) - 1)
		{
			Sequence3(0);
			spawnedSequence2 = false;
			//GameManager.timer.Remove(RandomName + "_Sequence1");
		}
			
		if(Mathf.Round(transform.position.x) == startX && !spawnedSequence2)
		{			
			Sequence2();			
			spawnedSequence2 = true;
			Sequence1();//GameManager.timer.Add(RandomName + "_Sequence1", 5f, true, true, Sequence1);
		}
		
		float newX = Mathf.PingPong(Time.time * 2, pongValue) - (pongValue / 2);		
		transform.position = new Vector3(startX - newX, transform.position.y, transform.position.z);
    }
}