using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class freehealth : MonoBehaviour
{

    Transform boss;

    // Start is called before the first frame update
    void Start()
    {
        boss = GameObject.Find("Balls").transform;
        transform.parent = null;
        boss.gameObject.GetComponent<Boss>().health += 2.5f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position -= (transform.position - boss.position).normalized;
        if (transform.position.y > 100)
        {
            Destroy(this.gameObject);
        }
    }
}
