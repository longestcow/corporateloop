using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public GameObject youwin;
    public bool started = false;
    public float health;
    public Scrollbar healthbar;
    public Image healthimg;
    public Sprite nonbreakhealthimg;
    public Sprite breakhealthimg;
    Animator anim;
    int state = 0;
    bool idled = false, switchSided = false, left = true, bombing = false;
    float waitTime = 2f;
    bool busy;
    public Transform TargLeft, TargRight;
    public Transform moneyParent, bombSpot, paperSpot;
    public Transform[] enemySpots;
    public GameObject moneyPrefab, bombPrefab, paperPrefab;
    public GameObject[] enemyPrefabs;
    StateManager stateManager;
    // 0 startednt idle
    // 1 cutscene (consume dudes)
    // 2 idle
    // - switch sides (every 15 seconds) (automatic in idle)
    // 3 pick random attack
    // attacks
    // 0 paper stacks
    // 1 spawn enemy
    // 2 bomb
    // 3 money berdley

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        health = 50;
        switchSided = true;
        left = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
        {
            healthbar.gameObject.SetActive(false);
            return;
        }
        health = Mathf.Clamp(health, 0, 150);

        if (GameObject.Find("player").GetComponent<Player>().dead)
        {
            started = false;
            health = 50;
            state = 0;
        }

        if (health > 50) healthimg.sprite = breakhealthimg;
        else healthimg.sprite = nonbreakhealthimg;

        if (busy) return;
        if (killzone.killtime) return;

        if (state == 2)
        {
            if (!idled)
            {
                StartCoroutine(Idle());
                if (switchSided)
                    StartCoroutine(switchSide());
            }
        }
        if (state == 3)
        {
            // int r = Random.Range(0, 3);
            idled = false;
            int r = Random.Range(0,3);
            if (r == 0) // paper stack
            {
                //instantiate paper stack projectile
                StartCoroutine("Paper");
                waitTime = 2f;

            }
            else if (r == 1)
            {
                //instantiate 1 dude in both sides
                StartCoroutine("Enemies");
                waitTime = 2f;
            }
            else if (r == 2)
            {
                StartCoroutine("Money");
                waitTime = 8;
            }
            else if (!bombing)  
            {
                // instantiate 1 bomb 5m away from boss
                StartCoroutine(Bomb());
                waitTime = 3f;
            }


        }

    }

    private void FixedUpdate()
    {
        healthbar.size = (0.34f * health / 50 );
        /*transform.position = new Vector3((Mathf.Lerp(transform.position.x, (left?TargLeft.position.x:TargRight.position.x), 0.8f)), transform.position.y, 0);
        if (transform.position.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else transform.rotation = Quaternion.Euler(0, 180, 0);*/
    }

    public void Cutscene()
    {
        // do your magic
        started = true;
        healthbar.gameObject.SetActive(true);
        state = 2;
        idled = false;
        switchSided = false; left = true;
        bombing = false;
        busy = false;
        StartCoroutine("KillCutscene");
    }

    IEnumerator KillCutscene()
    {
        killzone.killtime = true;
        yield return new WaitForSeconds(3);
    }

    IEnumerator Idle()
    {
        idled = true;
        anim.SetTrigger("smoke");
        yield return new WaitForSeconds(5f);
        state = 3;
    }

    IEnumerator switchSide()
    {
        switchSided = false;
        yield return new WaitForSeconds(5f);
        switchSided = true;
        left = !left;
    }
    IEnumerator Money()
    {
        anim.SetTrigger("money");
        waitTime = 8;
        busy = true;
        yield return new WaitForSeconds(0.65f);
        idled = false;
        busy = false;
        bombing = false;
        state = 2;// idle
        int i = 0;
        foreach (Transform child in moneyParent)
        {
            i += 1;
            GameObject moneyObj = Instantiate(moneyPrefab, child);
            moneyObj.transform.localPosition = Vector3.zero;
            moneyObj.GetComponent<MoneyProjectile>().StartCoroutine(moneyObj.GetComponent<MoneyProjectile>().launch(i));
        }

    }
    IEnumerator Bomb()
    {
        bombing = true;
        GameObject obj = Instantiate(bombPrefab);
        obj.transform.position = new Vector3(bombSpot.position.x, bombSpot.position.y + 20, 0);
        yield return new WaitForSeconds(4f);
        state = 2;// idle
        idled = false;
        bombing = false;
        Destroy(obj);

    }
    IEnumerator Enemies()
    {
        bombing = true;
        Instantiate(enemyPrefabs[Random.Range(0,3)],enemySpots[0]);
        Instantiate(enemyPrefabs[Random.Range(0,2)],enemySpots[1]);
        idled = false;
        bombing = false;
        state = 2;// idle
        yield return new WaitForSeconds(4f);

    }
    IEnumerator Paper()
    {
        bombing = true;
        Instantiate(paperPrefab,paperSpot.position + new Vector3(Random.Range(-10,10),Random.Range(-0.5f, 0.5f),0),Quaternion.Euler(0,0,0) ,null);
        Instantiate(paperPrefab,paperSpot.position + new Vector3(Random.Range(-10,10),Random.Range(-0.5f, 0.5f),0),Quaternion.Euler(0,0,0) ,null);
        Instantiate(paperPrefab,paperSpot.position + new Vector3(Random.Range(-10,10),Random.Range(-0.5f, 0.5f),0),Quaternion.Euler(0,0,0) ,null);
        Instantiate(paperPrefab,paperSpot.position + new Vector3(Random.Range(-10,10),Random.Range(-0.5f, 0.5f),0),Quaternion.Euler(0,0,0) ,null);
        Instantiate(paperPrefab,paperSpot.position + new Vector3(Random.Range(-10,10),Random.Range(-0.5f, 0.5f),0),Quaternion.Euler(0,0,0) ,null);
        Instantiate(paperPrefab,paperSpot.position + new Vector3(Random.Range(-10,10),Random.Range(-0.5f, 0.5f),0),Quaternion.Euler(0,0,0) ,null);
        Instantiate(paperPrefab,paperSpot.position + new Vector3(Random.Range(-10,10),Random.Range(-0.5f, 0.5f),0),Quaternion.Euler(0,0,0) ,null);
        Instantiate(paperPrefab,paperSpot.position + new Vector3(Random.Range(-10,10),Random.Range(-0.5f, 0.5f),0),Quaternion.Euler(0,0,0) ,null);
        Instantiate(paperPrefab,paperSpot.position + new Vector3(Random.Range(-10,10),Random.Range(-0.5f, 0.5f),0),Quaternion.Euler(0,0,0) ,null);
        Instantiate(paperPrefab,paperSpot.position + new Vector3(Random.Range(-10,10),Random.Range(-0.5f, 0.5f),0),Quaternion.Euler(0,0,0) ,null);
        Instantiate(paperPrefab,paperSpot.position + new Vector3(Random.Range(-10,10),Random.Range(-0.5f, 0.5f),0),Quaternion.Euler(0,0,0) ,null);
        Instantiate(paperPrefab,paperSpot.position + new Vector3(Random.Range(-10,10),Random.Range(-0.5f, 0.5f),0),Quaternion.Euler(0,0,0) ,null);
        Instantiate(paperPrefab,paperSpot.position + new Vector3(Random.Range(-10,10),Random.Range(-0.5f, 0.5f),0),Quaternion.Euler(0,0,0) ,null);
        idled = false;
        bombing = false;
        state = 2;// idle
        yield return new WaitForSeconds(4f);

    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (!collision.gameObject.name.Contains("hitbox")) return;
        if (collision.gameObject.layer == 7)//throwable
        {
            Throwable throwable = collision.transform.GetComponentInParent<Throwable>();
            if (!throwable.grounded && throwable.active && !throwable.enemy)
            {
                Hurt(8);
                // throwable.throwTimesFR += 1;

            }
            //if (throwable.throwTimesFR >= throwable.health)
            //Destroy(throwable.transform.gameObject);
        }
        if (collision.gameObject.layer == 12)//pen
        {
            if (collision.gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude > 1)
            {
                collision.gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                Hurt(14);
            }
        }
        if (collision.gameObject.layer == 3)//player punch
        {
            Hurt(stateManager.playerPunchDmg);
        }
        if (collision.gameObject.layer == 13)
        {

        }
        if (collision.gameObject.layer == 17) // bomb
        {
            Hurt(15);
        } 


    }
    void Hurt(float dmg)
        {
            dmg = dmg / 2;
            health -= dmg;
            if (health <= 0)
            {
                stateManager.StartCoroutine("CoolHitStun");
                anim.SetTrigger("dead");
                busy = true;
                youwin.SetActive(true);
                Destroy(this.gameObject, 5f);
                Destroy(this);
            }
            
        }
}
