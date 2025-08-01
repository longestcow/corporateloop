using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHurtbox : MonoBehaviour
{
    // Start is called before the first frame update
    public float health;
    StateManager stateManager;
    Animator anim;
    bool dead = false;
    void Start()
    {
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        anim = GetComponentInParent<Animator>();
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name != "hitbox" || dead) return;
        if (collision.gameObject.layer == 7)//throwable
        {
            Throwable throwable = collision.transform.GetComponentInParent<Throwable>();
            if (!throwable.grounded && throwable.active && !throwable.enemy)
                Hurt(4);
        }
        if (collision.gameObject.layer == 3)//player punch
        {
            Hurt(stateManager.playerPunchDmg);
        }



        if (health <= 0)
        {
            anim.SetTrigger("dead");
            dead = true;
            Destroy(transform.parent.gameObject, 2.5f); // die after 2.5s
        }
    }

    void Hurt(float dmg)
    {
        health -= dmg;
        print("mouch " + dmg);
        StartCoroutine("HitStun");
        //flash colour
    }

    IEnumerator HitStun()
    {
        //do funny thing with music like hollow knight
        Time.timeScale = 0.1f;
        stateManager.paused = true;
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(0.2f);
        stateManager.paused = false;


    }
}
