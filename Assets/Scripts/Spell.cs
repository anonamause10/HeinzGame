using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(300)]
public class Spell : MonoBehaviour
{
    public MoveHeinz player;
    public bool going = true;
    public bool buff = false;//spell that boosts player stats or not
    
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("paris").GetComponent<MoveHeinz>();
        StartStuff();
    }

    public virtual void StartStuff(){

    }

    // Update is called once per frame
    public virtual void LateUpdate()
    {
        
    }

    public virtual void UseEffect(){
        
    }

    public virtual void StopEffect(){

    }

    public virtual void UseEffectEnemy(GameObject enemy){
        
    }

    public virtual bool NewAttackValid(MoveHeinz other){
        return other.currSpell == null||!other.currSpell.GetComponent<Spell>().going;
    }

}
