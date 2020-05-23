using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlock : MonoBehaviour
{
    GameObject target;
    float initSpeed = 20;
    public float speed = 20;
    private float speedDamp = 1;
    private float colorVal = 1;
    private float colorDampVel = 0;
    public int totalHealth = 20;
    private float dps = 0;
    private float dpsTime = 0;
    private bool poisoned = false;
    public float health;
    Color fullHealthColor = Color.green;
    Color emptyHealthColor = Color.red;  
    Material currMaterial;
    Renderer rend;
    void Start(){
        target = GameObject.Find("paris");
        rend = GetComponent<Renderer>();
        currMaterial = new Material(Shader.Find("Lightweight Render Pipeline/Lit"));
        rend.material = currMaterial;

        health = totalHealth;
    }

    void Update(){
        if(poisoned){
            if(dpsTime>0){
                health-=dps;
                dpsTime-=Time.deltaTime;
            }else{
                poisoned = false;
            }
        }
        if(health<0||transform.position.y<-10f){
            Destroy(gameObject);
        }
        speed = Mathf.SmoothDamp(speed, initSpeed, ref speedDamp, 15f);
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

    public void poison(float amount,float time){
        dps = 0.04f*Mathf.Atan(200*(amount+dps));
        dpsTime = 2.5f*Mathf.Atan(2000*(dpsTime+time));
        poisoned = true;
    }


}
