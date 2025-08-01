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


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name != "hitbox") return;
        if (collision.gameObject.layer == 7)//throwable
        {
            Throwable throwable = collision.transform.GetComponentInParent<Throwable>();
            if (!throwable.grounded && throwable.active && throwable.enemy) 
                player.Hurt(4f);
        }
        if (collision.gameObject.layer == 6)//brute
        {
            player.Hurt(2f);
        }
    }
}
