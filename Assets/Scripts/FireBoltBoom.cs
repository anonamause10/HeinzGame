using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBoltBoom : Boom
{   
    private float totalTime = 0.8f;
    private MoveHeinz player;
    public float radius = 20;
    public string opposing;
    public string origin;
    // Start is called before the first frame update
    public override void StartStuff(){
        transform.localScale = Vector3.zero;
        dying = true;
        aliveTime = totalTime;
    }

    public void SetPlayer(MoveHeinz boi){
        player = boi;
        origin = boi.gameObject.tag;
        opposing = origin == "Player"?"Enemy":"Player";
    }

    // Update is called once per frame
    public override void Update()
    {

        transform.localScale = radius*((totalTime-aliveTime)/totalTime)*Vector3.one;
        GetComponent<Renderer>().material.SetFloat("_Power", aliveTime/totalTime);
        if(dying){
            aliveTime-=Time.deltaTime;
        }

        if(aliveTime<0){
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == origin||other.gameObject.tag == "Spell"){
            return;
        }
        if(other.gameObject.tag == opposing){
            Vector3 force = (other.gameObject.transform.position-transform.position).normalized*radius*200;
            MoveHeinz playerScript = other.gameObject.GetComponent<MoveHeinz>();
            playerScript.health-=radius;
            if(!other.gameObject.GetComponent<Rigidbody>().isKinematic){
                other.gameObject.GetComponent<Rigidbody>().AddForce(force);
            }else{
                playerScript.SetKnockbackDirection(transform.position,radius);
            }
        }
    }

    public override void kill(){

        dying = true;
        Destroy(gameObject);
    }
}
