using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInput : CharInput
{

    public GameObject centerPoint;
    public GameObject player;
    public MoveHeinz playerScript;
    public Vector3 vecToPlayer;
    public Vector3 vecToPlayerPrev;
    public float sightRadius = 30f;

    public override void Start(){    
        player = GameObject.Find("paris");
        playerScript = player.GetComponent<MoveHeinz>();
        cameraT = centerPoint.transform;
    }

    public override void CollectInputs(){
        vecToPlayerPrev = vecToPlayer;
        switchAttackModePrev = switchAttackMode;
        switchAttackMode = false;
        vecToPlayer = player.transform.position-transform.position;
        cameraT.forward = vecToPlayer;
        if((vecToPlayer.magnitude<sightRadius!=(vecToPlayerPrev.magnitude<sightRadius))){
            switchAttackMode = true;
        }
    }

}
