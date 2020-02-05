using UnityEngine;
using System.Collections;

public abstract class LivingObject : MonoBehaviour 
{
	public OnLivingObjectDelegate OnObjectDied = null;
	
	public int health;	
	public Transform explosion;
	
	[HideInInspector]
	public HitIndicator hitIndicator;
	
	[HideInInspector]
	public int spawnHealth;
	
	Vector3 _spawnPosition;
	public Vector3 spawnPosition
	{ 
		get { return _spawnPosition; } 
		set
		{
			_spawnPosition = value;
			transform.localPosition = _spawnPosition;
		}
	}
	
	public float spawnTime { get; private set; }
	public long spawnSystemTime { get; private set; }
	
	static int _spawnCounter;
	public int spawnCounter { get; private set; }
	
	string _randomName = null;
	public string RandomName 
	{
		get 
		{ 
			if(_randomName == null)
			{
				if(this.name.Contains("_Prefab(Clone)"))
					_randomName = this.name.Replace("_Prefab(Clone)", "");
				else if(this.name.Contains("(Clone)"))
					_randomName = this.name.Replace("(Clone)", "");
				else
					_randomName = this.name;
					
				int random = (int)((spawnSystemTime & (long)(Random.value * long.MaxValue)) >> 32);
				
				_randomName += "(" + random + ")";
			}
			
			return _randomName; 
		}
		
		//set { _randomName = value; }
	}
	
	public virtual void Awake()
	{
		spawnHealth = health;
		spawnPosition = transform.position;		
		spawnTime = Time.time;	
		spawnSystemTime = System.DateTime.Now.Ticks;		
		spawnCounter = _spawnCounter++;
		
		hitIndicator = gameObject.GetComponent<HitIndicator>();
		if(hitIndicator == null)
			hitIndicator = gameObject.AddComponent<HitIndicator>();
	}
	
	public virtual void Die()
	{
		Die(null);
	}
	
	public virtual void Die(LivingObject sender)
	{	
		if(OnObjectDied != null)
			OnObjectDied(this);
			
		OnObjectDied = null;
	}
}