using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public static int runcount;
    Camera cam;
    Rigidbody2D rb;
    Collider2D col;
    Animator anim;
    SpriteRenderer sprite;
    public SpriteRenderer darkness;
    float input;
    public float speed;
    public float strength;
    public float health = 10;
    public float maxhealth = 10;
    public Scrollbar healthbar;
    public GameObject throwPoint;
    StateManager stateManager;
    public int[] items = new int[3];
    bool slamming = false;
    /*
     * 0 - nothing
     * 1 - coffee - implemented
     * 2 - drug - implemented - obtainable
     * 3 - pen - implemented - obtainable
     * 4 - strength - implemented - obtainable
     * 5 - jump - implemented - obtainable
     * 6 - stick - uh oh
     * 7 - scissor - cancelled
     * 8 - slam - implemented - rpc make it obtainable
    */
    public Sprite[] itemsprites;
    public Image[] itemholders;
    int[] itemcooldown = new int[3];

    public bool coffeemode;
    bool steroidmode;
    bool punchCooldown;
    bool grabbing = false;
    public bool iframe = false, dead = false;
    int groundLayer;

    [HideInInspector] public Throwable throwable = null;

    public GameObject pen;
    public bool haspen;
    public int pencount;

    public GameObject slampng;
    public GameObject exclam;

    public bool nextrooming;
    public bool nextroomed;
    public static bool startedelevator;
    public bool inelevator;
    public bool stuckinelevator;
    public bool fallingelevator;
    float elevatorspeed;

    void Start()
    {
        runcount = 0;
        items[0] = 1;
        items[1] = 8;
        items[2] = 0;
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
        runcount++;
        dead = false;
        anim.SetBool("dead", false);
        itemholders[0].sprite = itemsprites[items[0]];
        itemholders[1].sprite = itemsprites[items[1]];
        itemholders[2].sprite = itemsprites[items[2]];
        pencount = 0;
        if (items[0] == 3) pencount++;
        if (items[1] == 3) pencount++;
        if (items[2] == 3) pencount++;
        health = maxhealth;
        transform.position = new Vector3(-5, -2, 0);
        sprite.color = new Color(1, 1, 1, 1);
        stateManager.SetDescriptions();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Restart();

        if (inelevator || stuckinelevator || fallingelevator) return;

        if (StateManager.coolstun) sprite.color = new Color(0, 0, 0, 1);
        else sprite.color = new Color(1, 1, 1, sprite.color.a);

        input = 0;
        input += Input.GetKey(stateManager.keybinds[4]) ? -1 : Input.GetKey(stateManager.keybinds[5]) ? 1 : 0;


        cam.transform.position = new Vector3(Mathf.Lerp(cam.transform.position.x, input * speed / 10f, 0.01f), Mathf.Lerp(cam.transform.position.y, 7 * Mathf.Round(transform.transform.position.y/7), 0.01f), -10);

        if (!stateManager.enemyStun && input != 0 && stateManager.started && !stateManager.paused && !dead && !iframe && !slamming) 
            transform.rotation = Quaternion.Euler(0, input > 0 ? 0 : 180, 0);

        if (!stateManager.started || stateManager.paused || iframe || dead || grabbing || nextrooming || nextroomed || slamming) return;

        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Punch") || anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Jab"))
            input /= 10;
        anim.SetBool("running", input != 0);
        rb.velocity = new Vector2(input * speed, rb.velocity.y);

        if (coffeemode)
        {
            return;
        }

            
        bool pointerOverUI = EventSystem.current.IsPointerOverGameObject();
        if (!pointerOverUI && Input.GetKeyDown(stateManager.keybinds[3]) && !punchCooldown && col.IsTouchingLayers(groundLayer)) 
            anim.SetTrigger("punch");
       



        //For checking if items are being used
        if (Input.GetKeyDown(stateManager.keybinds[0])) UseItem(0);
        if (Input.GetKeyDown(stateManager.keybinds[1])) UseItem(1);
        if (Input.GetKeyDown(stateManager.keybinds[2])) UseItem(2);

        //Room exit
        if (col.IsTouchingLayers(LayerMask.GetMask("exit")) && Input.GetKeyDown(stateManager.keybinds[6]))
        {
            StartCoroutine("NextRoom");
        }

        //Elevator
        if (col.IsTouchingLayers(LayerMask.GetMask("elevator")) && Input.GetKeyDown(stateManager.keybinds[6]))
        {
            StartCoroutine("Elevator");
        }
    }

    private void FixedUpdate()
    {
        //for cooldown of items
        itemcooldown[0]--;
        itemcooldown[1]--;
        itemcooldown[2]--;

        //for making items under cooldown a darker colour
        if (!coffeemode)
        {
            itemholders[0].color = (itemcooldown[0] >= 0) ? new Color(0.3f, 0.3f, 0.3f, 1) : new Color(1, 1, 1, 1);
            itemholders[1].color = (itemcooldown[1] >= 0) ? new Color(0.3f, 0.3f, 0.3f, 1) : new Color(1, 1, 1, 1);
            itemholders[2].color = (itemcooldown[2] >= 0) ? new Color(0.3f, 0.3f, 0.3f, 1) : new Color(1, 1, 1, 1);
        }

        if (items[0] == 3 && itemcooldown[0] < 0) itemholders[0].color = (pencount <= 0) ? new Color(0.3f, 0.3f, 0.3f, 1) : new Color(1, 1, 1, 1);
        if (items[1] == 3 && itemcooldown[1] < 0) itemholders[1].color = (pencount <= 0) ? new Color(0.3f, 0.3f, 0.3f, 1) : new Color(1, 1, 1, 1);
        if (items[2] == 3 && itemcooldown[2] < 0) itemholders[2].color = (pencount <= 0) ? new Color(0.3f, 0.3f, 0.3f, 1) : new Color(1, 1, 1, 1);

        if (nextrooming) sprite.color = new Color(1,1,1, sprite.color.a - (1f / 60f));
        else if (nextroomed) sprite.color = new Color(1, 1, 1, sprite.color.a + (1f / 60f));
        
        if (!dead) darkness.color = new Color(0, 0, 0, Mathf.Clamp01(darkness.color.a - (1f / 60f)));
        else if (dead) darkness.color = new Color(0, 0, 0, Mathf.Clamp01(darkness.color.a + (1f / 60f)));

        if (inelevator) { cam.transform.position = new Vector3(0, cam.transform.position.y + elevatorspeed, -10); elevatorspeed *= 0.99f; }
        if (stuckinelevator) cam.transform.position = new Vector3(0, cam.transform.position.y, -10);
        if (fallingelevator) { cam.transform.position = new Vector3(Random.Range(-0.5f, 0.5f), cam.transform.position.y - elevatorspeed, -10); elevatorspeed += 0.025f; }


        if ((col.IsTouchingLayers(LayerMask.GetMask("exit")) && !nextroomed && !nextrooming) || (col.IsTouchingLayers(LayerMask.GetMask("elevator")) && (!inelevator && !stuckinelevator && !fallingelevator)))
        {
            exclam.SetActive(true);
        }
        else
        {
            exclam.SetActive(false);
        }
    }

    void AddItem(int itemid) //call with id of the item to add 
    {
        items[2] = items[1];
        items[1] = items[0];
        items[0] = itemid;
        stateManager.SetDescriptions();
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
        if (items[outofthree] == 3)
        {
            PenThrow();
            itemcooldown[outofthree] = 8 * 60;
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


        if (items[outofthree] == 8)
        {
            Slam();
            itemcooldown[outofthree] = 4 * 60;
        }
    }

    void Coffee()
    {
        StartCoroutine("CoffeeTimer");
    }

    IEnumerator CoffeeTimer()
    {
        //for making items under cooldown a darker colour
        
        itemholders[0].color = new Color(0.3f, 0.3f, 0.3f, 1);
        itemholders[1].color = new Color(0.3f, 0.3f, 0.3f, 1);
        itemholders[2].color = new Color(0.3f, 0.3f, 0.3f, 1);
        if (speed == 12) speed = 16;
        if (speed == 8) speed = 12;
        if (speed == 4) { coffeemode = true; speed = 8; anim.speed = 1.4f;}
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.7f);
        yield return new WaitForSeconds(2.5f);
        itemholders[0].color = new Color(1, 1, 1, 1);
        itemholders[1].color = new Color(1, 1, 1, 1);
        itemholders[2].color = new Color(1, 1, 1, 1);
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
        if (speed == 8) { coffeemode = false; speed = 4; anim.speed = 1;}
        if (speed == 12) speed = 8;
        if (speed == 16) speed = 12;
    }

    void Steroid()
    {
        StartCoroutine("SteroidTimer");
    }

    IEnumerator SteroidTimer()
    {
        if (stateManager.playerPunchDmg == 24) stateManager.playerPunchDmg = 72;
        if (stateManager.playerPunchDmg == 8) stateManager.playerPunchDmg = 24;
        if (stateManager.playerPunchDmg == 2) { steroidmode = true; stateManager.playerPunchDmg = 8; }
        yield return new WaitForSeconds(3);
        if (stateManager.playerPunchDmg == 8) { steroidmode = false; stateManager.playerPunchDmg = 2; }
        if (stateManager.playerPunchDmg == 24) stateManager.playerPunchDmg = 8;
        if (stateManager.playerPunchDmg == 72) stateManager.playerPunchDmg = 24;
    }

    void PenThrow()
    {
        if (pencount > 0)
        {
            Instantiate(pen, this.transform.position, this.transform.rotation, null);
            pencount--;
        }
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
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 10f);
        anim.SetTrigger("jump");
    }


    void Slam()
    {

        StartCoroutine(slamAttack());
        pencount--;
    }
    IEnumerator slamAttack()
    {
        slamming = true;
        rb.velocity = new Vector2(0, 5f);
        anim.SetBool("running", false);
        anim.SetTrigger("slam");
        yield return new WaitForSeconds(0.2f);
        rb.velocity = new Vector2(0, -8f);
        yield return new WaitForSeconds(0.12f);
        Instantiate(slampng, this.transform.position, Quaternion.Euler(this.transform.rotation.eulerAngles.x, 0, this.transform.rotation.eulerAngles.z), null);
        Instantiate(slampng, this.transform.position, Quaternion.Euler(this.transform.rotation.eulerAngles.x, 180, this.transform.rotation.eulerAngles.z), null);
        yield return new WaitForSeconds(0.2f);
        slamming = false;

        // camera shake
    }

    public void Hurt(float dmg, int id)
    {
        health -= dmg;

        if (health <= 0)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("dead", true);
            dead = true;
            StartCoroutine("death");
            if (id == 0)
            {
                anim.SetTrigger("basicDead");
                AddItem(4);
            }
            if (id == 1)
            {
                anim.SetTrigger("basicDead");
                AddItem(2);
            }
            if (id == 2)
            {
                anim.SetTrigger("slip");
                AddItem(5);
            }
            if (id == 3)
            {
                anim.SetTrigger("basicDead");
                AddItem(3);
            }
            if (id == 4)
            {
                anim.SetTrigger("basicDead");
            }
            if (id == 5)
            {
                anim.SetTrigger("basicDead");
            }
            if (id == 6)
            {
                anim.SetTrigger("basicDead");
            }
            else // need to add jump off and elevator
            {
                anim.SetTrigger("basicDead");
            }
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
        // 4 paper
        // 5 jump off
        // 6 elevator


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

    public IEnumerator NextRoom()
    {
        nextrooming = true;
        rb.velocity = Vector2.zero;
        anim.SetBool("running", false);
        yield return new WaitForSeconds(0.7f);
        nextrooming = false;
        nextroomed = true;
        transform.position = transform.position + new Vector3(0,7,0);
        if (Mathf.Round(transform.position.y / 7) > 14) // boss start
        {
            stateManager.bossStart();
        }
        stateManager.clearPuddles();
        yield return new WaitForSeconds(0.7f);
        nextroomed = false;
    }
    public IEnumerator Elevator()
    {
        nextroomed = true;
        startedelevator = true;
        rb.velocity = Vector2.zero;
        anim.SetBool("running", false);
        yield return new WaitForSeconds(2);
        inelevator = true;
        nextroomed = false;
        elevatorspeed = 0.1f;
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        yield return new WaitForSeconds(2);
        inelevator = false;
        stuckinelevator = true;
        yield return new WaitForSeconds(0.5f);
        stuckinelevator = false;
        fallingelevator = true;
        elevatorspeed = 0.5f;
        yield return new WaitForSeconds(1);
        dead = true;
        AddItem(8);
        StartCoroutine("death");
        health = 0;
        yield return new WaitForSeconds(1);
        fallingelevator = false;
    }

    public IEnumerator death()
    {
        SFXManager.instance.fadeOut();
        yield return new WaitForSeconds(3);
        SFXManager.instance.fadeIn();
        Restart();
    }

}
