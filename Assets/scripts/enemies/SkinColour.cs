using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinColour : MonoBehaviour
{

    Shader shader;
    public Color skincol;
    public Color skincol2;

    // Start is called before the first frame update
    void Start()
    {
        float x = Random.Range(0.3f, 1f);
        skincol = new Color(x, x * 0.7810523f, x * 0.5424528f);
        x = x + 0.08f;
        x = Mathf.Clamp(x, 0, 1);
        skincol2 = new Color(x, x * 0.7810523f, x * 0.5424528f);
        if (Random.Range(0, 100) >= 99)
        {
            skincol = new Color(0.4723749f, 0.0867746f, 0.735849f);
            skincol2 = new Color(0.3335119f, 0.120283f, 0.4811321f);
        }
        this.gameObject.GetComponent<SpriteRenderer>().material.SetColor("_TargetColor", skincol);
        this.gameObject.GetComponent<SpriteRenderer>().material.SetColor("_BaldColor", skincol2);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.R))
        {
            float x = Random.Range(0.3f, 1f);
            skincol = new Color(x, x * 0.7810523f, x * 0.5424528f);
            x = x + 0.08f;
            x = Mathf.Clamp(x, 0, 1);
            skincol2 = new Color(x, x * 0.7810523f, x * 0.5424528f);
            if (Random.Range(0, 100) >= 99)
            {
                skincol = new Color(0.4723749f, 0.0867746f, 0.735849f);
                skincol2 = new Color(0.3335119f, 0.120283f, 0.4811321f);
            }
            this.gameObject.GetComponent<SpriteRenderer>().material.SetColor("_TargetColor", skincol);
            this.gameObject.GetComponent<SpriteRenderer>().material.SetColor("_BaldColor", skincol2);
        }*/
    }
}
