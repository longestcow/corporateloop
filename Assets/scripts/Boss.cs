using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public bool started = false;
    public float health = 70;
    Animator anim;
    int state = 0;
    bool idled = false, switchSided = false, left = true, bombing = false;
    float waitTime = 2f;
    public Transform moneyParent, bombSpot;
    public GameObject moneyPrefab, bombPrefab;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (!started) return;
        if (state == 2)
        {
            if (idled)
            {
                if (switchSided)
                    StartCoroutine(switchSide());
                StartCoroutine(Idle());
            }
            else
            {
                state = 3;
            }
        }
        if (state == 3)
        {
            // int r = Random.Range(0, 3);
            int r = 2;
            if (r == 0) // paper stack
            {
                //instantiate paper stack projectile
                waitTime = 2f;

            }
            else if (r == 1)
            {
                //instantiate 1 dude in both sides
            }
            else if (r == 2 && !bombing)
            {
                // instantiate 1 bomb 5m away from boss
                StartCoroutine(Bomb());
                waitTime = 3f;
            }
            // else
            // {
            //     StartCoroutine("Money");
            //     waitTime = 8;
            // }


        }

    }

    public void Cutscene()
    {
        // do your magic

        started = true;
        state = 2;
    }

    IEnumerator Idle()
    {
        anim.SetTrigger("smoke");
        yield return new WaitForSeconds(waitTime);
        idled = false;

    }

    IEnumerator switchSide()
    {
        switchSided = false;
        yield return new WaitForSeconds(15f);
        switchSided = true;
        anim.SetTrigger(left ? "LtoR" : "RtoL");
        left = !left;
    }
    IEnumerator Money()
    {
        anim.SetTrigger("money");
        waitTime = 8;
        yield return new WaitForSeconds(0.65f);
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
        idled = true;
        bombing = false;
        Destroy(obj);

    }
}
