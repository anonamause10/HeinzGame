using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    protected float aliveTime = 0.2f;
    protected bool dying = true;
    // Start is called before the first frame update
    void Start()
    {
        StartStuff();
    }

    public virtual void StartStuff()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if(dying){
            aliveTime-=Time.deltaTime;
        }
        if(aliveTime<0){
            Destroy(gameObject);
        }
    }

    public virtual void kill(){

        GetComponent<ParticleSystem>().Stop();
        dying = true;
    }
}
