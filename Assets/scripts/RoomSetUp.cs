using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSetUp : MonoBehaviour
{
    public bool starting;

    public Sprite[] roombgs;
    public SpriteRenderer roombgrenderer;
    public GameObject elevatorshadow;
    public GameObject[] exits; //0 - left, 1- right
    public GameObject[] enemyprefabs;
    public GameObject[] throwableprefabs;
    bool iselevator;
    public int index;
    int enemycount;
    int[] enemies = new int[3];
    // 0 - brute
    // 1 - staple
    // 2 - janitor
    bool hasthrowable;
    bool elevatorscene;
    int lastcount;

    public Sprite chosenbg;
    public Sprite whitebg;

    // Start is called before the first frame update
    void Start()
    {
        elevatorscene = false;
        if (starting) return;
        index = transform.GetSiblingIndex();
        if (iselevator) return;
        iselevator = false;
        if (index % 2 == 0)
        {
            roombgrenderer.sprite = roombgs[0];
            chosenbg = roombgs[0];
            exits[1].SetActive(false);
        }
        else
        {
            chosenbg = roombgs[1];
            exits[0].SetActive(false);
        }
        CalculateEnemies();
        lastcount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (elevatorscene && iselevator) return;

        if (StateManager.coolstun) roombgrenderer.sprite = whitebg;
        else roombgrenderer.sprite = chosenbg;

        if (lastcount != Player.runcount)
        {
            lastcount = Player.runcount;
            SpawnEnemies();
        }

        if (iselevator && Player.startedelevator) StartCoroutine("ElevatorOpenClose");
    }

    public void CalculateEnemies()
    {
        if (index < 2) enemycount = 1;
        else if (index < 5) enemycount = 2;
        else if (index < 10) enemycount = 3;
        else enemycount = 4;

        for (int i = 0; i < enemycount; i++)
        {
            enemies[Random.Range(0, 3)]++;
        }
        if (Random.Range(0, 4) != 0) hasthrowable = true;
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < enemies[i]; j++)
            {
                Instantiate(enemyprefabs[i], new Vector3(Random.Range(-5f, 5f),(7 * (index + 1))- 2,0), Quaternion.Euler(0,0,0), this.transform);
            }
        }
        if (hasthrowable)
        {
            Instantiate(throwableprefabs[0], new Vector3(Random.Range(-5f, 5f), (7 * (index + 1)) - 2, 0), Quaternion.Euler(0, 0, 0), this.transform);
        }
    }

    public void MakeElevator()
    {
        roombgrenderer.sprite = roombgs[2];
        chosenbg = roombgs[2];
        elevatorshadow.SetActive(true);
        exits[0].SetActive(false);
        iselevator = true;
        elevatorscene = false;
    }

    IEnumerator ElevatorOpenClose()
    {
        Player.startedelevator = false;
        elevatorscene = true;
        roombgrenderer.sprite = roombgs[3];
        yield return new WaitForSeconds(0.5f);
        roombgrenderer.sprite = roombgs[4];
        yield return new WaitForSeconds(0.5f);
        roombgrenderer.sprite = roombgs[5];
        yield return new WaitForSeconds(1f);
        roombgrenderer.sprite = roombgs[2];
        elevatorscene = false;
    }
}
