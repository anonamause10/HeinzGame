using UnityEngine;
using System;
using System.Collections;

public class MoveHeinz : MonoBehaviour {
	//public vars

	public CharInput charInput;

	protected float walkSpeed = 15;
	protected float runSpeed = 30;
	protected int gravity = 20;
	public float flySpeed = 100;
	//some move stuff
	protected float turnSmoothTime = 0.2f;
	protected float turnSmoothVelocity;
	protected float turnSmoothVelocityY;
	public bool walking = false;

	//move and jump stuff
	protected float speedSmoothTime = 0.1f;
	protected float force = 40f;
	public float currentSpeed;
	protected float speedSmoothVelocity = 1f;
	protected float moveMode;
	protected float moveModeVelocity = 1f;
	protected int knockback = 0;
	protected float knockbackdamp;
	protected float knockbackspeed = 20;
	protected Vector2 knockbackdirection;
    protected Vector3 moveVec;
	protected Vector2 inputDir;
	protected Vector2 inputSmoothDamp;
	protected float jumpTimeCounter;
	protected float jumpTime=1;
	protected bool isJumping;
	public bool flying = false;
	protected bool outFly = false;
	protected int jumpmode;

	//attack stuff
	public bool isAttacking = false;
	public bool attackingPrev;
	protected float attackFrameCounter;
	protected Transform arm;
	protected Transform forearm;
	protected Transform hand;
	public GameObject wandTip;
	public bool attackMode = false;
	public bool attackValidPrev;
	public event Action OnAttackModeSwitch;
	protected float armturnvel = 1f;
	protected float armdeg = 0;
	protected float armprev = 0;
	protected int armTurnFrameCounter;
	public RaycastHit hit;
	public Transform crosshair;
	protected GameObject spell;
	public GameObject currSpell;
	protected int spellIndex;
	protected String[] spells;

	//controller stuff
	protected CharacterController controller;
	protected Animator animator;
	protected Rigidbody rbody;
	public Transform cameraT;

	void Start () {
		charInput = GetComponent<CharInput>();
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator> ();
		rbody = GetComponent<Rigidbody>();
		arm = transform.Find("Armature").Find("Bone").Find("Bone.007");
		forearm = arm.Find("Bone.008");
		hand = forearm.Find("Bone.009");
		cameraT = charInput.cameraT;
		spells = new String[]{"FreezeSpell","DeathSpell","FireBoltCharge","BaseBoltSpell","BaseBoltHeavySpell"};
		spell = (GameObject)Resources.Load("Prefabs/" + spells[spellIndex]);
		crosshair = GameObject.Find("Canvas").transform.Find("Crosshair");
		//animator.speed = 0.5f;
	}

	void LateUpdate () {
		attackingPrev = isAttacking;
		charInput.CollectInputs();
		Knockback();
		if(flying){
			fly();
		}else{
			jump();
			move();
		}		
		attack();
		
		launchAttack();
		Physics.Raycast(cameraT.position, cameraT.forward, out hit, Mathf.Infinity, ~(1<<10));
		//print(knockback);
		
		//Debug.DrawRay(wandTip.transform.position, (lookHit.distance!=0?(lookHit.point-wandTip.transform.position):cameraT.position+cameraT.forward*100)*100,Color.red);
	}

	void move(){
		//walking
		jumpmode = 0;
		if(knockback==0){
			Vector2 input = charInput.inputDir;
			inputDir = Vector2.SmoothDamp(inputDir, input.normalized, ref inputSmoothDamp, !controller.isGrounded?turnSmoothTime:turnSmoothTime*1f*((currentSpeed<5?5:currentSpeed)/runSpeed));
			if (inputDir != Vector2.zero || animator.GetInteger("attack")==2||attackMode) {
				float targetRotation = Mathf.Atan2 (inputDir.x*(((attackMode&&charInput.inputDir.y<0)?-1:1)), (attackMode)?Mathf.Abs(inputDir.y):inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;

				transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, controller.isGrounded?turnSmoothTime:turnSmoothTime*1f*((currentSpeed<5?5:currentSpeed)/runSpeed));
			}
			walking = charInput.walking;


			float targetSpeed = ((walking) ? walkSpeed : runSpeed) * inputDir.magnitude;
			currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, controller.isGrounded?speedSmoothTime:5f);
		}

		moveVec = (((attackMode&&!outFly)?(Quaternion.AngleAxis(cameraT.eulerAngles.y, Vector3.up)*new Vector3(inputDir.x,0,inputDir.y)):transform.forward) * currentSpeed)+(transform.up*moveVec.y)+(knockback!=0? new Vector3(knockbackdirection.x,0,knockbackdirection.y):Vector3.zero);

		//movement handling
        moveVec.y-=gravity*Time.deltaTime;
		controller.Move(moveVec* Time.deltaTime);
		float animationSpeedPercent = ((!walking) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);
		float targetMoveMode = attackMode?1:0;
		moveMode = Mathf.SmoothDamp(moveMode,targetMoveMode,ref moveModeVelocity,0.1f);
		animator.SetInteger("knockback", knockback);
		animator.SetFloat("moveMode", moveMode);
		animator.SetFloat("movespeed", animationSpeedPercent);
		animator.SetFloat("moveX", inputDir.x*animationSpeedPercent);
		animator.SetFloat("moveY", inputDir.y*animationSpeedPercent);
		//Debug.DrawRay(transform.position+(3*Vector3.up),(new Vector3(moveVec.x,0,moveVec.z))*100, Color.blue);
	}

	void jump(){
		//jumping
		if(controller.isGrounded&&charInput.jumping){
			moveVec.y = 0;
			isJumping = true;
			jumpTimeCounter = jumpTime;
			moveVec += transform.up*3.0f;
		}
        if(charInput.jumping&&isJumping){
			if(jumpTimeCounter>0){
				moveVec+=transform.up*force*jumpTimeCounter*Time.deltaTime;
				jumpTimeCounter -= Time.deltaTime;
			}else{
				isJumping = false;
			}
        }
		if(!charInput.jumping&&charInput.jumpingPrev){
			isJumping = false;
		}
		if(controller.isGrounded){
			jumpmode = 0;
			outFly = false;
		}else{
			jumpmode = (jumpTimeCounter>0.75)?1:2;
		}
		animator.SetInteger("jump",jumpmode);
		if((charInput.jumping&&!charInput.jumpingPrev)&&!controller.isGrounded){
			flying = true;
			isJumping = true;
		}
	}

	void fly(){
		outFly = false;

		if (currentSpeed > 5 || charInput.leftMouseDown) {
			transform.forward = Vector3.SmoothDamp(transform.forward, Quaternion.AngleAxis(-25f*currentSpeed/flySpeed, transform.right)*cameraT.forward, ref moveVec, 0.03f);
		}

		float targetSpeed = ((charInput.inputDir.y+1)/2f)*flySpeed;
		
		currentSpeed = Mathf.SmoothDamp (currentSpeed, charInput.inputDir.y==0?currentSpeed:targetSpeed, ref speedSmoothVelocity, 1f);
        moveVec = (transform.forward * currentSpeed);

		if(charInput.jumping&&!charInput.jumpingPrev){
			flying = false;
			transform.eulerAngles = Vector3.up*transform.eulerAngles.y;
			outFly = true;
		}
		controller.Move(moveVec* Time.deltaTime);
		animator.SetInteger ("jump", 2);
	}

	void attack(){
		isAttacking = false;
		if(charInput.switchSpell){
			spellIndex = (spellIndex+1)%spells.Length;
			spell = (GameObject)Resources.Load("Prefabs/" + spells[spellIndex]);
		}
		//attacking
		bool valid = attackValid();
		if(charInput.leftMouseDown&&valid){
			isAttacking = true;
			animator.SetInteger("attack",2);
			attackFrameCounter=-1;
		}
		if(!charInput.leftMouseDown&&charInput.leftMouseDownPrev||!valid){
			animator.SetInteger("attack",0);
			if((!charInput.leftMouseDown&&charInput.leftMouseDownPrev&&valid)||(!valid&&attackValidPrev)){
				attackFrameCounter = 7;
			}
			isAttacking = false;
		}
		if(!isAttacking&&attackFrameCounter>0){
			attackFrameCounter-=1;
		}
		if(charInput.switchAttackMode){
			attackMode = !attackMode;
			animator.SetBool("scope", attackMode);
			if(OnAttackModeSwitch!=null){
				OnAttackModeSwitch();
			}
		}
		if((isAttacking||attackFrameCounter>0||attackMode)&&controller.isGrounded){
			arm.RotateAround(arm.position,transform.right,!flying?(cameraT.eulerAngles.x+(attackMode?-20*(currentSpeed/runSpeed):0)):0);
		}
		rotateArm(cameraT.forward, transform.forward);
		attackValidPrev = valid;
	}

	public void Knockback(){
		int num = charInput.knockback;
		if(num!=-1&&knockback==0){
			knockback = num;
			knockbackspeed = 120;
			knockbackdamp = 0;
		}
		knockbackdirection = new Vector2(transform.forward.x,transform.forward.z);
		if(knockback!=0&&controller.isGrounded){
			inputDir = Vector2.zero;
			if(knockback<5){
				knockbackspeed = Mathf.SmoothDamp(knockbackspeed,0,ref knockbackdamp,knockback==4?0.3f:0.1f);
				if(knockback<4){
					knockbackdirection*=-1;
				}else{
					
				}
			}else{
				knockbackspeed = Mathf.SmoothDamp(knockbackspeed,0,ref knockbackdamp,0.3f);
				if(knockback<6){
					knockbackdirection*=-1;
				}else{
				
				}
			}
			knockbackdirection *= knockbackspeed;
		}
		if(animator.IsInTransition(0)){
			knockback = 0;
		}
	}

	void launchAttack(){
		if(isAttacking){
			if(spell.GetComponent<Spell>().NewEffectValid(this)){
				currSpell = Instantiate(spell,wandTip.transform.position,Quaternion.identity) as GameObject;
				currSpell.GetComponent<Spell>().SetPlayer(this);
			}
			//spellLine.SetPosition(0,wandTip.transform.position);
			//spellLine.SetPosition(1,cameraT.position+cameraT.forward*100);
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
		if((!(isAttacking||attackFrameCounter>0)||flying)){
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
		return !(degree>135&&degree<225)&&spell.GetComponent<Spell>().EffectValid(this)&&knockback==0;
	}

	public bool MouseDown(){
		return charInput.leftMouseDown&&!charInput.leftMouseDownPrev;
	}

}