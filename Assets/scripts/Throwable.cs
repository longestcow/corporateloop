using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    Rigidbody2D rb;
    public bool active = false;
    public bool enemy = false; // if active and enemy, hurts player on trigger with hitbox vice versa
    public bool grounded = true;
    bool pGrounded = true;
    LayerMask layerMask;
    public Collider2D col;
    int throwTimes = -1;
    int health = 2;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        layerMask = LayerMask.GetMask("Floor");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        grounded = col.IsTouchingLayers(layerMask);
        rb.velocity = new Vector2(grounded ? rb.velocity.x * 0.8f : rb.velocity.x, rb.velocity.y);

        if (grounded && !pGrounded)
        {

            throwTimes += 1;
            if (throwTimes % 2 == 0 && throwTimes!=0)
            {
                active = false;
                enemy = false;
            }

            if (throwTimes >= health * 2)//chair break
            {
                Destroy(gameObject, 0.7f);
            }
            print("BLAH");
        }
        else if (!grounded && pGrounded)
            active = true;


        pGrounded = grounded;
    }
}


