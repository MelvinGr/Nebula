using UnityEngine;
using System.Collections;

public class Bullet : Projectile
{	
	public override void Update()
	{
		base.Update();			
			
		float amountToMove = bulletSpeed * Time.deltaTime;
		transform.Translate(transform.forward * amountToMove, Space.World);
	}
}