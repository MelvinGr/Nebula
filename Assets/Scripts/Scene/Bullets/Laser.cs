using UnityEngine;
using System.Collections;

public class Laser : Projectile
{		
	public float maxScale = 1;
	
	public override void Awake()
	{
		base.Awake();
		
		projectileColor = projectileColor; //reset
		transform.localScale = new Vector3(1, 1, 0);
	}		
	
	public override void Update()
	{
		base.Update();		
			
		if(transform.localScale.z >= maxScale)
		{
			float amountToMove = bulletSpeed * Time.deltaTime;
			transform.Translate(transform.forward * amountToMove, Space.World);
		}
		else
		{
			//float amountToScale = bulletSpeed * Time.deltaTime;
			transform.localScale += new Vector3(0, 0, 0.1f);			
		}	
	}
}