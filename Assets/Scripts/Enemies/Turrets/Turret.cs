using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turret : LivingObject
{
	public AudioClip shootSound;

	public Transform fireBall;
	public float roundsPerMinute = 60;
	public float fireRange = 20;
	
	[HideInInspector]
	public List<Transform> gunTransforms = new List<Transform>();

	public float turnSpeed = 150f;
	
	public Color bulletColor = Color.red;
	
	float mod = 0;
	bool didShoot = false;
	
	public Transform targetTransform;
	
	public override void Awake()
	{
		base.Awake();
		
		//turretObject = Functions.FindChildTransformsRecruisive(transform, "TurretObject")[0].gameObject;		
		gunTransforms = Functions.FindChildTransformsRecruisive(transform, "gun_ref", true);
	}
	
	void Start()
	{
		if(GameManager.player != null)
			targetTransform = GameManager.player.transform;
	}

	void OnBecameVisible() 
	{
        enabled = true;
    }
	
    void OnBecameInvisible() 
	{
        enabled = false;
    }
	
	public override void Die(LivingObject sender)
	{
		if(sender != null && sender.GetType() == typeof(Player))
		{
			//
		}
		
		Destroy(gameObject);
	}

	void Update()
	{
		if(Time.timeScale <= 0 || GameManager.player == null)
			return;

		Vector3 simulatedPlayerPos = new Vector3(targetTransform.position.x, transform.position.y, targetTransform.position.z);
		Vector3 direction = Functions.Direction(transform.position, simulatedPlayerPos);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
		
        float angle = Vector3.Angle(simulatedPlayerPos - transform.position, transform.forward);		
		bool bossActive = (GameManager.level != null ? GameManager.level.bossActive : false);
	
		if(Mathf.Abs(transform.position.z - GameManager.player.transform.position.z) <= fireRange && 
			GameManager.player.config.inputEnabled && !bossActive && angle < 60f)
		{		
			mod = Mathf.FloorToInt(Time.time) % ((roundsPerMinute / 60f) + 1);
			if(mod == 0 && !didShoot)
			{
				foreach(Transform gun in gunTransforms)
				{
					direction = Functions.RelativePos(gun.position, GameManager.player.transform.position);					
					Transform inst = (Transform)Instantiate(fireBall, gun.position, Quaternion.LookRotation(direction));
					inst.localScale *= 1.3f;
					
					Projectile proj = inst.GetComponent<Projectile>();
					proj.originObject = transform;
					proj.bulletSpeed = 12;
					proj.projectileColor = bulletColor;
					proj.ignoreCleaner = true;

					if(shootSound != null)
						Functions.PlayAudioClip(inst.position, shootSound);
				}
				
				didShoot = true;
			}
			else if(mod == 1)
				didShoot = false;
		}
	}
}