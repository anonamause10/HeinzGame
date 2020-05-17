using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBoltSpell : Spell
{
    public float velocity=50;
    public float timer = 5f;
    public bool attackingDone;

    public override void StartStuff(){
        transform.up = (player.cameraT.position+player.cameraT.forward*100)-player.wandTip.transform.position;
    }

    public override void LateUpdate()
    {
        UseEffect();
        if(!player.isAttacking){
            player.currSpell = null;
        }

    }

    public override void UseEffect(){

        transform.Translate(velocity*Time.deltaTime*transform.up,Space.World);  
        timer -= Time.deltaTime;
        if(timer<0){
            StopEffect();
        }  
    }

    public override void StopEffect(){
        Destroy(gameObject);
    }

    public override void UseEffectEnemy(GameObject enemy){
        enemy.GetComponent<MoveBlock>().health-=10;
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"||other.gameObject.tag == "Spell"){
            return;
        }
        if(other.gameObject.tag == "EnemyCube"){
            UseEffectEnemy(other.gameObject);
        }
        StopEffect();
    }

    public override bool NewAttackValid(MoveHeinz other){
        attackingDone = !other.attackingPrev;
        return attackingDone;
    }
 
}
