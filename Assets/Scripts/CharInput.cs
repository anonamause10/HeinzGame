using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharInput : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector2 inputDir;
    public bool leftMouseDown = false;
    public bool leftMouseDownPrev = false;
    public bool walking;
    public bool jumping = false;
    public bool jumpingPrev = false;
    public bool switchAttackMode = false;
    public bool switchSpell;
    public int knockback;
    public Transform cameraT;

    public virtual void Start(){
        cameraT = Camera.main.transform;
    }
    
    public virtual void CollectInputs(){
        leftMouseDownPrev = leftMouseDown;
        jumpingPrev = jumping;
        jumping = Input.GetKey(KeyCode.Space);
        inputDir = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
        leftMouseDown = Input.GetMouseButton(0);
        switchSpell = Input.GetKeyDown(KeyCode.Q);
        switchAttackMode = Input.GetMouseButtonDown(2);
        knockback = GetPressedNumber();
        walking = Input.GetKey (KeyCode.LeftShift);
    }

    public int GetPressedNumber() {

    	for (int number = 0; number <= 9; number++) {
    	    if (Input.GetKeyDown(number.ToString()))
    	        return number;
    	}
 
    	return -1;
	}

}
