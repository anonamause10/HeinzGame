using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject block;
    public int width;
    public int length;
    public float spawnTime = 3;
    public float spawnTimer;
    public float spawnDist = 50;
    public bool init = true;
    // Start is called before the first frame update
    void Start()
    {
        if(init){
            for (int y=0; y<length; y++)
            {
                for (int x=0; x<width; x++)
                {
                    Instantiate(block, new Vector3(x*spawnDist,50,y*spawnDist), Quaternion.identity);
                }        
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float randW = Random.Range(0, width*spawnDist);
        float randL = Random.Range(0, length*spawnDist);

        if(spawnTimer<=0){
            spawnTimer = spawnTime;
            Instantiate(block, new Vector3(randW,50,randL), Quaternion.identity);
        }

        spawnTimer -= Time.deltaTime;


    }
}
