using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBoltBoom : Boom
{   
    private float totalTime = 2f;
    public float radius = 20;
    // Start is called before the first frame update
    public override void StartStuff(){
        transform.localScale = Vector3.zero;
        dying = true;
        aliveTime = totalTime;
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
        if(other.gameObject.tag == "Player"||other.gameObject.tag == "Spell"){
            return;
        }
        if(other.gameObject.tag == "EnemyCube"){
            Vector3 force = (other.gameObject.transform.position-transform.position).normalized*radius*200;
            other.gameObject.GetComponent<Rigidbody>().AddForce(force);
            other.gameObject.GetComponent<MoveBlock>().health-=1;
        }
    }

    public override void kill(){

        dying = true;
        Destroy(gameObject);
    }
}
