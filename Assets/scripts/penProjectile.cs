using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class penProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (transform.rotation.eulerAngles.y == 0) this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(30,0);
        else this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-30,0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
}
