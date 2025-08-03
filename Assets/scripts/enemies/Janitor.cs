using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Janitor : MonoBehaviour
{
    public int state = 0;
    public float speed;
    // cycle based
    // every cycle, walk or sweep or idle
    // sweep every 2 cycles if you can
    // 0 idle (1 second)
    // 1 walk (2 seconds)
    // 2 sweep (animation length)
    // always idle after 0 and 2

    Animator anim;
    Rigidbody2D rb;
    public GameObject sweepSpot, water;
    public float chosenSpot = 0;
    int cycleSinceWater = 0;
    Coroutine currentCycle;
    public GameObject hp;
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentCycle = StartCoroutine(Cycle());
        // start idle anim
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (GameObject.Find("KillZone").transform.position.y > transform.position.y)
        {
            hp.SetActive(true);
            Destroy(this.gameObject, 0.1f);
        }

        if (state == 1 && Mathf.Abs(transform.position.x - chosenSpot) < 0.5f)
        {
            state = 0;
            anim.SetBool("running", false);
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else if (anim.GetBool("running"))
        {
            if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Dead"))
            {
                rb.velocity = Vector2.zero;
                return;
            }
            ;
            rb.velocity = new Vector2(Mathf.Sign(chosenSpot - transform.position.x) * speed, rb.velocity.y);
            transform.rotation = Quaternion.Euler(0, rb.velocity.x < 0 ? 0 : 180, 0);
        }
        if (currentCycle == null) currentCycle = StartCoroutine(Cycle());
    }

    IEnumerator Cycle()
    {
        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Dead"))  //dudes dead, waiting to get destroyed now
            yield break;
        if (state == 0) // idle
        {
            anim.SetBool("running", false);
            rb.velocity = new Vector2(0, rb.velocity.y);
            yield return new WaitForSeconds(1);
            RaycastHit2D playerHit = Physics2D.Raycast(new Vector2(10, transform.position.y), Vector2.left, 20, LayerMask.GetMask("Player"));

            if (GameObject.FindGameObjectsWithTag("water").Length < 2 && playerHit.collider!=null)
            {
                if (GameObject.FindGameObjectsWithTag("water").Length == 0 && Random.value < 0.8f)
                    state = 2;
                else if (cycleSinceWater > 0 && Random.value < 0.5f)
                    state = 2;
                else if (cycleSinceWater > 2)
                    state = 2;
                else
                    state = 1;
            }
            else
                state = 1;
            if(playerHit.collider!=null)
                cycleSinceWater += 1;
        }
        if (state == 1)
        {
            if (!anim.GetBool("running"))
            {
                anim.SetBool("running", true);
                if (transform.position.x < 0)
                    chosenSpot = Random.Range(1, 6);
                else
                    chosenSpot = Random.Range(-1, -6);
                
            }
        }
        if (state == 2)
        {
            anim.SetTrigger("sweep");
            yield return new WaitForSeconds(0.2f);
            if (water == null || sweepSpot==null) yield break;
            GameObject obj = Instantiate(water, sweepSpot.transform);
            obj.transform.parent = null;
            yield return new WaitForSeconds(1.5f);
            state = 0;
            cycleSinceWater = 0;

        }
        currentCycle = null;
    }


    
}
