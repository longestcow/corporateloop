using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stapler : MonoBehaviour
{
    public int state = 0;
    // 0 idle
    // 1 player found 
    // 2 PRPRPRPRPRPRPRPRR
    Animator anim;
    Transform player;
    Rigidbody2D rb;
    bool punchCooldown = false;
    public GameObject stapleSpawn, staple;
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
            float distance = transform.position.x - player.position.x; // if positive = player to the left, vice versa
            transform.rotation = Quaternion.Euler(0, distance > 0 ? 0 : 180, 0);

            if (!punchCooldown)
            { 
                anim.SetTrigger("punch");
                StartCoroutine("PunchTimer");
            }

        }
    }

    IEnumerator PunchTimer()
    {
        punchCooldown = true;
        yield return new WaitForSeconds(0.6f);
        GameObject stapleObj = Instantiate(staple, stapleSpawn.transform);
        stapleObj.transform.localPosition = Vector3.zero;
        stapleObj.transform.parent = null;
        stapleObj.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(stapleObj.transform.position.x - player.position.x)*-5f, 0);
        yield return new WaitForSeconds(2.5f);
        Destroy(stapleObj);
        punchCooldown = false;
    }


}
