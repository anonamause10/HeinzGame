using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInput : CharInput
{

    public GameObject centerPoint;
    public GameObject player;
    public MoveHeinz playerScript;

    public override void Start(){    
        player = GameObject.Find("paris");
        playerScript = player.GetComponent<MoveHeinz>();
        cameraT = centerPoint.transform;
    }

    public override void CollectInputs(){
        cameraT.forward = player.transform.position-transform.position;
    }

}
