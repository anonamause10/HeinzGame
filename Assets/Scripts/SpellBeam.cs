using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBeam : Spell
{
    public Vector3 rotdamp = Vector3.zero;
    public SpellBeam beam;
    public Transform beamStart;

    public override void Start(){
        player = GameObject.Find("paris").GetComponent<MoveHeinz>();
        beam = GetComponent<SpellBeam>();
    }


    public override void LateUpdate()
    {
        if(player.isAttacking&&going){
            useEffect();
        }else if(!player.isAttacking){
            stopEffect();
        }

    }

    public override void useEffect(){
        if(going){
            //print("active");

            transform.up = player.cameraT.forward*100;//Vector3.SmoothDamp(transform.forward, player.cameraT.forward,ref rotdamp, 0.2f);
            transform.position = (player.wandTip.transform.position);
        }else{
            //print("no");
        }
    }

    public override void stopEffect(){
        going = false;
       
        Destroy(gameObject);
        
    }
}
