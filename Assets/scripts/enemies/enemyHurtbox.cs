using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHurtbox : MonoBehaviour
{
    // Start is called before the first frame update
    public float health;
    StateManager stateManager;
    Animator anim;
    void Start()
    {
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        anim = GetComponentInParent<Animator>();
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.name.Contains("hitbox")) return;
        if (collision.gameObject.layer == 7)//throwable
        {
            Throwable throwable = collision.transform.GetComponentInParent<Throwable>();
            if (!throwable.grounded && throwable.active && !throwable.enemy)
            {
                Hurt(10);
                throwable.throwTimesFR += 1;

            }
            //if (throwable.throwTimesFR >= throwable.health)
                    //Destroy(throwable.transform.gameObject);
        }
        if (collision.gameObject.layer == 12)//pen
        {
            if (collision.gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude > 1)
            {
                collision.gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                Hurt(20);
            }
        }
        if (collision.gameObject.layer == 3)//player punch
        {
            Hurt(stateManager.playerPunchDmg);
        }



        if (health <= 0)
        {
            anim.SetTrigger("dead");
            Destroy(transform.parent.gameObject, 2.5f); // die after 2.5s
            foreach (Transform child in transform.parent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    void Hurt(float dmg)
    {
        health -= dmg;
        print("mouch " + dmg);
        if (health <= 0)
        stateManager.StartCoroutine("CoolHitStun");
        else
        stateManager.StartCoroutine("HitStun");
        //flash colour
    }


}
