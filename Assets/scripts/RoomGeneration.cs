using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGeneration : MonoBehaviour
{
    public GameObject roomprefab;
    public static int elevatorroomindex;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i < 15; i++)
        {
            Instantiate(roomprefab, new Vector3(0, 7 * i, 0), Quaternion.Euler(0, 0, 0), this.transform);
        }
        elevatorroomindex = Random.Range(3, transform.childCount / 2) * 2 - 1;
        transform.GetChild(elevatorroomindex).gameObject.GetComponent<RoomSetUp>().MakeElevator();
        print(elevatorroomindex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
