using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    float aliveTime = 3f;
    bool dying = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(dying){
            aliveTime-=Time.deltaTime;
        }
        if(aliveTime<0){
            Destroy(gameObject);
        }
    }

    public void kill(){

        GetComponent<ParticleSystem>().Stop();
        dying = true;
    }
}
