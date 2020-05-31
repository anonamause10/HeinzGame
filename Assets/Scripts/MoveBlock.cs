using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlock : MoveHeinz
{
    GameObject target;
    float initSpeed = 20;
    private float colorVal = 1;
    private float colorDampVel = 0;
    Color fullHealthColor = Color.green;
    Color emptyHealthColor = Color.red;  
    Material currMaterial;
    Renderer rend;

    public override void Start(){
        target = GameObject.Find("paris");
        rend = GetComponent<Renderer>();
        currMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        rend.material = currMaterial;

        health = totalHealth;
    }

    public override void LateUpdate(){
        HandleDamage();
        speedMult = Mathf.SmoothDamp(speedMult, 1f, ref speedMultDamp, 15f);
        float speed = speedMult*initSpeed;
        Vector3 targetVec = target.transform.position-transform.position;
	    Vector3 move = Vector3.ProjectOnPlane(targetVec,Vector3.up);
        float moveMag = Mathf.Clamp(0.4f*move.magnitude,0,speed);
        if (move.magnitude < 6){
            moveMag = 0;
        }
	    transform.Translate(moveMag*move.normalized*Time.deltaTime,Space.World);
        colorVal = Mathf.SmoothDamp(colorVal, health/totalHealth, ref colorDampVel, 0.1f);
		currMaterial.color = Color.Lerp(emptyHealthColor,fullHealthColor,colorVal);
    }

    public override void SetKnockbackDirection(Vector3 projPos, float magnitude){

    }

    public override void HandleDamage(){
        if(poisoned){
            if(dpsTime>0){
                health-=dps;
                dpsTime-=Time.deltaTime;
            }else{
                poisoned = false;
            }
        }
		dead = health<0;
        if((dead)){
            kill();
        }
    }

    public override void kill(){
        Destroy(gameObject);
    } 


}
