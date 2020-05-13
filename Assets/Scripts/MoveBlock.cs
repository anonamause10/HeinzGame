using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlock : MonoBehaviour
{
    GameObject target;
    public int totalHealth = 20;
    public int health;
    Gradient grad;  
    Material currMaterial;
    Renderer rend;
    void Start(){
        target = GameObject.Find("paris");
        rend = GetComponent<Renderer>();
        currMaterial = new Material(Shader.Find("Lightweight Render Pipeline/Lit"));
        rend.material = currMaterial;
        grad = new Gradient();

        health = totalHealth;

        //Color keys
        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].color = Color.red;
        colorKey[0].time = 0.0f;
        colorKey[1].color = new Color(0.18f,0.93f,1,1);
        colorKey[1].time = 1.0f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;

        grad.SetKeys(colorKey, alphaKey);
    }

    void Update(){
        if(health<0||transform.position.y<-10f){
            Destroy(gameObject);
        }
        Vector3 targetVec = target.transform.position-transform.position;
	    Vector3 move = Vector3.ProjectOnPlane(targetVec,Vector3.up);
        float moveMag = Mathf.Clamp(0.4f*move.magnitude,0,20);
        if (move.magnitude < 6){
            moveMag = 0;
        }
	    transform.Translate(moveMag*move.normalized*Time.deltaTime,Space.World);
		currMaterial.color = grad.Evaluate(((float)health)/totalHealth);

    }
}
