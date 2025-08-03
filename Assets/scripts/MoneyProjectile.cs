using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    bool move = false;
    void Start()
    {
        player = GameObject.Find("player");
    }

    // Update is called once per frame
    void Update()
    {
        if(move)
            transform.position += transform.forward * 1 * Time.deltaTime;
    }

    public IEnumerator launch(int t)
    {
        yield return new WaitForSeconds(1);
        Vector3 direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        yield return new WaitForSeconds(t - 1);
        move = true;
    }

    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.gameObject.layer == 9)// walls
    //         Destroy(this);
    // }
}
