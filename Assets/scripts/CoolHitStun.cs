using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolHitStun : MonoBehaviour
{
    public Color normalcolor;
    public bool white;
    public bool gone;
    public bool enemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!white)
        {
            if (StateManager.coolstun) this.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
            else this.GetComponent<SpriteRenderer>().color = normalcolor;
        }
        if (white)
        {
            if (StateManager.coolstun) this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            else this.GetComponent<SpriteRenderer>().color = normalcolor;
        }
        if (gone)
        {
            if (StateManager.coolstun) this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            else this.GetComponent<SpriteRenderer>().color = normalcolor;
        }
        if (enemy)
        {
            if (StateManager.coolstun) this.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_CoolStun", 1);
            else this.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_CoolStun", 0);
        }
    }
}
