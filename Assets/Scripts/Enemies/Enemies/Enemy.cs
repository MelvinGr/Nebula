using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : LivingObject
{	
	public AudioClip shootSound;
	public AudioClip exposionSound;
	public AudioClip collisionSound;
	
	/*float BounceRate = 2f;
	float BounceSync = -0.75f;
	float smooth = 5.0f;
	float JiggleWidth = 3;*/
	
	public float randomMoveSpeed = 0;

	public Transform fireBall;
	public float fireProbability = 25;

	[HideInInspector]
	public List<Transform> gunTransforms = new List<Transform>();

	public float turnSpeed = 150f;
	public bool lookAtPlayer = false;
	
	public Color bulletColor = Color.red;
	
	public bool canDie = false;
	
	bool PlayerIsInRange()
	{
		return (transform.localPosition.z - GameManager.player.sceneCamera.Constrains.yMin) <= 0;
	}
	
	public override void Awake()
	{
		base.Awake();
		
		gunTransforms = Functions.FindChildTransformsRecruisive(transform, "gun_ref", true);
		
		Detonator detonator = explosion.GetComponent<Detonator>();
		if(detonator != null)
		{
			detonator.size = 2;
			detonator.duration = 0.5f;
		}
			
		fireProbability = 6;// + transform.localScale.x;
		randomMoveSpeed = transform.localScale.x + UnityEngine.Random.Range(0, 3);
	}

	void Update()
	{
		/*if(state == EnemyState.Idle)
		{
			float t = Time.time * BounceRate + (transform.position.x + transform.position.y) / 2 * BounceSync;
			Quaternion target = Quaternion.Euler(0, transform.rotation.y, Mathf.Cos(t) * JiggleWidth);
			transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
		}*/

		if(Time.timeScale <= 0)
			return;

		if(health <= 0)
		{
			Die(GameManager.player);
			return;
		}

		if(lookAtPlayer)
		{
			Vector3 direction = (GameManager.player.transform.position - transform.position).normalized;
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(-direction), turnSpeed * Time.deltaTime);
		}

		if(PlayerIsInRange())
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, -transform.forward, out hit))
			{
				if(hit.transform.tag == "Player")
				{
					foreach(Transform gun in gunTransforms)
					{
						if(Random.Range(0, Mathf.RoundToInt(fireProbability * (lookAtPlayer ? 10 : 1) + (name.Contains("Enemy6") ? 2 : 0))) == 0)
						{
							Transform inst = (Transform)Instantiate(fireBall, transform.position + gun.localPosition, Quaternion.identity);
							inst.forward = -transform.forward;
							
							/*if(transform.localScale.x > 1)
								inst.localScale += transform.localScale;
							else if(transform.localScale.x < 1)
								inst.localScale -= transform.localScale;
							*/
							inst.localScale *= 1.3f;
							
							Projectile proj = inst.GetComponent<Projectile>();
							proj.originObject = transform;
							proj.projectileColor = bulletColor;
							
							//audio.PlayOneShot(shootSound);
							Functions.PlayAudioClip(inst.position, shootSound);
						}
					}
				}
			}
		}
	}

	void OnTriggerEnter(Collider otherObject)
	{
		if(otherObject.tag == "Player")
		{
			//health -= GameManager.GetCollissionDamage(this, otherObject.GetComponent(Player));
			Die(GameManager.player);
			//GameManager.instance.playerScore += 50;
		}
		else if(otherObject.tag == "Projectile")
		{
			Projectile bullet = otherObject.GetComponent<Projectile>();
			if(bullet != null)
			{
				if(bullet.originObject != null)
				{
					if(bullet.originObject.GetComponent<Player>() != null)
					{
						health -= 1;//Functions.GetBulletDamage(this, bullet);
						hitIndicator.hitIndicatorRemaining = 0.2f;
						
						Functions.PlayAudioClip(transform.position, collisionSound);
						bullet.Die();
					}
				}			
			}
		}
	}

	public override void Die(LivingObject sender)
	{
		if(!canDie)
			return;
			
		if(sender != null && sender.GetType() == typeof(Player))
		{
			GameManager.instance.config.achievements[AchievementTypes.ScrapSlayer].value++;
			GameManager.instance.config.achievements[AchievementTypes.MasterOfTheSkies].value++;
			
			/*if(GameManager.player.weaponType == WeaponType.ProtonBlaster)
				GameManager.instance.config.achievements[AchievementTypes.ProtonBlaster].value++;
			else if(GameManager.player.weaponType == WeaponType.DoubleBlaster)
				GameManager.instance.config.achievements[AchievementTypes.OverAchiever].value++;
			*/
			GameManager.instance.playerScore += 25;// * ((int)heightState + 1);

			if(GameManager.instance.config.gameOptions.effectEnabled)
				Instantiate(explosion, transform.position, transform.rotation);
		}
		
		if(sender != null)
			Functions.PlayAudioClip(transform.position, exposionSound);
		
		Destroy(gameObject);

		base.Die(sender);
	}
}