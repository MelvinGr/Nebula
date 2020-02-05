using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[Serializable]
public class LevelConfig
{
	public int ID;

	public string[] blockArrangement;
	public WaveSpawn[] waveSpawnList;

	public String bossName;
	public PickUpSpawn[] pickUpSpawnList;

	public int wavesLoopCount;
	public bool loopPickUps;
	public bool spawnEnemies;
	public bool spawnBlocks;
	public bool spawnPickups;
	public float loopPickUpsDelay;
	public bool spawnTurrets;
	public int turretRoundsPerMinute;
}

public class Level : MonoBehaviour
{
	class BlockClass
	{
		public Transform transform;
		public float rotation;

		public BlockClass(Transform t, float r)
		{
			transform = t;
			rotation = r;
		}

		public override string ToString()
		{
			return transform.name + " | " + rotation;
		}
	}

	public GUIScript guiScript;

	[HideInInspector]
	public float layerHeight = 0f;

	[HideInInspector]
	public Vector3 blockSize;

	[HideInInspector]
	public float maximumLevelWidth;

	[HideInInspector]
	public List<Wave> waveList = new List<Wave>();

	[HideInInspector]
	public List<PickUp> pickUpList = new List<PickUp>();

	List<List<BlockClass>> blockMatrix = new List<List<BlockClass>>();

	public LevelConfig config;
	//float newLevelStartZ = 0;
	int blockLineIndex = 0;

	[HideInInspector]
	public bool bossActive = false;

	public GameObject waterObject;

	[HideInInspector]
	public GameObject waterCameraObject;

	float maximumEnemyDistanceBehindPlayer;
	float maximumDistanceBehindPlayer;
	float blockSpawnTreshhold;
	float bossSpawnTreshhold;

	GameObject enemies, blocks, powerUps;
	
	bool firstBoss = true;
	
	public bool WavesWiped
	{
		get 
		{
			foreach(Wave wave in waveList)
			{
				if(wave != null)
					return false;
			}

			return true;
		}
	}

	GameObject CreateGameObjectWithParent(String name)
	{
		GameObject obj = new GameObject(name);
		obj.transform.parent = transform;
		obj.transform.localPosition = Vector3.zero;

		return obj;
	}

	void Awake()
	{
		GameManager.instance.playerScore = 0;
		UnityEngine.Random.seed = Environment.TickCount;
		guiScript = Functions.GetComponentInGameObject<GUIScript>("GUI");
		
		if(GameObject.Find("MainMenuSound") != null)
			Destroy(GameObject.Find("MainMenuSound"));

		//if(waterObject != null)
			//waterCameraObject = waterObject.GetComponent<PlanarReflection>().reflectionCamera.gameObject;

		GameManager.level = this;

		enemies = CreateGameObjectWithParent("Enemies");
		blocks = CreateGameObjectWithParent("Blocks");
		powerUps = CreateGameObjectWithParent("PowerUps");

		blockSize = GameManager.instance.blockPrefabs[0].GetComponent<AlternativeMeshRenderer>().transformThatContainsRenderer.renderer.bounds.size;
		layerHeight = blockSize.y;
	}

	void Start()
	{
		config = Functions.DeserializeFromResourcesXML<LevelConfig>("Configs/Levels/level" + GameManager.instance.levelID);
		//Functions.SerializeToXML(config, "C:/level0.xml");

		ParseBlocks();
		ParsePickups();

		maximumLevelWidth = blockMatrix[0].Count * blockSize.x;

		maximumEnemyDistanceBehindPlayer = 5;//blockSize.z / 2;
		maximumDistanceBehindPlayer = blockSize.z;

		blockSpawnTreshhold = blockSize.z * 1.5f;
		bossSpawnTreshhold = blockSize.z;

		guiScript.SetNextStageEnabled(true, (GameManager.instance.levelID == 0));
		SpawnEnemies(new Vector3(0, 0, 12));
	}

	void ParseBlocks()
	{
		blockMatrix.Clear();
		foreach(string blockLine in config.blockArrangement)
		{
			List<BlockClass> innerArray = new List<BlockClass>();
			foreach(string block in blockLine.Split(','))
			{
				bool gotBlock = false;
				foreach(Transform d in GameManager.instance.blockPrefabs)
				{
					string[] parts = block.Split('|');
					if(d.name.Replace("_Prefab", "") == parts[0].Trim())
					{
						float rot = 0;
						if(parts.Length == 2)
							rot = float.Parse(parts[1].Trim());

						innerArray.Add(new BlockClass(d, rot));

						gotBlock = true;
						break;
					}
				}

				if(!gotBlock)
					innerArray.Add(null);
			}

			blockMatrix.Add(innerArray);
		}
	}

	void ParsePickups()
	{
		pickUpList.Clear();
		foreach(PickUpSpawn pickUpSpawn in config.pickUpSpawnList)
		{
			foreach(Transform d in GameManager.instance.pickUpPrefabs)
			{
				if(d.name.Replace("_Prefab", "") == pickUpSpawn.name)
				{
					Vector2 center = Functions.RectCenter(GameManager.player.sceneCamera.Constrains);
					float x = center.x;

					Transform spawn = (Transform)Instantiate(d, new Vector3(x, layerHeight, 0) + pickUpSpawn.startPosition, Quaternion.identity);
					spawn.parent = powerUps.transform;

					PickUp pickUp = spawn.GetComponent<PickUp>();
					pickUp.OnPickUp += OnPickUp;
					pickUp.OnPickUpFailed += OnPickUpFailed;

					pickUpList.Add(pickUp);

					break;
				}
			}
		}
	}

	void SpawnEnemies(Vector3 offset)
	{
		if(!config.spawnEnemies)
			return;

		//waveList.Clear();
		foreach(WaveSpawn waveSpawn in config.waveSpawnList)
		{
			foreach(Transform waveConfig in GameManager.instance.wavePrefabs)
			{
				if(waveConfig.name.Replace("_Prefab", "") == waveSpawn.name)
				{
					//Vector2 center = Functions.RectCenter(GameManager.player.sceneCamera.Constrains);
					//float x = center.x;

					Transform waveTrans = (Transform)Instantiate(waveConfig, offset + waveSpawn.startPosition/* + new Vector3(x, layerHeight, 0)*/, Quaternion.identity);
					waveTrans.parent = enemies.transform;

					Wave wave = waveTrans.GetComponent<Wave>();
					wave.OnWaveWiped += OnWaveWiped;
					//wave.waveSpawn = (WaveSpawn)((ICloneable)waveSpawn).Clone();
					waveList.Add(wave);

					break;
				}
			}
		}
	}

	void SpawnBoss()
	{
		//foreach(Wave wave in waveList)
			//wave.Wipe(false);

		config.wavesLoopCount = 0;

		foreach(Transform trans in GameManager.instance.bossPrefabs)
		{
			if(config.bossName == trans.name.Replace("_Prefab", ""))
			{
				Vector2 center = Functions.RectCenter(GameManager.player.sceneCamera.Constrains);
				float x = center.x;

				Transform spawn = (Transform)Instantiate(trans, new Vector3(x, layerHeight,
					GameManager.player.transform.position.z + bossSpawnTreshhold), Quaternion.identity);

				spawn.parent = enemies.transform;

				LivingObject boss = spawn.GetComponent<LivingObject>();
				boss.OnObjectDied += OnBossDied;

				bossActive = true;
				break;
			}
		}
	}

	void OnPickUp(LivingObject sender)
	{
		PickUp pickup = (PickUp)sender;
	
		if(!GameManager.instance.config.achievementSave.fullyEquipped.Contains(pickup.pickUpType))
			GameManager.instance.config.achievementSave.fullyEquipped.Add(pickup.pickUpType);

		GameManager.instance.pickedUpPowerUp = true;

		switch(pickup.pickUpType)
		{
			case PickUpType.Health:
			{
				//if(GameManager.player.health < 3)
					GameManager.player.health++;

				break;
			}

			case PickUpType.GunSpread:
			{
				GameManager.player.weaponType = WeaponType.DoubleBlaster;

				break;
			}
		}

		if(config.loopPickUps)
			GameManager.timer.Add("Level_PickUpLoop_" + pickup.RandomName, config.loopPickUpsDelay, false, false, delegate(){ PickUpLoop(pickup); });
	}
	
	void OnPickUpFailed(LivingObject sender)
	{
		PickUp pickup = (PickUp)sender;

		if(config.loopPickUps)
			GameManager.timer.Add("Level_PickUpLoop_" + pickup.RandomName, config.loopPickUpsDelay, false, false, delegate(){ PickUpLoop(pickup); });
	}
	
	void PickUpLoop(LivingObject sender)
	{
		foreach(PickUpSpawn pickUpSpawn in config.pickUpSpawnList)
		{
			if(pickUpSpawn.name == sender.name.Replace("_Prefab(Clone)", ""))
			{
				Vector2 center = Functions.RectCenter(GameManager.player.sceneCamera.Constrains);
				float x = center.x;

				if(((PickUp)sender).pickUpType == PickUpType.GunSpread && GameManager.player.weaponType == WeaponType.DoubleBlaster)
					x += 100;
					
				Transform spawn = (Transform)Instantiate(sender.transform,
					new Vector3(x, layerHeight, GameManager.player.transform.position.z) + pickUpSpawn.startPosition, Quaternion.identity);

				spawn.parent = powerUps.transform;
				spawn.name = sender.name;

				PickUp pickUp = spawn.GetComponent<PickUp>();
				pickUp.OnPickUp += OnPickUp;
				pickUp.OnPickUpFailed += OnPickUpFailed;

				int index = pickUpList.IndexOf((PickUp)sender);
				if(index >= 0)
					pickUpList[index] = pickUp;

				break;
			}
		}
		
		Destroy(sender.gameObject);
	}

	public void OnWaveWiped(Wave sender)
	{
		if(bossActive)
			return;
	
		int index = waveList.IndexOf(sender);
		if(index >= 0)
			waveList[index] = null;	
		
		if(WavesWiped) 
		{
			if(config.wavesLoopCount > 0)
			{
				GameManager.timer.Add("Level-WaveLoader-" + config.wavesLoopCount, 4, false, false,
					delegate()
					{
						SpawnEnemies(new Vector3(0, 0, GameManager.player.transform.position.z - 5));// + 
							//(sender.pattern == WavePatterns.Vertical || sender.pattern == WavePatterns.VerticalWave ? 5 : 0)));
					}
				);

				config.wavesLoopCount--;
			}
			else
			{
				if(guiScript != null)
					guiScript.SetNewBossNotificationEnabled(true, firstBoss);
					
				firstBoss = false;

				SpawnBoss();
			}
		}
	}

	void loadNextLevel()
	{
		GameManager.player.config.onRails = true;
		GameManager.player.sceneCamera.config.followPlayer = true;
		
		guiScript.SetNextStageEnabled(true, false);

		//newLevelStartZ = GameManager.player.transform.position.z + 25;

		GameManager.timer.Add("Level-StartNewLevel", 5, false, false, Start);
	}

	public void OnBossDied(LivingObject boss)
	{
		GameManager.instance.levelID++;
		bossActive = false;

		float pauseTime = 0;
		Detonator detonator = boss.explosion.GetComponent<Detonator>();
		if(detonator != null)
			pauseTime = detonator.duration;

		if(GameManager.instance.levelID < GameManager.maxLevelID)
			GameManager.timer.Add("Level-LoadNext-" + (GameManager.instance.levelID - 1), pauseTime, false, false, loadNextLevel);
		else
			GameManager.timer.Add("Level-GameComplete", pauseTime * 3, false, false, GameComplete);
	}

	void GameComplete()
	{
		if(!GameManager.instance.pickedUpPowerUp)
			GameManager.instance.config.achievements[AchievementTypes.DieHard].value = 1;

		if(!GameManager.instance.lostLife)
			GameManager.instance.config.achievements[AchievementTypes.Invincible].value = 1;

		GameManager.instance.config.achievements[AchievementTypes.DownToEarth].value = 1;
		
		CameraFade cam = Camera.main.GetComponent<CameraFade>();
		cam.OnFadeOutCompleted += FadeOutCompleted;
		cam.FadeOut();
	}

	void FadeOutCompleted()
	{
		GameManager.instance.playerDied = false;
		Application.LoadLevel("Results");
	}

	void Update()
	{
		if(waterObject != null)
		{
			waterObject.transform.position = new Vector3(waterObject.transform.position.x,
				waterObject.transform.position.y, GameManager.player.sceneCamera.transform.position.z);
		}
		if(waterCameraObject != null)
		{
			waterCameraObject.transform.position = new Vector3(GameManager.player.sceneCamera.transform.position.x,
				waterCameraObject.transform.position.y, GameManager.player.sceneCamera.transform.position.z);
		}

		// Blocks //////////////////////////////////////////////////////////////////
		float nextBlockPositionZ = (blockLineIndex * (blockSize.z - 0.2f));
		if(config.spawnBlocks && GameManager.player.transform.position.z >= (nextBlockPositionZ - blockSpawnTreshhold))
		{
			List<BlockClass> blockLine = blockMatrix[blockLineIndex % blockMatrix.Count];
			for(int i = 0; i < blockLine.Count; i++)
			{
				if(blockLine[i] == null)
					continue;

				Transform t = (Transform)Instantiate(blockLine[i].transform, new Vector3(i * (blockSize.x - 0.2f), 0, nextBlockPositionZ),
					Quaternion.AngleAxis(blockLine[i].rotation, Vector3.up));

				t.parent = blocks.transform;
			}

			if(!config.spawnTurrets)
			{
				foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Turret"))
					Destroy(obj);
			}
			else
			{
				foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Turret"))
					obj.GetComponent<Turret>().roundsPerMinute = config.turretRoundsPerMinute;
			}

			blockLineIndex++;
		}

		DoCleanup();
	}

	void OnDestroy()
	{
		GameManager.timer.RemoveContaining("Level");
	}

	void DoCleanup()
	{
		if(GameManager.player == null)
			return;

		Rect rect = GameManager.player.sceneCamera.Constrains;

		foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
		{
			if(enemy.transform.position.z < rect.y - maximumEnemyDistanceBehindPlayer)
				enemy.GetComponent<LivingObject>().Die(null);
		}

		foreach(GameObject bullet in GameObject.FindGameObjectsWithTag("Projectile"))
		{
			if(bullet.GetComponent<Projectile>().ignoreCleaner)
				continue;

			if(bullet.transform.position.z < rect.y - maximumEnemyDistanceBehindPlayer || bullet.transform.position.z > (rect.height + 1))
				Destroy(bullet);
		}

		foreach(GameObject pickUp in GameObject.FindGameObjectsWithTag("PickUp"))
		{
			if(pickUp.transform.position.z < rect.y - maximumEnemyDistanceBehindPlayer)
				pickUp.GetComponent<PickUp>().Die(null);
		}

		foreach(GameObject block in GameObject.FindGameObjectsWithTag("Block"))
		{
			if(block.transform.position.z < rect.y - maximumDistanceBehindPlayer)
				Destroy(block);
		}

		foreach(GameObject destroyable in GameObject.FindGameObjectsWithTag("Destroyable"))
		{
			if(destroyable.transform.position.z < rect.y - maximumDistanceBehindPlayer)
				Destroy(destroyable);
		}
	}
}