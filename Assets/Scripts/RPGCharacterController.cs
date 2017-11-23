using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGCharacterController : MonoBehaviour {
	#region Public Variables
	public string m_moveStatus = "idle";
	public bool m_walkByDefault = true;
	public float m_gravity = 20.0f;
	
	public float m_jumpSpeed = 8.0f;
	public float m_runSpeed = 10.0f;
	public float m_walkSpeed = 4.0f;
	public float m_turnSpeed = 250.0f;
	public float m_moveBackwardMultiplier = 0.75f;
	public HitBox m_weaponHitBox;
	#endregion
	#region Private Variables
	private float m_speedMultiplier = 0.0f;
	private bool m_grounded = false;
	private Vector3 m_moveDirection = Vector3.zero;
	private bool m_isWalking = false;
	private bool m_isJumping  = false;
	private bool m_mouseSideDown = false;
	private CharacterController m_controller;
	private Animator m_animationController;
	private int m_attackState;
	#endregion

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		m_controller = GetComponent<CharacterController>();
		m_animationController = GetComponent<Animator>();
		m_attackState = Animator.StringToHash("UpperTorso.attack");
	}
	
	// Update is called once per frame
	void Update () {
		m_moveStatus = "idle";
		m_isWalking = m_walkByDefault;

		if(Input.GetAxis("Run") != 0){
			m_isWalking = !m_walkByDefault;
		}

		if(m_grounded){

			if(Input.GetMouseButton(1)){
				m_moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			}
			else{
				m_moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
			}

			if(Input.GetButtonDown("Toggle Move")){
				m_mouseSideDown = !m_mouseSideDown;
			}

			if(m_mouseSideDown && (Input.GetAxis("Vertical") == 0 || Input.GetButton("Jump")) 
				|| Input.GetMouseButton(0) && Input.GetMouseButton(1)){
				m_mouseSideDown = false;
			}

			if((Input.GetMouseButton(0) && Input.GetMouseButton(1)) || m_mouseSideDown){
				m_moveDirection.z = 1;
			}

			if(!Input.GetMouseButton(1) && Input.GetAxis("Horizontal") == 0){
				m_moveDirection.x -= Input.GetAxis("Strafing");
			}

			if(((!Input.GetMouseButton(1) && Input.GetAxis("Horizontal") == 0)
				|| Input.GetAxis("Strafing") != 0) && Input.GetAxis("Vertical") != 0){
				if(m_moveDirection.magnitude > 1){
					m_moveDirection.Normalize();
				}
			}

			if((!Input.GetMouseButton(1) && Input.GetAxis("Horizontal") != 0 ) 
				|| Input.GetAxis("Strafing") != 0 || Input.GetAxis("Vertical") < 0){
				m_speedMultiplier = m_moveBackwardMultiplier;
			}
			else{
				m_speedMultiplier = 1f;
			}

			m_moveDirection *= m_isWalking ? m_walkSpeed * m_speedMultiplier : m_runSpeed * m_speedMultiplier;

			if(Input.GetButton("Jump")){
				m_isJumping = true;
				m_moveDirection.y = m_jumpSpeed;
			}

			if(m_moveDirection.magnitude > 0.05f){ //TODO: Magic number
				m_animationController.SetBool("isWalking", true);
			} else {
				m_animationController.SetBool("isWalking", false);
			}

			m_animationController.SetFloat("Speed", m_moveDirection.z);
			m_animationController.SetFloat("Direction", m_moveDirection.x);

			m_moveDirection = transform.TransformDirection(m_moveDirection);
		}

		if(Input.GetMouseButton(1)){
			transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
		} else {
			transform.Rotate(0, Input.GetAxis("Horizontal") * m_turnSpeed * Time.deltaTime, 0);
		}

		m_moveDirection.y -= m_gravity * Time.deltaTime;

		m_grounded = (m_controller.Move(m_moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;

		m_isJumping = m_grounded ?  false : m_isJumping;

		if(m_isJumping){
			m_moveStatus = "jump";
		}
		AnimatorStateInfo currentUpperTorsoState = m_animationController.GetCurrentAnimatorStateInfo(1);
		if(currentUpperTorsoState.fullPathHash == m_attackState){
			m_weaponHitBox.enabled = true; 
		} else{
			if(Input.GetButtonDown("Attack")){
				m_animationController.SetBool("isAttacking", true);
			} else{
				m_animationController.SetBool("isAttacking", false);
				m_weaponHitBox.enabled = false;
			}
		}
	}
}
