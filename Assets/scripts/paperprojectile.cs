using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paperprojectile : MonoBehaviour
{
    int killyourselftimer;

    // Start is called before the first frame update
    void Start()
    {
        killyourselftimer = 120;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position -= Vector3.up * 0.1f;
        if (killyourselftimer-- < 0) Destroy(this.gameObject);
    }



}
