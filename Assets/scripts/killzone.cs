using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killzone : MonoBehaviour
{

    public static bool killtime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (killtime) transform.position += new Vector3(0, 0.4f, 0);
        if (transform.position.y > 105)
        {
            killtime = false;
            transform.position = new Vector3(0, -4.5432f, 0);
        }
    }
}
