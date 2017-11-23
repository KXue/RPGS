using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {
	public int m_minDamage = 1;
	public int m_maxDamage = 5;
	/// <summary>
	/// OnTriggerEnter is called when the Collider other enters the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerEnter(Collider other)
	{
		Health healthScript = other.GetComponent<Health>();
		if(healthScript != null){
			healthScript.ApplyDamage(Random.Range(m_minDamage, m_maxDamage));
			healthScript.AggroTarget = transform;
		}
	}
	
	
}
