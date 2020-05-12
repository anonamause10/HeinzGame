using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamSpell : Spell
{
    public Vector3 rotdamp = Vector3.zero;
    public Transform beamStart;
    public Material material;
    GameObject explosion;
    public GameObject currExplosion;

    public override void StartStuff(){
        material = GetComponent<Renderer>().material;
        explosion = (GameObject)Resources.Load("Prefabs/Explosion");
    }


    public override void LateUpdate()
    {
        if(player.isAttacking&&going){
            useEffect();
        }else if(!player.isAttacking){
            stopEffect();
        }

    }

    public override void useEffect(){
        float dist = Mathf.Clamp((player.hit.distance!=0?player.hit.distance:200)/2,0,100);
        transform.localScale = new Vector3(0.3f*Mathf.Atan(dist/5)+0.3f,dist,0.3f*Mathf.Atan(dist/5)+0.3f);
        transform.up = (player.cameraT.position+player.cameraT.forward*100)-player.wandTip.transform.position;
        transform.position = (player.wandTip.transform.position+(transform.localScale.y*transform.up));
       
        if(player.hit.distance!=0&&player.hit.distance<100){
            if(currExplosion==null){
                currExplosion = Instantiate(explosion,player.hit.point,Quaternion.identity) as GameObject;
            }else{
                currExplosion.transform.position = player.hit.point;
                currExplosion.transform.forward = player.hit.normal;
            }
        }
    }

    public override void stopEffect(){
        going = false;
        if(currExplosion!=null){
            currExplosion.GetComponent<Explosion>().kill();
        }
        Destroy(gameObject);
        
    }
}
