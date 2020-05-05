using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlock : MonoBehaviour
{
    GameObject target;
    void Start(){
        target = GameObject.Find("paris");
    }

    void Update(){
        Vector3 targetVec = target.transform.position-transform.position;
	    Vector3 move = Vector3.ProjectOnPlane(targetVec,Vector3.up);
        float moveMag = Mathf.Clamp(0.4f*move.magnitude,0,20);
        if (move.magnitude < 6){
            moveMag = 0;
        }
	    transform.Translate(moveMag*move.normalized*Time.deltaTime,Space.World);
				
    }
}
