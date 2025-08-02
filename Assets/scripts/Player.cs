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
    public Scrollbar healthbar;
    public GameObject throwPoint;
    StateManager stateManager;
    int[] items = new int[3];
    /*
     * 0 - nothing
     * 1 - coffee - implemented
     * 2 - drug
     * 3 - pen
     * 4 - strength - implemented
     * 5 - jump - implemented
     * 6 - stick
     * 7 - scissor
     * 8 - slam
    */
    public Sprite[] itemsprites;
    public Image[] itemholders;
    int[] itemcooldown = new int[3];

    bool coffeemode;
    bool steroidmode;
    bool punchCooldown;
    bool grabbing = false;
    public bool iframe = false, dead = false;
    int groundLayer;

    [HideInInspector] public Throwable throwable = null;

    void Start()
    {
        items[0] = 1;
        items[1] = 5;
        items[2] = 4;
        itemcooldown[0] = -1;
        itemcooldown[1] = -1;
        itemcooldown[2] = -1;
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        col = this.gameObject.GetComponent<Collider2D>();
        cam = Camera.main;
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        groundLayer = LayerMask.GetMask("Floor");
        Restart();
    }

    void Restart() //called after you die and restart from the beginning of the level, don't restart scene
    {
        dead = false;
        anim.SetBool("dead", false);
        itemholders[0].sprite = itemsprites[items[0]];
        itemholders[1].sprite = itemsprites[items[1]];
        itemholders[2].sprite = itemsprites[items[2]];
    }

    void Update()
    {
        input = 0;
        input += Input.GetKey(stateManager.keybinds[4]) ? -1 : Input.GetKey(stateManager.keybinds[5]) ? 1 : 0;
        if (input != 0) transform.rotation = Quaternion.Euler(0, input > 0 ? 0 : 180, 0);

        if (!stateManager.started || stateManager.paused || iframe || dead || grabbing) return;


        anim.SetBool("running", input != 0);
        bool pointerOverUI = EventSystem.current.IsPointerOverGameObject();

        if (!pointerOverUI && Input.GetKeyDown(stateManager.keybinds[3]) && !punchCooldown && col.IsTouchingLayers(groundLayer))
            anim.SetTrigger("punch");


        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Punch") || anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Jab"))
            input /= 10;

        //Cool camera effect
        cam.transform.position = new Vector3(Mathf.Lerp(cam.transform.position.x, input * speed / 10f, 0.01f), cam.transform.position.y, -10);

        rb.velocity = new Vector2(input * speed, rb.velocity.y);

        //For checking if items are being used
        if (Input.GetKeyDown(stateManager.keybinds[0])) UseItem(0);
        if (Input.GetKeyDown(stateManager.keybinds[1])) UseItem(1);
        if (Input.GetKeyDown(stateManager.keybinds[2])) UseItem(2);

        //Sets healthbar size
        healthbar.size = (health / 20f);
    }

    private void FixedUpdate()
    {
        //for cooldown of items
        itemcooldown[0]--;
        itemcooldown[1]--;
        itemcooldown[2]--;

        //for making items under cooldown a darker colour
        itemholders[0].color = (itemcooldown[0] >= 0) ? new Color(0.3f, 0.3f, 0.3f, 1) : new Color(1, 1, 1, 1);
        itemholders[1].color = (itemcooldown[1] >= 0) ? new Color(0.3f, 0.3f, 0.3f, 1) : new Color(1, 1, 1, 1);
        itemholders[2].color = (itemcooldown[2] >= 0) ? new Color(0.3f, 0.3f, 0.3f, 1) : new Color(1, 1, 1, 1);

    }

    void AddItem(int itemid) //call with id of the item to add 
    {
        items[2] = items[1];
        items[1] = items[0];
        items[0] = itemid;
    }

    void UseItem(int outofthree) //called when an item is being used
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


        if (items[outofthree] == 4)
        {
            Grab();
            //no cooldown
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
        if (speed == 4) { coffeemode = true; speed = 8; }
        yield return new WaitForSeconds(3);
        if (speed == 8) { coffeemode = false; speed = 4; }
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
        anim.SetTrigger("jump");
    }

    void Grab()
    {
        if (throwable == null)
            return;
        if (throwable.active) return;
        rb.velocity = Vector2.zero;
        grabbing = true;
        throwable.enemy = false;
        throwable.transform.parent = throwPoint.transform;
        throwable.transform.localPosition = Vector3.zero;
        throwable.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        anim.SetTrigger("grab");
        StartCoroutine("ReleaseThrowable");
    }

    public void GrabUtil(bool near)
    {
        if (items[0] == 4)
        {
            if (near)
                itemholders[0].transform.localScale = new Vector3(1.2f, 1.2f, 0);
            else
                itemholders[0].transform.localScale = new Vector3(1, 1, 0);
        }
        if (items[1] == 4)
        {
            if (near)
                itemholders[1].transform.localScale = new Vector3(1.2f, 1.2f, 0);
            else
                itemholders[1].transform.localScale = new Vector3(1, 1, 0);
        }
        if (items[2] == 4)
        {
            if (near)
                itemholders[2].transform.localScale = new Vector3(1.2f, 1.2f, 0);
            else
                itemholders[2].transform.localScale = new Vector3(1, 1, 0);
        }
        
    }

    IEnumerator ReleaseThrowable()
    {
        yield return new WaitForSeconds(1f);
        if (throwable == null)
        {
            grabbing = false;
            yield break;
        }
        Rigidbody2D throwRB = throwable.gameObject.GetComponent<Rigidbody2D>();
        throwable.transform.parent = null;
        throwRB.gravityScale = 1;
        Vector2 direction = Vector2.right * (transform.rotation.y == 0 ? 1 : -1);
        direction += Vector2.up * 0.1f;
        direction = direction.normalized;
        throwRB.velocity = direction * 10f;
        rb.velocity = Vector3.zero;
        grabbing = false;


    }

    public void Hurt(float dmg, int id)
    {
        health -= dmg;

        if (health <= 0)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("dead", true);
            dead = true;
            //AddItem                 ------------------------------------------------------------- RPC idk how the indexing works so you can do this
            if (id == 0 || id == 1 || id == 3 || id == 4)
            {
                anim.SetTrigger("basicDead");
            }
            else if (id == 2)
                anim.SetTrigger("slip");
            else // need to add jump off and elevator
                anim.SetTrigger("basicDead");
        }
        else if (id == 2)
        {
            anim.SetTrigger("slip");
            StartCoroutine(Slip());
        }
        // ids
        // 0 throwable
        // 1 brute
        // 2 slip
        // 3 staple


    }
    public IEnumerator Slip()
    {
        yield return new WaitForSeconds(0.1f);
        rb.velocity = Vector2.zero;
        sprite.color = new Color(0.9f, 0.9f, 0.9f);
        iframe = true;
        yield return new WaitForSeconds(1.5f);
        sprite.color = new Color(1, 1, 1);
        iframe = false;
    }



}
