using UnityEngine;
using System.Collections;

public class TerrorVilleTurret : MonoBehaviour
{	
	TerrorVille terrorVille;
	
	public int health = 0;
	public Transform explosion;
	
	public AudioClip explosionSound;
	public AudioClip collisionSound;
	
	public HitIndicator hitIndicator;

	/*public override */void Awake()
	{
		//base.Awake();
		
		health = 5;
		//spawnHealth = health;
		
		terrorVille = transform.parent.parent.GetComponent<TerrorVille>();
		hitIndicator = gameObject.AddComponent<HitIndicator>();
		
		BoxCollider colider = gameObject.AddComponent<BoxCollider>();
		colider.isTrigger = true;
		colider.size = new Vector3(colider.size.x * 1.5f, colider.size.y * 2f, colider.size.z * 1.5f);
	}		
		
	void Start()
	{	
		terrorVille.RegisterChild(this);
		
		explosionSound = terrorVille.explosionSound;
		collisionSound = terrorVille.explosionSound;
	}

	/*public override */void Die(LivingObject sender)
	{
		if(sender != null && sender.GetType() == typeof(Player))
		{
			GameManager.instance.playerScore += 25;	
			if(GameManager.instance.config.gameOptions.effectEnabled)
				Instantiate(explosion, transform.position, transform.rotation);
		}				
		
		Functions.PlayAudioClip(transform.position, explosionSound);
		terrorVille.UnRegisterChild(this);	
		Destroy(gameObject);	
	}
	
	void OnDestroy() 
	{
		terrorVille.UnRegisterChild(this);
		//GameManager.timer.RemoveContaining(RandomName);
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
	}
	
	void OnTriggerEnter(Collider otherObject)
	{				
		if(otherObject.tag == "Block")
		{
			Die(GameManager.player);
		}		
		else if(otherObject.tag == "Projectile")
		{
			Projectile projectile = otherObject.GetComponent<Projectile>();
			
			if(projectile.originObject != null)
			{		
				if(projectile.originObject.GetComponent<TerrorVilleTurret>() != null || projectile.originObject.GetComponent<TerrorVille>() != null)
					return;
			
				if(projectile.originObject.GetComponent<Player>() != null)
				{	
					if(true)//!(childKind == ThePilarChildKind.Base && !thePilar.IsFinal()))
					{
						health -= 1;//Functions.GetBulletDamage(this, projectile);
						hitIndicator.hitIndicatorRemaining = 0.2f;
					}
					
					Functions.PlayAudioClip(transform.position, collisionSound);
					projectile.Die();
				}				
			}		
		}
		else if(otherObject.tag == "Player")
		{
			health = 0;
			Functions.PlayAudioClip(transform.position, collisionSound);
		}
	}
}