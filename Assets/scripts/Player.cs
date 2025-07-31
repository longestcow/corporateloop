using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    Camera cam;
    Rigidbody2D rb;
    Collider2D col;
    Animator anim;
    SpriteRenderer sprite;
    float input;
    public float speed;
    public float health = 20;
    StateManager stateManager;
    int[] items = new int[3];
    /*
     * 0 - nothing
     * 1 - coffee - implemented
     * 2 - drug
     * 3 - pen
     * 4 - strength
     * 5 - jump - implemented
     * 6 - stick
     * 7 - scissor
     * 8 - slam
    */
    public Sprite[] itemsprites;
    public Image[] itemholders;
    int[] itemcooldown = new int[3];

    bool coffemode;
    bool steroidmode;

    void Start()
    {
        items[0] = 1;
        items[1] = 5;
        items[2] = 1;
        itemcooldown[0] = -1;
        itemcooldown[1] = -1;
        itemcooldown[2] = -1;
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        col = this.gameObject.GetComponent<Collider2D>();
        cam = Camera.main;
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        Restart();
    }

    void Restart()
    {
        itemholders[0].sprite = itemsprites[items[0]];
        itemholders[1].sprite = itemsprites[items[1]];
        itemholders[2].sprite = itemsprites[items[2]];
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

        cam.transform.position = new Vector3(Mathf.Lerp(cam.transform.position.x, input * speed / 10f, 0.01f), cam.transform.position.y, -10);
        
        rb.velocity = new Vector2(input * speed, rb.velocity.y);
        if (input != 0) sprite.flipX = input < 0;

        if (Input.GetKeyDown(stateManager.keybinds[0])) UseItem(0);
        if (Input.GetKeyDown(stateManager.keybinds[1])) UseItem(1);
        if (Input.GetKeyDown(stateManager.keybinds[2])) UseItem(2);
    }

    private void FixedUpdate()
    {
        itemcooldown[0]--;
        itemcooldown[1]--;
        itemcooldown[2]--;

        itemholders[0].color = (itemcooldown[0] >= 0)? new Color(0.3f, 0.3f, 0.3f, 1) : new Color(1 ,1 ,1 ,1); 
        itemholders[1].color = (itemcooldown[1] >= 0)? new Color(0.3f, 0.3f, 0.3f, 1) : new Color(1 ,1 ,1 ,1); 
        itemholders[2].color = (itemcooldown[2] >= 0)? new Color(0.3f, 0.3f, 0.3f, 1) : new Color(1 ,1 ,1 ,1); 
    }

    void AddItem(int itemid)
    {
        items[2] = items[1];
        items[1] = items[0];
        items[0] = itemid;
    }

    void UseItem(int outofthree)
    {
        if (itemcooldown[outofthree] >= 0) return;

        if (items[outofthree] == 1)
        {
            Coffee();
            itemcooldown[outofthree] = 5 * 60;
        }
        if (items[outofthree] == 2)
        {
            Steroid();
            itemcooldown[outofthree] = 5 * 60;
        }



        if (items[outofthree] == 5)
        {
            Jump();
            itemcooldown[outofthree] = 2 * 60;
        }
    }

    void Coffee()
    {
        StartCoroutine("CoffeeTimer");
    }

    IEnumerator CoffeeTimer()
    {
        if (speed == 12) speed = 16;
        if (speed == 8) speed = 12;
        if (speed == 4) { coffemode = true; speed = 8; }
        yield return new WaitForSeconds(3);
        if (speed == 8) { coffemode = false; speed = 4; }
        if (speed == 12) speed = 8;
        if (speed == 16) speed = 12;
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




    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 10f);
    }
}
