using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBoltHeavySpell : Spell
{
    public float velocity=50;
    public float timer = 5f;
    private GameObject release;
    private GameObject explosion;
    public bool attackingDone;

    public override void StartStuff(){
        damage = 6;
        //Physics.IgnoreCollision(player.gameObject.GetComponent<Collider>(), GetComponent<Collider>(), bool ignore = true);
        transform.up = (player.hit.distance!=0?player.hit.point:player.cameraT.position+player.cameraT.forward*100)-player.wandTip.transform.position;
        release = (GameObject)Resources.Load("Prefabs/BaseBoltRelease");
        explosion = (GameObject)Resources.Load("Prefabs/BaseBoltRelease");
        Instantiate(release, player.wandTip.transform.position, Quaternion.LookRotation(transform.up));
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
        enemy.GetComponent<MoveHeinz>().health-=damage;
        enemy.GetComponent<MoveHeinz>().SetKnockbackDirection(transform.position,damage * 4);
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == origin||other.gameObject.tag == "Spell"){
            return;
        }
        Instantiate(explosion, other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position), Quaternion.LookRotation(Vector3.up)); 
        if(other.gameObject.tag == opposing){
            UseEffectEnemy(other.gameObject);
        }
        StopEffect();
    }

    public override bool NewEffectValid(MoveHeinz other){
        attackingDone = !other.attackingPrev;
        return attackingDone;
    }

    public override bool EffectValid(MoveHeinz other){
        return other.MouseDown();
    }
 
}
