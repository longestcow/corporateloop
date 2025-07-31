using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    Collider2D col;
    Animator anim;
    SpriteRenderer sprite;
    float input;
    public float speed;
    public bool running;
    StateManager stateManager;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody2D>();
        col = this.GetComponent<Collider2D>();
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stateManager.started || stateManager.paused) return;
        input = Input.GetAxisRaw("Horizontal");

        anim.SetBool("running", input != 0);
        bool pointerOverUI = EventSystem.current.IsPointerOverGameObject();
  
        if (!pointerOverUI && Input.GetMouseButtonDown(0)) anim.SetTrigger("punch");
        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Punch") || anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Jab"))
            input /= 10;
        
        
        rb.velocity = new Vector2(input * speed, 0);
        if (input != 0) sprite.flipX = input < 0;
    }
}
