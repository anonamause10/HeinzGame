using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public ParticleSystem beamLine;
    public bool active;
    public bool buff;//spell that boosts player stats or not

    
    // Start is called before the first frame update
    void Start()
    {
        beamLine = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void useEffect(Vector3 direction){
        
    }

    public virtual void stopEffect(){

    }
}
