using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
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
    int[] items = new int[3];
    /*
     * 0 - nothing
     * 1 - coffee
     * 2 - drug
     * 3 - pen
     * 4 - strength
     * 5 - jump
     * 6 - stick
     * 7 - scissor
     * 8 - slam
    */
    public Sprite[] itemsprites;
    public Image[] itemholders;

    bool coffemode;
    bool steroidmode;

    void Start()
    {
        items[0] = 0;
        items[1] = 0;
        items[2] = 0;
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody2D>();
        col = this.GetComponent<Collider2D>();
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        Restart();
    }

    void Restart()
    {
        itemholders[0].sprite = itemsprites[items[0]];
        itemholders[1].sprite = itemsprites[items[2]];
        itemholders[1].sprite = itemsprites[items[2]];
    }

    // Update is called once per frame
    void Update()
    {
        if (!stateManager.started || stateManager.paused) return;
        if (Input.GetKey(stateManager.keybinds[4]) && Input.GetKey(stateManager.keybinds[5]))
            input = 0;
        else if (Input.GetKey(stateManager.keybinds[4]))
            input = -1;
        else if (Input.GetKey(stateManager.keybinds[5]))
            input = 1;
        else
            input = 0;

        anim.SetBool("running", input != 0);
        bool pointerOverUI = EventSystem.current.IsPointerOverGameObject();
  
        if (!pointerOverUI && Input.GetKey(stateManager.keybinds[3])) anim.SetTrigger("jab");
        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Punch") || anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Jab"))
            input /= 10;
        
        
        rb.velocity = new Vector2(input * speed, 0);
        if (input != 0) sprite.flipX = input < 0;
    }

    void AddItem(int itemid)
    {
        items[2] = items[1];
        items[1] = items[0];
        items[0] = itemid;
    }

    void Coffee()
    {
        StartCoroutine("CoffeeTimer");
    }

    IEnumerator CoffeeTimer()
    {
        coffemode = true;
        yield return new WaitForSeconds(3);
        coffemode = false;
    }

    void Steroid()
    {
        StartCoroutine("SteroidTimer");
    }

    IEnumerator SteroidTimer()
    {
        steroidmode = true;
        yield return new WaitForSeconds(3);
        steroidmode = false;
    }
}
