using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeExplosion : Boom
{
    public override void StartStuff(){
        dying = false;
        aliveTime = 3f;
    }
}
