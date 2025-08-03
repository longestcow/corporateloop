using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombPush : MonoBehaviour
{
    // Start is called before the first frame update
    Player player;
    Collider2D col;
    Rigidbody2D rb;
    Boss boss;
    void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
        boss = GameObject.Find("Balls").GetComponent<Boss>();
        col = GetComponent<Collider2D>();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (col.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            Vector2 direction = transform.position - boss.transform.position;
            rb.velocity = direction.normalized * -6;
        }
    }
}
