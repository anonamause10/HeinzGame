﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public ParticleSystem beamLine;
    public MoveHeinz player;
    public bool going = true;
    public bool buff;//spell that boosts player stats or not
    
    
    // Start is called before the first frame update
    void Start()
    {
        beamLine = GetComponent<ParticleSystem>();
        player = GameObject.Find("paris").GetComponent<MoveHeinz>();
    }

    // Update is called once per frame
    public virtual void LateUpdate()
    {
        
    }

    public virtual void useEffect(){
        
    }

    public virtual void stopEffect(){

    }

}
