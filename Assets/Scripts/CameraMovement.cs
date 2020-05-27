using UnityEngine;
using System;
using System.Collections;


public class CameraMovement : MonoBehaviour {

	public float turnSpeed = 4.0f;
 
	public GameObject target;
	private float targetDistance;
	float targetDistanceInitial;
	float speedSmoothVelocity = 1f;
	Vector3 switchSmoothVelocity = Vector3.zero;
	Vector3 attackModeOffset = Vector3.zero;
	Vector3 targetAttackModeOffset = Vector3.zero;
	
	
	public float minTurnAngle = -90.0f;
	public float maxTurnAngle = 0.0f;
	private float rotX;

	MoveHeinz playerScript;
	bool flying;
	
	void Start ()
	{
	    targetDistance = Vector3.Distance(transform.position, target.transform.position);
		targetDistanceInitial = targetDistance;
		GameObject thePlayer = GameObject.Find("paris");
        playerScript = thePlayer.GetComponent<MoveHeinz>();
		//playerScript.OnAttackModeSwitch += switchAttackMode;
	}
	
	void LateUpdate ()
	{
		if(playerScript.flying){
			flyCam();
		}else{
			runCam();
		}

	}

	void runCam(){
		flying = playerScript.flying;
	    // get the mouse inputs
	    float y = Input.GetAxis("Mouse X") * turnSpeed;
	    rotX += Input.GetAxis("Mouse Y") * turnSpeed;
	
	    // clamp the vertical rotation
	    rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);
		targetDistance = Mathf.SmoothDamp (targetDistance, targetDistanceInitial*(playerScript.attackMode?0.7f:1), ref speedSmoothVelocity, 0.1f);
	
	    // rotate the camera
	    transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + y, 0);
	
	    // move the camera position
		attackModeOffset = Vector3.SmoothDamp(attackModeOffset, playerScript.attackMode?transform.right:Vector3.zero,ref switchSmoothVelocity,0.1f);
		transform.position = (target.transform.position)+attackModeOffset - (transform.forward * targetDistance);

	}

	void flyCam(){
		flying = playerScript.flying;
		targetAttackModeOffset = Vector3.zero;
	    // get the mouse inputs
	    float y = Input.GetAxis("Mouse X") * turnSpeed;
	    rotX += Input.GetAxis("Mouse Y") * turnSpeed;

		targetDistance = Mathf.SmoothDamp (targetDistance, targetDistanceInitial*(1+playerScript.currentSpeed/playerScript.flySpeed), ref speedSmoothVelocity, 0.1f);
	
		// clamp the vertical rotation
	    rotX = Mathf.Clamp(rotX, minTurnAngle, 60f);
	
	    // rotate the camera
	    transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + y, 0);
	
	    // move the camera position
	    attackModeOffset = Vector3.SmoothDamp(attackModeOffset, targetAttackModeOffset,ref switchSmoothVelocity, 0.1f);
		transform.position = (target.transform.position)+attackModeOffset - (transform.forward * targetDistance);
	}
	
	void switchAttackMode(){
		if(playerScript.attackMode){
			targetAttackModeOffset = target.transform.right;
		}else{
			targetAttackModeOffset = Vector3.zero;
		}
	}
}