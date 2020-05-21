using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    float aliveTime = 1f;
    bool dying = true;
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
