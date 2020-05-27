using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBoltSpell : Spell
{
    private float spawnRate = 10f;//projectiles per second
    private GameObject bolt;
    private float spawntime;
    private float spawnTimer = 0;
    

    public override void StartStuff(){
        bolt = (GameObject)Resources.Load("Prefabs/BaseBolt");
        spawntime = 1/spawnRate;
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
        
		if(spawnTimer<=0){
            GameObject firedBolt = Instantiate(bolt,player.wandTip.transform.position, Quaternion.LookRotation(transform.up));
            firedBolt.GetComponent<Spell>().SetPlayer(player);
            spawnTimer = spawntime;
        }
        spawnTimer -= Time.deltaTime;
    }

    public override void StopEffect(){
        going = false;
        Destroy(gameObject);
    }
 
}
