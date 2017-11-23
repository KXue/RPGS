using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	int m_maxHP = 100;
	public float m_deathTime = 3f;
	public float m_hitReact = 0.1f;
	private int m_currentHealth;
	private Animator m_animController;
	private float m_hitDelay;
	private Transform m_aggroTarget;
	public Transform AggroTarget{
		get{
			return m_aggroTarget ? m_aggroTarget : null;
		}
		set{
			m_aggroTarget = value;
		}
	}
	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		m_animController = GetComponent<Animator>();
		m_currentHealth = m_maxHP;
	}

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		if(m_hitDelay <= 0){
			m_animController.SetBool("tookDamage", false);
		} else{
			m_hitDelay -= Time.deltaTime;
		}

		if(m_currentHealth <= 0){
			Die();
		}
	}
	public void ApplyDamage(int amount){
		m_currentHealth -= amount;
		if(m_currentHealth <= 0){
			m_hitDelay = m_deathTime;
			m_animController.SetBool("died", true);
		}else{
			m_hitDelay = m_hitReact;
			m_animController.SetBool("tookDamage", true);
		}
	}
	private void Die(){
		if(m_hitDelay <= 0){
			Destroy(gameObject);
		}
	}
}
