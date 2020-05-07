using UnityEngine;
using System;
using System.Collections;

public class MoveHeinz : MonoBehaviour {
	//public vars
	public float walkSpeed = 15;
	public float runSpeed = 30;
	public int gravity = 20;
	public float flySpeed = 100;
	//some move stuff
	public float turnSmoothTime = 0.2f;
	float turnSmoothVelocity;
	float turnSmoothVelocityY;
	bool walking = false;

	float moveMode;
	//move and jump stuff
	public float speedSmoothTime = 0.1f;
	public float force = 40f;
	public float currentSpeed;
	float speedSmoothVelocity = 1f;
    Vector3 moveVec;
	Vector2 inputDir;
	Vector2 inputSmoothDamp;
	float jumpTimeCounter;
	float jumpTime=1;
	bool isJumping;
	public bool flying = false;
	public bool outFly = false;
	int jumpmode;

	//attack stuff
	public bool isAttacking = false;
	float attackFrameCounter;
	Transform arm;
	Transform forearm;
	Transform hand;
	public GameObject wandTip;
	public bool attackMode = false;
	public bool attackValidPrev;
	public event Action OnAttackModeSwitch;
	float armturnvel = 1f;
	float armdeg = 0;
	float armprev = 0;
	int armTurnFrameCounter;
	RaycastHit hit;
	GameObject spell;
	public GameObject currSpell;

	//controller stuff
	CharacterController controller;
	Animator animator;
	public Transform cameraT;

	void Start () {
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator> ();
		arm = transform.Find("Armature").Find("Bone").Find("Bone.007");
		forearm = arm.Find("Bone.008");
		hand = forearm.Find("Bone.009");
		cameraT = Camera.main.transform;
		spell = (GameObject)Resources.Load("Prefabs/BeamPoint");
		
		//animator.speed = 0.5f;
	}

	void LateUpdate () {
		if(flying){
			fly();
		}else{
			jump();
			move();
		}		
		attack();
		
		launchAttack();
		
		print(spell.GetComponent<Beam>().going);
		
		//Debug.DrawRay(wandTip.transform.position, !flying?(cameraT.forward*100):transform.forward*100);
	}

	void move(){
		//walking
		moveMode = 0f;
		jumpmode = 0;
		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		inputDir = Vector2.SmoothDamp(inputDir, input.normalized, ref inputSmoothDamp, !controller.isGrounded?turnSmoothTime:turnSmoothTime*1f*((currentSpeed<5?5:currentSpeed)/runSpeed));
		if (inputDir != Vector2.zero || animator.GetInteger("attack")==2||attackMode) {
			float targetRotation = Mathf.Atan2 (inputDir.x*(((attackMode&&Input.GetAxisRaw("Vertical")<0)?-1:1)), (attackMode)?Mathf.Abs(inputDir.y):inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
			if(input==Vector2.zero){
				targetRotation = transform.eulerAngles.y;
			}
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, controller.isGrounded?turnSmoothTime:turnSmoothTime*1f*((currentSpeed<5?5:currentSpeed)/runSpeed));
			moveMode = 1f;
		}
		walking = Input.GetKey (KeyCode.LeftShift);
		if(walking){
			moveMode/=2;
		}

		float targetSpeed = ((walking) ? walkSpeed : runSpeed) * inputDir.magnitude;
		currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, controller.isGrounded?speedSmoothTime:5f);
        moveVec = ((attackMode&&!outFly?(Quaternion.AngleAxis(cameraT.eulerAngles.y, Vector3.up)*new Vector3(inputDir.x,0,inputDir.y)):transform.forward) * currentSpeed)+(transform.up*moveVec.y);

		//movement handling
        moveVec.y-=gravity*Time.deltaTime;
		controller.Move(moveVec* Time.deltaTime);
		float animationSpeedPercent = ((!walking) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);
		animator.SetFloat ("movespeed", attackMode?1.5f:animationSpeedPercent);
		animator.SetFloat ("moveX", inputDir.x*animationSpeedPercent);
		animator.SetFloat ("moveY", inputDir.y*animationSpeedPercent);
		//Debug.DrawRay(transform.position+(3*Vector3.up),(new Vector3(moveVec.x,0,moveVec.z))*100, Color.blue);
	}

	void jump(){
		//jumping
		if(controller.isGrounded&&Input.GetKey(KeyCode.Space)){
			moveVec.y = 0;
			isJumping = true;
			jumpTimeCounter = jumpTime;
			moveVec += transform.up*3.0f;
		}
        if(Input.GetKey(KeyCode.Space)&&isJumping){
			if(jumpTimeCounter>0){
				moveVec+=transform.up*force*jumpTimeCounter*Time.deltaTime;
				jumpTimeCounter -= Time.deltaTime;
			}else{
				isJumping = false;
			}
        }
		if(Input.GetKeyUp(KeyCode.Space)){
			isJumping = false;
		}
		if(controller.isGrounded){
			jumpmode = 0;
			outFly = false;
		}else{
			jumpmode = (jumpTimeCounter>0.75)?1:2;
		}
		animator.SetInteger("jump",jumpmode);
		if(Input.GetKeyDown(KeyCode.Space)&&!controller.isGrounded){
			flying = true;
			isJumping = true;
		}
	}

	void fly(){
		outFly = false;
		Vector2 input = new Vector2 (Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		inputDir = input.normalized;

		if (currentSpeed > 5 || Input.GetMouseButton(0)) {
			transform.forward = Vector3.SmoothDamp(transform.forward, Quaternion.AngleAxis(-25f*currentSpeed/flySpeed, transform.right)*cameraT.forward, ref moveVec, 0.03f);
			moveMode = 1f;
		}

		float targetSpeed = ((Input.GetAxisRaw("Vertical")+1)/2f)*flySpeed;
		
		currentSpeed = Mathf.SmoothDamp (currentSpeed, Input.GetAxisRaw("Vertical")==0?currentSpeed:targetSpeed, ref speedSmoothVelocity, 1f);
        moveVec = (transform.forward * currentSpeed);

		if(Input.GetKeyDown(KeyCode.Space)){
			flying = false;
			transform.eulerAngles = Vector3.up*transform.eulerAngles.y;
			outFly = true;
		}
		controller.Move(moveVec* Time.deltaTime);
		animator.SetInteger ("jump", 2);
	}

	void attack(){
		isAttacking = false;
		//attacking
		bool valid = attackValid();
		if(Input.GetMouseButton(0)&&valid){
			isAttacking = true;
			animator.SetInteger("attack",2);
			attackFrameCounter=-1;
		}
		if(Input.GetMouseButtonUp(0)||!valid){
			animator.SetInteger("attack",0);
			if((Input.GetMouseButtonUp(0)&&valid)||(!valid&&attackValidPrev)&&isAttacking){
				attackFrameCounter = 7;
			}
			isAttacking = false;
		}
		if(!isAttacking&&attackFrameCounter>0){
			attackFrameCounter-=1;
		}
		if(Input.GetMouseButtonDown(2)){
			attackMode = !attackMode;
			animator.SetBool("scope", attackMode);
			if(OnAttackModeSwitch!=null){
				OnAttackModeSwitch();
			}
		}
		if((isAttacking||attackFrameCounter>0)&&controller.isGrounded){
			arm.RotateAround(arm.position,transform.right,!flying?(cameraT.eulerAngles.x+(attackMode?-20*(currentSpeed/runSpeed):0)):0);
		}
		rotateArm(cameraT.forward, transform.forward);
		attackValidPrev = valid;
	}

	void launchAttack(){
		if(isAttacking){
			if(currSpell==null){
				currSpell = Instantiate(spell,wandTip.transform.position,Quaternion.identity) as GameObject;
			}else if(!currSpell.GetComponent<Spell>().going){
				currSpell = Instantiate(spell,wandTip.transform.position,Quaternion.identity) as GameObject;
			}
			if(Physics.Raycast(wandTip.transform.position, cameraT.forward, out hit))
 			{
     			GameObject block = hit.collider.gameObject;
				if(block.tag == "EnemyCube"){
					Destroy(block);
				}
 			}
		}else{
			
		}

	}

	void rotateArm(Vector3 forward, Vector3 inputDir){
		//   1  0  7
		//    \ | /
		//     \|/
		//   2--0--6
		//     /|\
		//    / | \
		//   3  4  5
		int quad = 0;
		forward = Vector3.ProjectOnPlane(forward, Vector3.up);	
		float degree = Vector3.SignedAngle(inputDir, forward, Vector3.up);
		degree = degree<0?360+degree:degree;
		if(!(isAttacking||attackFrameCounter>0)||flying){
			degree = 0;
		}
		quad = (int)degree/45;
		armdeg = Mathf.SmoothDampAngle(armdeg, degree, ref armturnvel, 0.1f);
		armdeg = armdeg<0?armdeg%-360:armdeg%360;
		if(quad<3.9f){
			arm.RotateAround(arm.position,transform.up,armdeg);
			hand.RotateAround(hand.position,transform.up,(0.4f*(armdeg)));
		}else{
			arm.RotateAround(arm.position,transform.up,(0.1f*(armdeg)));
			forearm.RotateAround(forearm.position,transform.up,(0.3f*(armdeg)));
			hand.RotateAround(hand.position,transform.up,(0.3f*(armdeg)));
		}

		armprev = degree;

	}

	bool attackValid(){
		Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
		Vector3 camforward = Vector3.ProjectOnPlane(cameraT.forward, Vector3.up);
		float degree = Vector3.SignedAngle(camforward, forward, Vector3.up);
		degree = degree<0?360+degree:degree;
		return !(degree>135&&degree<225);
	}

}