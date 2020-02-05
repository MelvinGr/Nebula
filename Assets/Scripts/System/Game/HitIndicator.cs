using UnityEngine;
using System.Collections;

public class HitIndicator : MonoBehaviour
{
	public float hitIndicatorRemaining = 0;
	public Color hitColor = Color.red;
	
	public bool blink = false;
	
	void Update()
	{
		if(hitIndicatorRemaining > 0)
		{
			foreach(MeshRenderer r in transform.GetComponentsInChildren<MeshRenderer>())
			{
				if(blink)
				{
					float value = Mathf.PingPong(Time.time, 1f);
				
					r.material.color = new Color(value, hitColor.g, hitColor.b);
				}
				else
					r.material.color = hitColor;
			}
			
			hitIndicatorRemaining -= Time.deltaTime;
		}
		else if(hitIndicatorRemaining != 0)
		{
			foreach(MeshRenderer r in transform.GetComponentsInChildren<MeshRenderer>())
				r.material.color = new Color(0.5f, 0.5f, 0.5f);
				
			hitIndicatorRemaining = 0;
		}
	}
}
