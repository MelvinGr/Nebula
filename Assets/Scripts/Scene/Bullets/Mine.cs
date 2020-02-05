using UnityEngine;
using System.Collections;

public class Mine : Projectile
{		
	public override void Update()
	{
		base.Update();		
		
		//transform.position += (Random.insideUnitSphere / 10);
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Mathf.Sin(Time.time * 3) / 5);
	}
}