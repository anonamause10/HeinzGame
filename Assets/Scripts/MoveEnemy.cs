using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveEnemy : MoveHeinz
{
    public Slider healthBarSlider;
    protected float healthBarDamp = 0;

    public override void HandleDamage(){
        healthBarSlider.value = Mathf.SmoothDamp(healthBarSlider.value, health/totalHealth, ref healthBarDamp, 0.1f);
        base.HandleDamage();
    }

    public override void kill(){
        knockback = Vector3.Dot(projVec,forwardVec)>0?8:9;
        animator.SetInteger("knockback", Vector3.Dot(projVec,forwardVec)>0?8:9);
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Knockback.dead")){
			Destroy(gameObject);
		}

    }
}
