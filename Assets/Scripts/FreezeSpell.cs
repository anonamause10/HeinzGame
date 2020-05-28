using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeSpell : Spell
{
    public Material material;
    GameObject explosion;
    private GameObject currExplosion;
    private float densityTime;
    private float dieTime;
    private float speedDamage;

    public override void StartStuff(){
        speedDamage = 0.05f;
        damage = 1f;//damage per second for this case
        material = GetComponent<Renderer>().material;
        explosion = (GameObject)Resources.Load("Prefabs/FreezeExplosion");
        dieTime = 0.5f;
    }


    public override void LateUpdate()
    {
        if(player.isAttacking&&going){
            UseEffect();
        }else{
            StopEffect();
        }

    }

    public override void UseEffect(){
        material.SetFloat("_Density",Mathf.Lerp(1.3f,-0.2f,densityTime/0.5f));
        densityTime+=Time.deltaTime;

        Vector3 wandToPoint = (player.hit.distance!=0?player.hit.point:(player.cameraT.position+player.cameraT.forward*100))-player.wandTip.transform.position;

        float dist = wandToPoint.magnitude/2;
        float xzscale = 0.3f*Mathf.Atan(dist/10)+0.1f;
        transform.localScale = new Vector3(xzscale,dist,xzscale);
        transform.up = wandToPoint;
        transform.position = (player.wandTip.transform.position+(transform.localScale.y*transform.up));
       
        if(player.hit.distance!=0&&player.hit.distance<100){
            if(currExplosion==null){
                currExplosion = Instantiate(explosion,player.hit.point,Quaternion.LookRotation(player.hit.normal,Vector3.up)) as GameObject;
            }else{
                currExplosion.transform.position = player.hit.point;
                currExplosion.transform.forward = player.hit.normal;
            }
        }else{
            if(currExplosion!=null){
                currExplosion.GetComponent<Boom>().kill();
                currExplosion = null;
            }
        }

        if(player.hit.collider!=null&&wandToPoint.magnitude<100)
 		{
     		GameObject block = player.hit.collider.gameObject;
			if(block.tag == opposing){
				UseEffectEnemy(block);
			}
 		}
    }

    public override void UseEffectEnemy(GameObject enemy){
        MoveHeinz blockScript = enemy.GetComponent<MoveHeinz>();
        if(blockScript.speedMult>0){
            blockScript.speedMult-=speedDamage;
            blockScript.health-=damage*Time.deltaTime;
        }
    }

    public override void StopEffect(){
        going = false;
        if(currExplosion!=null){
            currExplosion.GetComponent<Boom>().kill();
        }
        material.SetFloat("_Density",Mathf.Lerp(1.3f,material.GetFloat("_Density"),dieTime/0.5f));
        if(dieTime<0){
            Destroy(gameObject);
        }
        dieTime-=Time.deltaTime;
        
    }
}
