using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[Serializable]
public class WaveSpawn : ICloneable
{
	public string name;
	public Vector3 startPosition;
	
	object ICloneable.Clone()
	{
		return this.MemberwiseClone();
	}
}

public class Wave : MonoBehaviour
{
	public WavePatterns pattern;
	
	public int enemyMultiplyCount;
	public float enemyMoveSpeed;
	
	public Transform[] enemyPrefabs;
	
	public OnWaveWipedDelegate OnWaveWiped;
	
	[HideInInspector]
	public List<Enemy> enemyList = new List<Enemy>();
	
	//WaveSpawn _waveSpawn;
	public WaveSpawn waveSpawn
	{
		get
		{
			//if(_waveSpawn == null)
			//{
				WaveSpawn _waveSpawn = new WaveSpawn();
				_waveSpawn.name = transform.name.Substring(0, transform.name.IndexOf("_Prefab"));
				_waveSpawn.startPosition = transform.position;
			//}
			
			return _waveSpawn;
		}
		
		//set
		//{
		//	_waveSpawn = value;
		//}
	}
	
	bool isResetting = false;
	public bool lookAtPlayer = false;
	
	// incomming
	public float incommingStartDistance = 30;
	
	// circular
	public float circularRotationSpeed = 1;
	public float circularRadius = 3;
	float circularTheta = 0;
	//public bool circularMoveToCenter = true;
	//float circularRadiusMoveToCenterAdjustment = 0;

	public bool IsWiped
	{
		get 
		{
			foreach(Enemy enemy in enemyList)
			{
				if(enemy != null)
					return false;
			}

			return true;
		}
	}

	public void OnEnemyDied(LivingObject sender)
	{
		if(isResetting)
			return;
			
		int index = enemyList.IndexOf((Enemy)sender);
		if(index >= 0)
			enemyList[index] = null;
		
		if(IsWiped)
		{
			if(OnWaveWiped != null)		
			{			
				OnWaveWiped(this);
				OnWaveWiped = null;
			}
				
			Destroy(gameObject);			
		}
	}
	
	Enemy InstantiateEnemy(Transform enemyTrans)
	{
		Transform trans = (Transform)Instantiate(enemyTrans, Vector3.zero, Quaternion.identity);			
		trans.parent = transform;
		//trans.position = Vector3.zero;
		
		Enemy enemy = trans.GetComponent<Enemy>();
		enemy.OnObjectDied += OnEnemyDied;			
		enemy.lookAtPlayer = false;		
		enemyList.Add(enemy);
		
		return enemy;
	}
	
	void Start()
	{
		Wipe(true);
		enemyList.Clear();
		isResetting = false;

		Rect rect = GameManager.player.sceneCamera.Constrains;
		
		switch(pattern)
		{
			case WavePatterns.InComming:
			{
				float[] offsets = {rect.xMin * 0.75f, rect.width * 1.25f};
				float[] x = { offsets[0], offsets[1] };
				
				for(int i = 0; i < enemyMultiplyCount; i++)
				foreach(Transform enemyTrans in enemyPrefabs)
				{
					if(enemyTrans == null)
						continue;			
					
					Enemy enemy = InstantiateEnemy(enemyTrans);
					
					int lIndex = (enemyList.IndexOf(enemy) < enemyList.Count / 2 ? 0 : 1);
					enemy.spawnPosition = new Vector3(x[lIndex], 0, 0);							
					enemy.canDie = true;
					
					x[lIndex]++;
				}
				
				break;
			}
			
			case WavePatterns.Horizontal:
			{
				float x = 0;//transform.position.z;
				
				for(int i = 0; i < enemyMultiplyCount; i++)				
				foreach(Transform enemyTrans in enemyPrefabs)
				{
					if(enemyTrans == null)
						continue;

					Enemy enemy = InstantiateEnemy(enemyTrans);
					enemy.spawnPosition = new Vector3(x, 0, 0);				
					enemy.canDie = true;				

					x += 1.5f;
				}
				
				break;
			}
			
			case WavePatterns.Vertical:
			{
				float z = 0;//transform.position.z;
				
				for(int i = 0; i < enemyMultiplyCount; i++)				
				foreach(Transform enemyTrans in enemyPrefabs)
				{
					if(enemyTrans == null)
						continue;

					Enemy enemy = InstantiateEnemy(enemyTrans);
					enemy.spawnPosition = new Vector3(0, 0, z);				
					enemy.canDie = true;				

					z += 1.5f;
				}
				
				break;
			}
			
			case WavePatterns.HorizontalWave:
			{
				float x = 0;//transform.position.z;
				
				for(int i = 0; i < enemyMultiplyCount; i++)				
				foreach(Transform enemyTrans in enemyPrefabs)
				{
					if(enemyTrans == null)
						continue;

					Enemy enemy = InstantiateEnemy(enemyTrans);
					enemy.spawnPosition = new Vector3(x, 0, 0);				
					enemy.canDie = true;				

					x += 1.5f;
				}
				
				break;
			}
			
			case WavePatterns.VerticalWave:
			{
				float z = 0;//transform.position.z;
				
				for(int i = 0; i < enemyMultiplyCount; i++)				
				foreach(Transform enemyTrans in enemyPrefabs)
				{
					if(enemyTrans == null)
						continue;

					Enemy enemy = InstantiateEnemy(enemyTrans);
					enemy.spawnPosition = new Vector3(0, 0, z);				
					enemy.canDie = true;				

					z += 1.5f;
				}
				
				break;
			}
			
			case WavePatterns.Circle:
			{					
				for(int i = 0; i < enemyMultiplyCount; i++)
				foreach(Transform enemyTrans in enemyPrefabs)
				{
					if(enemyTrans == null)
						continue;

					Enemy enemy = InstantiateEnemy(enemyTrans);
					enemy.spawnPosition = Vector3.zero;			
					enemy.canDie = true;
				}
				
				break;
			}
			
			case WavePatterns.VShapeKamikaze:
			{	
				int enemyCounter = 0;
				int row = 0;
				
				for(int i = 0; i < enemyMultiplyCount; i++)				
				foreach(Transform enemyTrans in enemyPrefabs)
				{
					if(enemyTrans == null)
						continue;						
					
					int x = (enemyCounter % 2 == 0 ? -row : row);

					Enemy enemy = InstantiateEnemy(enemyTrans);
					enemy.spawnPosition = new Vector3(x, 0, row);				
					enemy.canDie = true;	
					
					if(enemyCounter == 0)
						enemyCounter = 1;
					
					enemyCounter++;
					if(enemyCounter % 2 == 0)
						row++;
				}
				
				break;
			}
		}			
	}

	void Update()
	{			
		if(Time.timeScale <= 0)
			return;
			
		if(pattern == WavePatterns.Circle)
			circularTheta += circularRotationSpeed;					
		
		int index = 0;
		foreach(Enemy enemy in enemyList)
		{			
			if(enemy == null)
			{
				index++;
				continue;
			}
	
			switch(pattern)
			{
				case WavePatterns.InComming:
				{
					Vector3 target = Functions.ScreenCenterInWorldSpace(GameManager.player.sceneCamera, GameManager.player.transform.position.y) + 
						new Vector3(0, 0, 10);				
					
					if((transform.position.z - target.z) <= incommingStartDistance)
					{
						if (Vector3.Distance(enemy.transform.position, target) >= 1 && !GameManager.player.isGoingDown)
						{
							enemy.transform.position += Functions.Direction(enemy.transform.position, target) * (enemyMoveSpeed * 100) * Time.deltaTime;
						}
						else
						{
							enemy.transform.position += Functions.Direction(enemy.transform.position, enemy.spawnPosition) * 
								(enemyMoveSpeed * 100) * Time.deltaTime;
						}
					}
					
					break;
				}
				
				case WavePatterns.Horizontal:
				{
					//transform.position += new Vector3(0, 0, -enemyMoveSpeed * Time.deltaTime);
					
					break;
				}
				
				case WavePatterns.Vertical:
				{
					//transform.position += new Vector3(0, 0, -enemyMoveSpeed * Time.deltaTime);
					
					break;
				}
				
				case WavePatterns.HorizontalWave:
				{
					float zBounce = Mathf.Sin(Time.time + enemy.transform.position.x * 0.1f) * 0.1f;
					enemy.transform.position += new Vector3(0, 0, zBounce);
					
					break;
				}
				
				case WavePatterns.VerticalWave:
				{
					float xBounce = Mathf.Sin(Time.time + enemy.transform.position.z * 0.1f) * 0.1f;
					enemy.transform.position += new Vector3(xBounce, 0, 0);
					
					break;
				}

				case WavePatterns.Circle:
				{
					float localTheta = circularTheta + (360f / enemyList.Count * index);
					
					float localRadius = circularRadius;
					/*if(circularMoveToCenter)// && localRadius > 1f)
					{
						localRadius -= circularRadiusMoveToCenterAdjustment;//((index + 1) * circularRadiusMoveToCenterAdjustment);
						
						if(localRadius > circularRadius || localRadius < circularRadius)
							circularRadiusMoveToCenterAdjustment = -circularRadiusMoveToCenterAdjustment;
							
						circularRadiusMoveToCenterAdjustment += 0.01f;
					}*/				

					enemy.lookAtPlayer = lookAtPlayer;						
					
					enemy.transform.position = new Vector3
					(
						transform.position.x + localRadius * Mathf.Cos(localTheta * Mathf.Deg2Rad),							
						enemy.transform.position.y,						
						transform.position.z + localRadius * Mathf.Sin(localTheta * Mathf.Deg2Rad)
					);
					
					break;
				}
				
				case WavePatterns.VShapeKamikaze:
				{
					//
					
					break;
				}
			}
			
			transform.position += new Vector3(0, 0, -enemyMoveSpeed * Time.deltaTime);
			
			index++;
		}
	}

	public void Wipe(bool _isResetting)
	{
		isResetting = _isResetting;
		
		foreach(Enemy enemy in enemyList)
		{
			if(enemy == null)
				continue;

			enemy.Die();
		}
	}
}