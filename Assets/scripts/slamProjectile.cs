using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slamProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (transform.rotation.eulerAngles.y == 0) this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(10, 0);
        else this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-10, 0);
        Destroy(gameObject, 4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
