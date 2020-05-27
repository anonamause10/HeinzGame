using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(300)]
public class Spell : MonoBehaviour
{
    public MoveHeinz player;
    public bool going = true;
    public bool buff = false;//spell that boosts player stats or not
    public float damage = 0;
    public string opposing;
    public string origin;
    
    
    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.Find("paris").GetComponent<MoveHeinz>();
        StartStuff();
    }

    public virtual void SetPlayer(MoveHeinz boi){
        player = boi;
        origin = boi.gameObject.tag;
        opposing = origin == "Player"?"Enemy":"Player";
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

    public virtual bool NewEffectValid(MoveHeinz other){
        return other.currSpell == null||!other.currSpell.GetComponent<Spell>().going;
    }

    public virtual bool EffectValid(MoveHeinz other){
        return true;
    }
}
