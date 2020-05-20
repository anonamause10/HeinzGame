using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeSpell : Spell
{
    public Vector3 rotdamp = Vector3.zero;
    public Transform beamStart;
    public Material material;
    GameObject explosion;
    public GameObject currExplosion;
    public float densityTime;
    public float dieTime;

    public override void StartStuff(){
        material = GetComponent<Renderer>().material;
        explosion = (GameObject)Resources.Load("Prefabs/Explosion");
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

        float dist = Mathf.Clamp((player.hit.distance!=0?player.hit.distance:200)/2,0,100);
        float xzscale = 0.3f*Mathf.Atan(dist/10)+0.1f;
        transform.localScale = new Vector3(xzscale,dist,xzscale);
        transform.up = (player.cameraT.position+player.cameraT.forward*100)-player.wandTip.transform.position;
        transform.position = (player.wandTip.transform.position+(transform.localScale.y*transform.up));
       
        if(player.hit.distance!=0&&player.hit.distance<100){
            if(currExplosion==null){
                currExplosion = Instantiate(explosion,player.hit.point,Quaternion.LookRotation(player.hit.normal,Vector3.up)) as GameObject;
            }else{
                currExplosion.transform.position = player.hit.point;
                currExplosion.transform.forward = player.hit.normal;
            }
        }

        if(player.hit.collider!=null)
 		{
     		GameObject block = player.hit.collider.gameObject;
			if(block.tag == "EnemyCube"){
				UseEffectEnemy(block);
			}
 		}
    }

    public override void UseEffectEnemy(GameObject enemy){
        MoveBlock blockScript = enemy.GetComponent<MoveBlock>();
        if(blockScript.speed>0){
            blockScript.speed-=1;
            blockScript.health-=0.4f;
        }
    }

    public override void StopEffect(){
        going = false;
        if(currExplosion!=null){
            currExplosion.GetComponent<Explosion>().kill();
        }
        material.SetFloat("_Density",Mathf.Lerp(1.3f,material.GetFloat("_Density"),dieTime/0.5f));
        if(dieTime<0){
            Destroy(gameObject);
        }
        dieTime-=Time.deltaTime;
        
    }
}
