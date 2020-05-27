using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBoltSpell : Spell
{
    private float timer = 0;
    private float radius = 0.1f;
    private float endTime = 3f;
    private float finalTime = 10f;
    private GameObject bolt;
    private Renderer rend;
    public bool attackingDone;

    public override void StartStuff(){
        timer = 0;
        //Physics.IgnoreCollision(player.gameObject.GetComponent<Collider>(), GetComponent<Collider>(), bool ignore = true);
        transform.localScale = Vector3.zero;
        bolt = (GameObject)Resources.Load("Prefabs/FireBoltSpell");
        rend = GetComponent<Renderer>();
    }

    public override void LateUpdate()
    {
        UseEffect();
        if(!player.isAttacking){
            StopEffect();
        }

    }

    public override void UseEffect(){
        transform.position = player.wandTip.transform.position;
        transform.localScale = radius*Mathf.Clamp(timer/endTime,0,1)*Vector3.one;
        rend.material.SetFloat("_Amount", Mathf.Clamp(timer/endTime,0,1));
        if(timer>finalTime){
            StopEffect();
        }
        timer += Time.deltaTime;

    }

    public override void StopEffect(){
        GameObject firedBolt = Instantiate(bolt, player.wandTip.transform.position,Quaternion.identity);
        firedBolt.GetComponent<Spell>().SetPlayer(player);
        firedBolt.GetComponent<Spell>().damage = Mathf.Clamp(timer/endTime,0.2f,1)*20;
        Destroy(gameObject);
    }

    public override bool EffectValid(MoveHeinz other){
        return Input.GetMouseButton(0)&&timer<finalTime;
    }
 
}
