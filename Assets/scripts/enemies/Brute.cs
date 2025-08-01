using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brute : MonoBehaviour
{
    public int state = 0;
    // 0 idle
    // 1 player found (check if throwables in same room)
    // 2 no throwables, beat up
    // 3 throwable, go to throwable
    // 4 grab and throw
    Animator anim;
    Transform player, throwable;
    Rigidbody2D rb;
    bool punchCooldown = false, canGrab=true;
    public GameObject grabPoint;
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        // start idle anim
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Dead")) return; //dudes dead, waiting to get destroyed now
        if (state == 0) // look for player
        {
            RaycastHit2D playerHit = Physics2D.Raycast(new Vector2(10, transform.position.y), Vector2.left, 20, LayerMask.GetMask("Player"));
            // Debug.DrawRay(new Vector2(10, transform.position.y), Vector2.left * 20, Color.red);
            if (playerHit.collider != null)
            {
                state = 1;
                player = playerHit.collider.gameObject.transform;
            }
        }
        if (state == 1)
        {
            if (!canGrab) return;
            RaycastHit2D throwableHit = Physics2D.Raycast(new Vector2(10, transform.position.y), Vector2.left, 20, LayerMask.GetMask("Throwable"));
            if (throwableHit.collider != null)
            {
                state = 3;
                throwable = throwableHit.collider.gameObject.transform.root;
            }
            else
                state = 2;
        }
        if (state == 2)
        {
            float distance = transform.position.x - player.position.x; // if positive = player to the left, vice versa
            if (Mathf.Abs(distance) > 1.5f)
            {// keep moving closer
                rb.velocity = new Vector2((distance > 0 ? -1 : 1) * 2, rb.velocity.y);
                transform.rotation = Quaternion.Euler(0, distance > 0 ? 0 : 180, 0);
                anim.SetBool("running", true);
            }
            else if (!punchCooldown)
            {
                transform.rotation = Quaternion.Euler(0, distance > 0 ? 0 : 180, 0);
                anim.SetBool("running", false);
                anim.SetTrigger("punch");
                StartCoroutine("PunchTimer");
            }
            else
                anim.SetBool("running", false);

        }
        if (state == 3)
        {
            float distance = transform.position.x - throwable.position.x; // if positive = throwable to the left, vice versa
            if (Mathf.Abs(distance) > 1.5f)
            {// keep moving closer
                rb.velocity = new Vector2((distance > 0 ? -1 : 1) * 2, rb.velocity.y);
                anim.SetBool("running", true);
                transform.rotation = Quaternion.Euler(0, distance > 0 ? 0 : 180, 0);
            }
            else if (!anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("grab") && canGrab)//not already grabbing
            {
                anim.SetBool("running", false);
                throwable.GetComponent<Throwable>().enemy = true;
                throwable.parent = grabPoint.transform;
                throwable.localPosition = Vector3.zero;
                throwable.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                anim.SetTrigger("grab");
                canGrab = false;
                StartCoroutine("ReleaseThrowable");

            }

        }
    }

    IEnumerator PunchTimer()
    {
        punchCooldown = true;
        yield return new WaitForSeconds(3f);
        punchCooldown = false;
    }

    IEnumerator ReleaseThrowable()
    {
        yield return new WaitForSeconds(1f);
        Rigidbody2D throwRB = throwable.gameObject.GetComponent<Rigidbody2D>(); 

        float distance = transform.position.x - player.position.x;
        transform.rotation = Quaternion.Euler(0, distance > 0 ? 0 : 180, 0);
        throwable.parent = null;
        throwRB.gravityScale = 1;
        Vector2 direction = (player.position - throwable.transform.position).normalized;
        direction += Vector2.up * 0.1f;
        direction = direction.normalized;
        throwRB.velocity = direction*15f;
        state = 1;
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(3); // cooldown
        canGrab = true;
    }
}
