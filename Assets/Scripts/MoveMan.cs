using UnityEngine;
using System.Collections;

public class MoveMan : MonoBehaviour {

	public float walkSpeed = 2;
	public float runSpeed = 6;
	public int gravity = 8;

	public float turnSmoothTime = 0.2f;
	float turnSmoothVelocity;
	bool walking = false;

	int moveMode;

	public float speedSmoothTime = 0.1f;
	float speedSmoothVelocity;
	float currentSpeed;
    Vector3 moveVec;
	Vector2 inputDir;
	float jumpTimeCounter;
	float jumpTime=1;
	float attackFrameCounter;
	bool isJumping;
	bool isAttacking = false;

	CharacterController controller;
	Animator animator;
	Transform cameraT;

	void Start () {
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator> ();
		cameraT = Camera.main.transform;
	}

	void Update () {
		//walking
		moveMode = 0;
		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		inputDir = input.normalized;
		if (inputDir != Vector2.zero) {
			float targetRotation = Mathf.Atan2 (inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
			moveMode = 3;
		}
		walking = Input.GetKey (KeyCode.LeftShift);
		if(walking){
			moveMode/=3;
		}

		float targetSpeed = ((walking) ? walkSpeed : runSpeed) * inputDir.magnitude;
		currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
        moveVec = (transform.forward * currentSpeed)+(transform.up*moveVec.y);
		//jumping
		if(controller.isGrounded&&Input.GetKey(KeyCode.Space)){
			moveVec.y = 0;
			isJumping = true;
			jumpTimeCounter = jumpTime;
			moveVec += transform.up*3.0f;
		}
        if(Input.GetKey(KeyCode.Space)&&isJumping){
			if(jumpTimeCounter>0){
				moveVec+=transform.up*1.5f*jumpTimeCounter*Time.deltaTime;
				jumpTimeCounter -= Time.deltaTime;
			}else{
				isJumping = false;
			}
        }
		if(transform.position.y>=3){
			animator.SetFloat("jumpspeed",0);
		}
		if(Input.GetKeyUp(KeyCode.Space)){
			isJumping = false;
		}
		if(!controller.isGrounded){
			moveMode = 2;
		}else{
			animator.SetFloat("jumpspeed",1);
		}
		//movement handling
        moveVec.y-=gravity*Time.deltaTime;
		controller.Move(moveVec* Time.deltaTime);
		print(moveVec.y);
		float animationSpeedPercent = ((!walking) ? 3 : 1.5f) * inputDir.magnitude;
		animator.SetInteger ("move", moveMode);
		//attacking
		if(Input.GetMouseButtonDown(0)){
			isAttacking = true;
			animator.SetInteger("attack",1);
			attackFrameCounter = 0.5f;
		}
		if(attackFrameCounter<0){
			animator.SetInteger("attack",0);
			isAttacking = false;
		}
		if(isAttacking){
			attackFrameCounter-=Time.deltaTime;
		}
	}
}