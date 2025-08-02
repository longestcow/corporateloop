using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOnRestart : MonoBehaviour
{
    int lastcount;
    void Start()
    {
        lastcount = Player.runcount;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.runcount != lastcount)
        {
            Destroy(this.gameObject);
        }
    }
}
