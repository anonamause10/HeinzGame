using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : Spell
{
    public Vector3 rotdamp = Vector3.zero;
    public ParticleSystem beamLine;
    

    public override void StartStuff(){
        beamLine = GetComponent<ParticleSystem>();
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
            transform.position = player.wandTip.transform.position;
            transform.forward = player.cameraT.forward*100;//Vector3.SmoothDamp(transform.forward, player.cameraT.forward,ref rotdamp, 0.2f);
        }else{
            //print("no");
        }
		if(!beamLine.isEmitting){
			beamLine.Play();
		}
    }

    public override void stopEffect(){
        going = false;
        if(!beamLine.isStopped){
			beamLine.Stop();
		}
        if(!beamLine.IsAlive()){
            Destroy(gameObject);
        }
    }
}
