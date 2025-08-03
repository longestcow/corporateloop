using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHurtbox : MonoBehaviour
{
    Player player;
    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void FixedUpdate()
    {

        //Sets healthbar size
        player.healthbar.size = 1 - Mathf.Clamp01((player.health / player.maxhealth));
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (player.iframe || player.dead || player.coffeemode) return;
        if (collision.gameObject.layer == 7 && collision.gameObject.name == "interactZone")
        {
            player.GrabUtil(true);
            player.throwable = collision.gameObject.GetComponentInParent<Throwable>();
        }

        if (!collision.gameObject.name.Contains("hitbox")) return;
        if (collision.gameObject.layer == 7)//throwable
        {
            Throwable throwable = collision.transform.GetComponentInParent<Throwable>();


            if (!throwable.grounded && throwable.active && throwable.enemy)
            {
                player.Hurt(4f, 0);
                // throwable.throwTimesFR += 1;
            }
            // if (throwable.throwTimesFR >= throwable.health)
            //         Destroy(throwable.transform.gameObject);
        }
        if (collision.gameObject.layer == 6)//brute
        {
            player.Hurt(2f, 1);
        }
        if (collision.gameObject.layer == 10)//water
        {
            //slip animation
            player.Hurt(3f, 2);
            player.Slip();
            Destroy(collision.gameObject, 0.5f);

        }
        if (collision.gameObject.layer == 11)//staple
        {
            player.Hurt(1f, 3);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.layer == 12)//pen pickup
        {
            if (collision.gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude <= 1f)
            {
                player.pencount++;
                Destroy(collision.transform.parent.gameObject);
            }

        }
        if (collision.gameObject.layer == 15)// papered
        {
            player.Hurt(4f, 4);
        }
        if (collision.gameObject.layer == 16)// jump off
        {
            player.Hurt(1000f, 5);
        }
        if (collision.gameObject.layer == 17) // elevator
        {
            player.Hurt(1000f, 6);
        }
        if (collision.gameObject.layer == 17) // bomb
        {
            player.Hurt(5f, 7);
        }

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7 && collision.gameObject.name == "interactZone" && collision.gameObject.GetComponentInParent<Throwable>() == player.throwable)
        {
            player.GrabUtil(false);

            player.throwable = null;
        }

    }
}
