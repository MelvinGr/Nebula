using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

public class TerrorVille : LivingObject 
{	
	public Transform mainTrainsform;
	Dictionary<string, Transform> childDictionary;
	Dictionary<string, int[]> indexes = new Dictionary<string, int[]>();
	
	List<TerrorVilleTurret> dieableChilds = new List<TerrorVilleTurret>();
	
	public Transform targetTransform;
	
	public Transform ondergrond;

	public int goingDownEffectsPrecentage = 20;
	public float ballMoveSpeed = 50; 
	public float ringTurnSpeed = 50;
	public float followTurretMoveSpeed = 50;
	public Transform turretBullet;
	public Transform cannonBullet;
	public Transform enemyPrefab;
	
	int sequenceIndex = 0;
	KeyValuePair<string, int[]> currentIndexGroup;
	int currentIndexGroupIndex = 0;
	
	public AudioClip shootSound;
	public AudioClip explosionSound;
	
	bool isDieing = false;
	
	//Color[] colorChoices = {Color.red, Color.blue, Color.yellow, Color.green};
	
	public bool ChildsWiped
	{
		get 
		{
			foreach(TerrorVilleTurret child in dieableChilds)
			{
				if(child != null)
					return false;
			}

			return true;
		}
	}
	
	public void RegisterChild(TerrorVilleTurret child)
	{
		dieableChilds.Add(child);
	}
	
	public void UnRegisterChild(TerrorVilleTurret child)
	{
		int index = dieableChilds.IndexOf(child);
		if(index >= 0)
			dieableChilds[index] = null;	
	}

	bool PlayerIsInRange()
	{
		return (transform.position.z - GameManager.player.sceneCamera.Constrains.yMin) <= 17;
	}
	
	public override void Awake()
	{
		base.Awake();
		
		indexes.Add("Left_Tower_Turret_", new int[3]);
		indexes.Add("Left_Spike_01_Turret_", new int[3]);
		indexes.Add("Left_Spike_02_Turret_", new int[3]);
		indexes.Add("Right_Tower_Turret_", new int[3]);		
		indexes.Add("Right_Spike_01_Turret_", new int[3]);
		indexes.Add("Right_Spike_02_Turret_", new int[3]);		
		indexes.Add("Left_Base_Turret_", new int[2]);
		indexes.Add("Right_Base_Turret_", new int[2]);		
		indexes.Add("_Ball_Turret", new int[1]);	
		indexes.Add("_Canon_Turret", new int[2]);
		
		ondergrond.gameObject.SetActive(false);
		
		childDictionary = Functions.GetChildTransformDictionary(mainTrainsform, false);

		Detonator detonator = explosion.GetComponent<Detonator>();
		if(detonator != null)
		{
			detonator.size = 2;
			//detonator.duration = 0.5f;
		}
		
		transform.position = new Vector3(21.56f, 3.4f, transform.position.z);
	}
	
	void CurrentGroupSwitcher()
	{ 
		sequenceIndex = (sequenceIndex + 1) % indexes.Count;	

		int i = 0;
		foreach(KeyValuePair<string, int[]> keyValue in indexes)
		{
			if(i == sequenceIndex)
			{
				currentIndexGroup = keyValue;
				break;
			}
			
			i++;
		}
	}
	
	void CurrentIndexGroupIndexSwitcher()
	{
		currentIndexGroupIndex = (currentIndexGroupIndex + 1) % currentIndexGroup.Value.Length;
	}

	void Start()
	{
		//if(GameManager.level != null)
			//transform.position = new Vector3(transform.position.x, GameManager.level.layerHeight, transform.position.z);
		
		if(GameManager.player != null)
			targetTransform = GameManager.player.transform;
		else
			targetTransform = GameObject.Find("TargetCube").transform;
	}
	
	public override void Die(LivingObject sender)
	{
		base.Die(sender);
		
		if(sender != null && sender.GetType() == typeof(Player))
			GameManager.instance.playerScore += 15000;
		
		Functions.PlayAudioClip(transform.position, explosionSound);
		if(GameManager.instance.config.gameOptions.effectEnabled)
			Instantiate(explosion, transform.position, transform.rotation);
			
		transform.GetComponent<Rigidbody>().useGravity = true;	
		transform.GetComponent<Rigidbody>().isKinematic = false;
	}

	void OnDestroy() 
	{
		GameManager.timer.RemoveContaining(RandomName);
	}
	
	void UpdateMotion(KeyValuePair<string, Transform> keyValue, int index)
	{
		if(keyValue.Value == null)
			return;
			
		if(keyValue.Value.transform == null)
			return;
			
		if(keyValue.Key.Contains("_Ring"))
		{
			keyValue.Value.Rotate(Vector3.up * Time.deltaTime * (index % 2 == 0 ? -ringTurnSpeed : ringTurnSpeed));
		}
		else if(keyValue.Key.Contains("_Ball"))
		{
			keyValue.Value.position += new Vector3(0, Mathf.Sin(Time.time) / 10, 0);
		}
		else if(keyValue.Key.Contains("_Turret") && !keyValue.Key.Contains("_Ball") && !keyValue.Key.Contains("_Cannon"))
		{
			Vector3 simulatedTargetPos = new Vector3(targetTransform.position.x, keyValue.Value.position.y, targetTransform.position.z);
			Vector3 direction = Functions.Direction(keyValue.Value.position, simulatedTargetPos);
			
			Quaternion rotation = Quaternion.RotateTowards(keyValue.Value.rotation,
				Quaternion.LookRotation(direction), followTurretMoveSpeed * Time.deltaTime);
			
			//if(rotation.y >= 0.55f && rotation.y <= 0.65f)
			keyValue.Value.rotation = rotation;
		}
		else if(keyValue.Key.Contains("_Cannon"))
		{
			//
		}
	}
	
	void UpdateShoot()
	{		
		Transform bullet;
		string key;
		
		if(currentIndexGroup.Key.Contains("Canon") || currentIndexGroup.Key.Contains("Ball"))
		{
			key = (currentIndexGroupIndex == 0 ? "Left" : "Right") + currentIndexGroup.Key;
			bullet = turretBullet;
		}
		else
		{
			key = currentIndexGroup.Key + String.Format("{0:00}", currentIndexGroupIndex + 1);
			bullet = turretBullet;
		}
		
		Transform t = null;		
		foreach(TerrorVilleTurret tt in dieableChilds)
		{
			if(tt == null)
				continue;
				
			if(tt.name == key)
			{
				t = tt.transform;
				break;
			}
		}
		
		if(t == null)
			return;

		Transform inst = (Transform)Instantiate(bullet, t.position, t.rotation);
		Bullet rocket = inst.GetComponent<Bullet>(); 
		rocket.originObject = t;
		rocket.bulletSpeed = 10;
		rocket.transform.localScale = new Vector3(2, 2, 2);	
		rocket.projectileColor = Color.gray;
		
		Functions.PlayAudioClip(inst.position, shootSound);
	}
	
	void Update() 
	{
		if(targetTransform == null || Time.timeScale <= 0)
			return;
			
		if(PlayerIsInRange())
		{
			GameManager.player.config.onRails = false;
			GameManager.player.sceneCamera.config.followPlayer = false;
		
			GameManager.timer.Add(RandomName + "_Sequence-CurrentGroupSwitcher", 1, true, true, CurrentGroupSwitcher);
			GameManager.timer.Add(RandomName + "_Sequence-CurrentIndexGroupIndexSwitcher", 0.5f, true, true, CurrentIndexGroupIndexSwitcher);			
			GameManager.timer.Add(RandomName + "_Sequence-UpdateShoot", 0.4f, true, true, UpdateShoot);		
		}
		else
			GameManager.timer.RemoveContaining(RandomName + "_Sequence-");
					
		int i = 0;
		foreach(KeyValuePair<string, Transform> keyValue in childDictionary)
		{
			UpdateMotion(keyValue, i);				
			i++;
		}
		
		if(ChildsWiped && !isDieing)
		{
			Die(GameManager.player);
			isDieing = true;
		}
	}
}
