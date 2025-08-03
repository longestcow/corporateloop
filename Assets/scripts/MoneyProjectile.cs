using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    bool move;
    Vector3 direction;
    void Start()
    {
        player = GameObject.Find("player");
        move = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(move)
            transform.position += (direction).normalized * 0.2f;
    }

    public IEnumerator launch(int t)
    {
        yield return new WaitForSeconds(1);
        transform.parent = null;
        direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        yield return new WaitForSeconds((t*0.1f) - 1);
        move = true;
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }

    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.gameObject.layer == 9)// walls
    //         Destroy(this);
    // }
}
