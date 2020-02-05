using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class PlayerConfig
{	
	public float playerSpeed;
	public float slowPrecentage;
	public float smooth;
	public bool horizontalEnabled;
	public bool verticalEnabled;
	public bool onRails;
	public bool inputEnabled;
	//public int health;
	public float deathCountdown;
	public bool imortal;	
}

public class Player : LivingObject
{
	public Transform fireBall;
	
	public AudioClip shootSound; 
	public AudioClip shootUpgradeSound; 
	public AudioClip explosionSound;
	public AudioClip collisionSound;
	
	//[HideInInspector]
	//public bool isSafeToTransition;

	//float startTime;
	//bool didChange = true;

	public Transform smoke;
	public AudioClip smokeSound;

	public Transform[] gunRefs;

	public SceneCamera sceneCamera;
	
	[HideInInspector]
	public float amountToMove = 0;
	
	[HideInInspector]
	public bool isGoingDown = false;
	
	public PlayerConfig config;
	
	GameObject goingDownEffects;
	
	float spaceDownTime = 0;
	
	public bool shouldDie = false;
	
	float maxPowerUpTime = 6;
	//float timeWeaponChanged = 0;
	
	bool isFading = false;
	bool didExplode = false;
	bool didSlow = false;

	public float originalSpeed { get; private set; }

	WeaponType _weaponType; 
	public WeaponType weaponType
	{
		get { return _weaponType; }
		set
		{
			//timeWeaponChanged = Time.time;
			_weaponType = value;
		}
	}
	
	public override void Awake()
	{
		base.Awake();
		
		GameManager.player = this;
		GameManager.instance.playerScore = 0;
	}

	void Start()
	{
		if(GameManager.level != null)
			transform.position = new Vector3(transform.position.x, GameManager.level.layerHeight, transform.position.z);
			
		config = Functions.DeserializeFromResourcesXML<PlayerConfig>("Configs/playerConfig");	
		
		gunRefs = Functions.FindChildTransformsRecruisive(transform, "Gun_Ref", true).ToArray();

		goingDownEffects = Functions.FindChildTransformsRecruisive(transform, "downeffects", true)[0].gameObject;
		goingDownEffects.SetActive(false);
		
		originalSpeed = config.playerSpeed;
		
		hitIndicator.blink = true;

///////////////////////////////////
		//config.onRails = false;
		//config.imortal = true;
///////////////////////////////////
	}
	
	public void Shoot()
	{
		if(hitIndicator.hitIndicatorRemaining > 0)
			return;
			
		switch(weaponType)
		{
			case WeaponType.ProtonBlaster:
			{
				Transform inst = (Transform)Instantiate(fireBall, transform.position + gunRefs[0].localPosition, transform.rotation);
			
				/*if(transform.localScale.x > 1)
					inst.localScale += transform.localScale;
				else if(transform.localScale.x < 1)
					inst.localScale -= transform.localScale;
				*/
				
				Projectile proj = inst.GetComponent<Projectile>();
				proj.bulletSpeed = originalSpeed * 4;
				proj.originObject = transform;				
				proj.projectileColor = Color.blue;
				proj.transform.localScale = new Vector3(2, 2, 2);
				
				//audio.PlayOneShot(shootSound);
				Functions.PlayAudioClip(inst.position, shootSound);
					
				break;
			}
			
			case WeaponType.DoubleBlaster:
			{
				Vector3? audioPos = null;
				for(int i = 0; i < 2; i++)
				{
					Transform inst = (Transform)Instantiate(fireBall, transform.position + gunRefs[i + 1].localPosition, transform.rotation);
					
					Projectile proj = inst.GetComponent<Projectile>();
					proj.bulletSpeed = originalSpeed * 4;
					proj.originObject = transform;				
					proj.projectileColor = Color.red;
					proj.transform.localScale = new Vector3(2, 2, 2);	

					if(!audioPos.HasValue)
						audioPos = inst.position;
				}
				
				if(audioPos.HasValue)
					Functions.PlayAudioClip(audioPos.Value, shootUpgradeSound);
				
				break;
			}
			
			/*case WeaponType.DoubleBlaster:
			{
				int spreadGunBulletCount = (weaponType == WeaponType.Spread ? 3 : 6);
				
				float angle = -(angleStep * spreadGunBulletCount / 2);					
				for(int i = 0; i < spreadGunBulletCount; i++)
				{
					Transform inst = (Transform)Instantiate(fireBall, transform.position + gunRef.localPosition, transform.rotation);
					inst.eulerAngles += new Vector3(0, angle, 0);
					
					if(transform.localScale.x > 1)
						inst.localScale += transform.localScale;
					else if(transform.localScale.x < 1) 
						inst.localScale -= transform.localScale;
	
					Projectile proj = inst.GetComponent<Projectile>();
					proj.originObject = transform;
					proj.bulletSpeed = config.playerSpeed * 4;	
					proj.projectileColor = Color.blue;
					
					//audio.PlayOneShot(shootSound);
					Functions.PlayAudioClip(inst.position, shootSound);
					
					angle += angleStep;
				}
				
				break;
			}*/
		}
	}

	void Update()
	{
		if(Time.timeScale <= 0)
			return;		
			
		if(health <= 0)
		{
			isGoingDown = true;
			transform.GetComponent<Rigidbody>().useGravity = true;	
			transform.GetComponent<Rigidbody>().isKinematic = false;	
			
			if(goingDownEffects != null)
				goingDownEffects.SetActive(true);
		}

		if(shouldDie)
			config.deathCountdown -= Time.deltaTime;
			
		if(transform.position.y <= 0 && isGoingDown)
		{
			isGoingDown = false;
			shouldDie = true;
			transform.GetComponent<Rigidbody>().useGravity = false;
			transform.GetComponent<Rigidbody>().isKinematic = true;
			
			if(!didExplode)
			{
				if(GameManager.instance.config.gameOptions.effectEnabled)
					Instantiate(explosion, transform.position, transform.rotation);
					
				Functions.PlayAudioClip(transform.position, explosionSound);
					
				didExplode = true;
			}
		}
		
		if(config.deathCountdown <= 0)
			Die();
		
		if(isGoingDown)
		{
			float t = Mathf.Sin(Time.time * 0.5f);			
			transform.rotation = Quaternion.Euler(t * -45, t * 360, 0);
		}
		else
		{
			if(!transform.GetComponent<Rigidbody>().useGravity)
				HandleMovement();
			
			if(config.inputEnabled && !transform.GetComponent<Rigidbody>().useGravity)
			{
				if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.X))			
					Shoot();
				else if(Input.GetKey(KeyCode.Space) || Input.GetKeyDown(KeyCode.X))
				{
					if(spaceDownTime <= 0)
					{
						Shoot();
						spaceDownTime = 0.2f;
					}
					else
						spaceDownTime -= Time.deltaTime;
				}
			}
		}
		
		/*if(hitIndicator.hitIndicatorRemaining > 0 && !didSlow)
		{
			sceneCamera.config.cameraSpeed *= 0.2f;
			config.playerSpeed *= 0.2f;
			didSlow = true;
		}
		else if(hitIndicator.hitIndicatorRemaining <= 0 && didSlow)
		{
			sceneCamera.config.cameraSpeed = sceneCamera.originalSpeed;
			config.playerSpeed = originalSpeed;
			didSlow = false;
		}*/
		
		//if(Time.time >= timeWeaponChanged + maxPowerUpTime)
			//weaponType = WeaponType.ProtonBlaster;
			
		//if(Debug.isDebugBuild)
			//HandleCheats();
	}
	
	void Respawn()
	{
		transform.position = new Vector3(20.68f, GameManager.level.layerHeight, Camera.main.transform.position.z - 4);
		foreach(MeshRenderer renderer in meshRenderers)
				renderer.enabled = true;
				
		((BoxCollider)GetComponent<Collider>()).size = colliderSize;
		config.imortal = false;
		
		//
	}
	
	Vector3 colliderSize;
	public MeshRenderer[] meshRenderers;

	void OnTriggerEnter(Collider otherObject)
	{
		if(config.imortal)
			return;
		
		/*if(otherObject.tag == "Block")
		{
			isGoingDown = false;
			shouldDie = true;
			transform.rigidbody.useGravity = false;	
			transform.rigidbody.isKinematic = true;	
			
			Instantiate(explosion, transform.position, transform.rotation);
		}
		else */if(otherObject.tag == "Enemy" && hitIndicator.hitIndicatorRemaining <= 0)
		{
			health -= 1;//Functions.GetBulletDamage(this, bullet);
			foreach(MeshRenderer renderer in meshRenderers)
				renderer.enabled = false;
				
			colliderSize = ((BoxCollider)GetComponent<Collider>()).size;
			((BoxCollider)GetComponent<Collider>()).size = Vector3.zero;
			hitIndicator.hitIndicatorRemaining = 5f;
			config.imortal = true;
	
			weaponType = WeaponType.ProtonBlaster;
			
			//hitIndicator.hitIndicatorRemaining = 2f;
			GameManager.timer.Add("Player-Respawn-" + Time.time, 2, false, false, Respawn);
			
			Functions.PlayAudioClip(transform.position, collisionSound);
		}
		else if(otherObject.tag == "Boss" || otherObject.tag == "ThePilarChild")
		{
			health = 0;
			GameManager.instance.lostLife = true;
		}
		else if(otherObject.tag == "Projectile" && hitIndicator.hitIndicatorRemaining <= 0)
		{				
			Projectile bullet = otherObject.GetComponent<Projectile>();
			if(bullet.originObject != null)
			{
				if((bullet.originObject.GetComponent<LivingObject>() != null || bullet.originObject.GetComponent<TerrorVilleTurret>() != null) &&
					bullet.originObject.GetComponent<Player>() == null)
				{			
					health -= 1;//Functions.GetBulletDamage(this, bullet);
					foreach(MeshRenderer renderer in meshRenderers)
						renderer.enabled = false;
					
					colliderSize = ((BoxCollider)GetComponent<Collider>()).size;
					((BoxCollider)GetComponent<Collider>()).size = Vector3.zero;
					hitIndicator.hitIndicatorRemaining = 5f;
					config.imortal = true;
					
					weaponType = WeaponType.ProtonBlaster;
					
					//hitIndicator.hitIndicatorRemaining = 2f;
					GameManager.timer.Add("Player-Respawn-" + Time.time, 2, false, false, Respawn);
					
					Functions.PlayAudioClip(transform.position, collisionSound);
					bullet.Die();
				}					
			}
		}
	}

	public override void Die()
	{	
		base.Die();
		
		//health = totalHealth;	
		GameManager.instance.playerDied = true;
		GameManager.instance.pickedUpPowerUp = false;
		GameManager.instance.lostLife = false;
		
		if(!isFading)
		{
			CameraFade cam = Camera.main.GetComponent<CameraFade>();
			cam.OnFadeOutCompleted += delegate() { Application.LoadLevel("Results"); };
			cam.FadeOut();
			
			isFading = true;
		}
	}
	
	void HandleMovement()
	{
		if(sceneCamera != null)
		{
			Vector3 size = gameObject.GetComponent<AlternativeMeshRenderer>().transformThatContainsRenderer.GetComponent<Renderer>().bounds.size;
			Vector2[] vects = Functions.RectToVector2(Functions.AddRect(sceneCamera.Constrains, new Rect(size.x / 2, size.z / 2, -size.x / 2, -size.z / 2)));
			
			Vector3 tmp = Functions.ClampVector3(transform.position, 
				Functions.Vector2To3(vects[0], transform.position.y), 
				Functions.Vector2To3(vects[2], transform.position.y));	
			
			transform.position = new Vector3(tmp.x, transform.position.y, tmp.z);			
		}
		
		//Functions.VisualizeRect(sceneCamera.Get2DConstrains(), Color.red, transform.position.y);
		if(config.horizontalEnabled && config.inputEnabled)
		{
			float amountToMove = config.playerSpeed * Input.GetAxisRaw("Horizontal") * Time.deltaTime;
			transform.Translate(amountToMove, 0, 0, 0);

			float rotationAngle = Mathf.Clamp(amountToMove * 120, -90, 90);			
			Quaternion target = Quaternion.Euler(new Vector3(0, 0, -rotationAngle));
			transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * config.smooth); 
		}
 
		if(config.verticalEnabled && config.inputEnabled)
		{
			float val = Input.GetAxisRaw("Vertical");
			if(config.onRails && sceneCamera != null)
			{			
				amountToMove =
				(
					val != 0 && config.inputEnabled ?
					sceneCamera.config.cameraSpeed + config.playerSpeed * val :
					sceneCamera.config.cameraSpeed
				) * Time.deltaTime;

				transform.Translate(0, 0, amountToMove, 0);
			}
			else if(config.inputEnabled)
			{
				transform.Translate(0, 0, config.playerSpeed * val * Time.deltaTime, 0);
			}
		}
	}

	void HandleCheats()
	{
		if(Input.GetKeyDown(KeyCode.F2))
			config.imortal = !config.imortal;
		else if(Input.GetKeyDown(KeyCode.E))
			Time.timeScale = (Time.timeScale + 1) % 2;
		else if(Input.GetKeyDown(KeyCode.F9))
		{
			List<WaveSpawn> spawns = new List<WaveSpawn>();

			foreach (Wave wave in GameObject.Find("Level/Enemies").GetComponentsInChildren<Wave>())
				spawns.Add(wave.waveSpawn);    

			spawns.Sort(delegate(WaveSpawn c1, WaveSpawn c2) { return c1.startPosition.z.CompareTo(c2.startPosition.z); });
			
			Functions.SerializeToXMLFile(spawns, "spawns.xml");
		}		
		else if(Input.GetKeyDown(KeyCode.F10))
		{
			GameManager.level.config.wavesLoopCount = 0;
			foreach(Wave wave in GameManager.level.waveList)
			{
				if(wave == null)
					continue;

				wave.Wipe(false);
			}
		}
	}
}