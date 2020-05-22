using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBoltSpell : Spell
{
    public float velocity=50;
    public float timer = 5f;
    private GameObject explosion;
    public bool attackingDone;

    public override void StartStuff(){
        //Physics.IgnoreCollision(player.gameObject.GetComponent<Collider>(), GetComponent<Collider>(), bool ignore = true);
        transform.up = (player.hit.distance!=0?player.hit.point:player.cameraT.position+player.cameraT.forward*100)-player.wandTip.transform.position;
        explosion = (GameObject)Resources.Load("Prefabs/FireBoltRelease");
        Instantiate(explosion, player.wandTip.transform.position, Quaternion.LookRotation(transform.up));
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
        enemy.GetComponent<MoveBlock>().health-=11;
    }

    void OnTriggerEnter(Collider other){
        Instantiate(explosion, other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position), Quaternion.LookRotation(Vector3.up)); 
        if(other.gameObject.tag == "Player"||other.gameObject.tag == "Spell"){
            return;
        }
        if(other.gameObject.tag == "EnemyCube"){
            UseEffectEnemy(other.gameObject);
        }
        Collider[] objectsHit = Physics.OverlapSphere(transform.position, 10, ~(1<<10));
        foreach (Collider item in objectsHit)
        {
            if(item.gameObject.tag == "EnemyCube"){
                item.gameObject.GetComponent<MoveBlock>().health-=5;
                item.gameObject.GetComponent<Rigidbody>().AddExplosionForce(1000,transform.position,10,0f);
            }
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
