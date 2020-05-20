using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTest : MonoBehaviour
{

    public MoveHeinz player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("paris").GetComponent<MoveHeinz>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.hit.point;
    }
}
