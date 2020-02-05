using UnityEngine;
using System.Collections;

public class HomingRocket : Projectile
{
	public Transform target;
	public float rocketTurnSpeed = 90.0f;
	public float rocketSpeed = 10.0f;
	public float turbulence = 10.0f;

	public override void Update()
	{
		base.Update();		
			
		Vector3 direction = ((target.position - transform.position) + (Random.insideUnitSphere * turbulence)).normalized;
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rocketTurnSpeed * Time.deltaTime);
		transform.Translate(Vector3.forward * rocketSpeed * Time.deltaTime);
	}
}