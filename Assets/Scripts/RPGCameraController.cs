using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGCameraController : MonoBehaviour {
	#region Public Variables
	public Transform m_target;
	public float m_heightOffset = 1.7f;
	public float m_distance = 12.0f;
	public float m_offsetFromWall = 0.1f;
	public float m_minDistance = 0.6f;
	public float m_maxDistance = 20f;
	public float m_xSpeed = 200.0f;
	public float m_ySpeed = 200.0f;
	public float m_yMinLimit = -80.0f;
	public float m_yMaxLimit = 80.0f;
	public float m_zoomSpeed = 5.0f;
	public float m_autoRotationSpeed = 3.0f;
	public LayerMask m_collisionLayers = -1;
	public bool m_alwaysRotateToRearOfTarget = false;
	public bool m_allowMouseInputX = true;
	public bool m_allowMouseInputY = true;
	#endregion
	#region Private Variables
	private float m_xDeg = 0.0f;
	private float m_yDeg = 0.0f;
	private float m_currentDistance;
	private float m_desiredDistance;
	private float m_correctedDistance;
	private bool m_rotateBehind;
	private bool m_mouseSideButton;
	#endregion
	#region Methods

	// Use this for initialization
	void Start () {
		Vector3 angles = transform.eulerAngles;
		m_xDeg = angles.x;
		m_yDeg = angles.y;
		
		Vector3 distance = m_target.transform.position - transform.position;
		m_currentDistance = distance.magnitude;
		m_desiredDistance = m_currentDistance;
		m_correctedDistance = m_currentDistance;
		
		if(m_alwaysRotateToRearOfTarget){
			m_rotateBehind = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	/// <summary>
	/// LateUpdate is called every frame, if the Behaviour is enabled.
	/// It is called after all Update functions have been called.
	/// </summary>
	void LateUpdate()
	{
		if(Input.GetButton("Toggle Move")){
			m_mouseSideButton = !m_mouseSideButton;
		}

		// player moved or interrupted the auto-move
		if(m_mouseSideButton && (Input.GetAxis("Vertical")!= 0 || Input.GetButton("Jump")) 
			|| (Input.GetMouseButton(0) && Input.GetMouseButton(1))){
			m_mouseSideButton = false;
		}
		//if either mousebuttons are down, let the mouse govern camera's position
		if(GUIUtility.hotControl == 0){
			if(Input.GetMouseButton(0) || Input.GetMouseButton(1)){
				//
				if(m_allowMouseInputX){
					m_xDeg += Input.GetAxis("Mouse X") * m_xSpeed * Time.deltaTime;
				}
				else{
					RotateBehindTarget();
				}
				if(m_allowMouseInputY){
					m_yDeg -= Input.GetAxis("Mouse Y") * m_ySpeed * Time.deltaTime;
				}
				if(!m_alwaysRotateToRearOfTarget){
					m_rotateBehind = false;
				}
			} else if(Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 || m_rotateBehind || m_mouseSideButton){
				RotateBehindTarget();
			}
		}

		m_yDeg = ClampAngle(m_yDeg, m_yMinLimit, m_yMaxLimit);
		Quaternion rotation = Quaternion.Euler(m_yDeg, m_xDeg, 0);

		m_desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * m_zoomSpeed * Mathf.Abs(m_desiredDistance);
		m_desiredDistance = Mathf.Clamp(m_desiredDistance, m_minDistance, m_maxDistance);
		m_correctedDistance = m_desiredDistance;

		Vector3 targetOffset = new Vector3(0, -m_heightOffset, 0);
		Vector3 position = m_target.transform.position - (rotation * Vector3.forward * m_desiredDistance + targetOffset);

		//check collision
		RaycastHit collision;
		Vector3 trueTargetPosition = m_target.transform.position + Vector3.up * m_heightOffset;
		bool isCorrected = false;
		if(Physics.Linecast(trueTargetPosition, position, out collision, m_collisionLayers)){
			m_correctedDistance = Vector3.Distance(trueTargetPosition, collision.point) - m_offsetFromWall;
			isCorrected = true;
		}
		if(!isCorrected || m_correctedDistance > m_currentDistance){
			m_currentDistance = Mathf.Lerp(m_currentDistance, m_correctedDistance, Time.deltaTime * m_zoomSpeed);
		}
		else{
			m_currentDistance = m_correctedDistance;
		}
		m_currentDistance = Mathf.Clamp(m_currentDistance, m_minDistance, m_maxDistance);
		position = m_target.transform.position - (rotation * Vector3.forward * m_currentDistance + targetOffset);
		transform.rotation = rotation;
		transform.position = position;
	}
	private void RotateBehindTarget(){
		float targetRotationAngle = m_target.transform.eulerAngles.y;
		float currentRotationAngle = transform.eulerAngles.y;
		m_xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, m_autoRotationSpeed * Time.deltaTime);
		if(targetRotationAngle == currentRotationAngle){
			if(!m_alwaysRotateToRearOfTarget){
				m_rotateBehind = false;
			}
		} else{
			m_rotateBehind = true;
		}
	}
	private float ClampAngle(float angle, float min, float max){
		angle %= 360;
		return Mathf.Clamp(angle, min, max);
	}
	#endregion
}
