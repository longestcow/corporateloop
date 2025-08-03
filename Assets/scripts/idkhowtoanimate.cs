using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class idkhowtoanimate : MonoBehaviour
{
    public Sprite[] sps;
    public SpriteRenderer spr;
    int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        spr.sprite = sps[(counter++/5) % 5];
    }
}
