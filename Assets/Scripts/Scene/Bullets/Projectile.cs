using UnityEngine;
using System.Collections;

public abstract class Projectile : MonoBehaviour
{
	public Transform explosion;
	public AudioClip explosionSound;

	public float bulletSpeed = 0;

	[HideInInspector]
	public Transform originObject;

	public int maxLifeTime = 10;

	[HideInInspector]
	public float spawnTime = 0;
	
	public bool ignoreCleaner = false;
	
	bool adjustedHeight = false;
	
	Color _projectileColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
	public Color projectileColor
	{
		get { return _projectileColor; }
		set
		{	
			_projectileColor = value;
			foreach(MeshRenderer r in transform.GetComponentsInChildren<MeshRenderer>())
				r.material.SetColor("_TintColor", _projectileColor);
		}
	}
	
	static GameObject parentObject;

	public virtual void Awake()
	{
		if(parentObject == null)
		{
			parentObject = new GameObject("Projectiles");
			parentObject.transform.position = Vector3.zero;
		}
		
		transform.parent = parentObject.transform;
		
		spawnTime = Time.time;
		projectileColor = projectileColor; //reset
	}

	public virtual void Update()
	{					
		if(!adjustedHeight && originObject != null && originObject.GetComponent<Turret>() == null)
		{
			if(this.GetType() != typeof(HomingRocket))		
				transform.position = new Vector3(transform.position.x, GameManager.level.layerHeight, transform.position.z);
				
			adjustedHeight = true;
		}
		
		if(Mathf.RoundToInt(Time.time - spawnTime) >= maxLifeTime)
			Die();
	}

	public virtual void Die()
	{
		//audio.PlayOneShot(explosionSound);
		
		if(explosion != null && GameManager.instance.config.gameOptions.effectEnabled)
			Instantiate(explosion, transform.position, transform.rotation);
		
		Destroy(gameObject);
	}
}