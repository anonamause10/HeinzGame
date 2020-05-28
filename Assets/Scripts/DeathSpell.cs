using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSpell : Spell
{
    public ParticleSystem beamLine;
    private ParticleCollisionEvent[] CollisionEvents;
    

    public override void StartStuff(){
        damage = 1f;
        beamLine = GetComponent<ParticleSystem>();
        CollisionEvents = new ParticleCollisionEvent[8];
    }

    public override void LateUpdate()
    {
        if(player.isAttacking&&going){
            UseEffect();
        }else if(!player.isAttacking){
            StopEffect();
        }

    }

    public override void UseEffect(){

        transform.position = player.wandTip.transform.position;
        transform.forward = (player.hit.distance!=0?player.hit.point:player.cameraT.position+player.cameraT.forward*100)-player.wandTip.transform.position;
        
		if(!beamLine.isEmitting){
			beamLine.Play();
		}
    }

    public override void StopEffect(){
        going = false;
        if(!beamLine.isStopped){
			beamLine.Stop();
		}
        if(!beamLine.IsAlive()){
            Destroy(gameObject);
        }
    }

    public override void UseEffectEnemy(GameObject enemy){
        enemy.GetComponent<MoveHeinz>().poison(0.01f,5);
    }

    public void OnParticleCollision(GameObject other)
    {
        if(other.tag == opposing){
			UseEffectEnemy(other);
		}
    }   
 
}
