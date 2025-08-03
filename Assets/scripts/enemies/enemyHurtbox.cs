using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHurtbox : MonoBehaviour
{
    // Start is called before the first frame update
    public float health;
    StateManager stateManager;
    Animator anim;
    public SpriteRenderer sprite;
    public Material ogMat;
    Rigidbody2D rb;

    void Start()
    {
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        anim = GetComponentInParent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
        sprite = GetComponentInParent<SpriteRenderer>();
        ogMat = sprite.material;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.name.Contains("hitbox")) return;
        if (collision.gameObject.layer == 7)//throwable
        {
            Throwable throwable = collision.transform.GetComponentInParent<Throwable>();
            if (!throwable.grounded && throwable.active && !throwable.enemy)
            {
                Hurt(8);
                // throwable.throwTimesFR += 1;

            }
            //if (throwable.throwTimesFR >= throwable.health)
            //Destroy(throwable.transform.gameObject);
        }
        if (collision.gameObject.layer == 12)//pen
        {
            if (collision.gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude > 1)
            {
                collision.gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                Hurt(14);
            }
        }
        if (collision.gameObject.layer == 3)//player punch
        {
            Hurt(stateManager.playerPunchDmg);
        }
        if (collision.gameObject.layer == 13)
        {
            StartCoroutine(slammed());
        }
        if (collision.gameObject.layer == 17) // bomb
        {
            Hurt(15);
        }



        if (health <= 0)
        {
            anim.speed = 1;

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
        if (health <= 0)
        {
            stateManager.StartCoroutine("CoolHitStun");
        }
        else
            stateManager.StartCoroutine(stateManager.HitStun(this));

    }

    IEnumerator slammed()
    {
        // Hurt(2f);
        if (GetComponentInParent<Brute>() != null) GetComponentInParent<Brute>().enabled = false;
        if (GetComponentInParent<Stapler>()!=null) GetComponentInParent<Stapler>().enabled = false;
        if (GetComponentInParent<Janitor>() != null) GetComponentInParent<Janitor>().enabled = false;
        rb.velocity = new Vector2(0, 5);
        anim.speed = 0;
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
        yield return new WaitForSeconds(2);
        if (GetComponentInParent<Brute>() != null) GetComponentInParent<Brute>().enabled = true;
        if (GetComponentInParent<Stapler>()!=null) GetComponentInParent<Stapler>().enabled = true;
        if (GetComponentInParent<Janitor>() != null) GetComponentInParent<Janitor>().enabled = true;
        anim.speed = 1;
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
    }


}
