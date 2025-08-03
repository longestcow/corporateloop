using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StateManager : MonoBehaviour
{
    public Animator canvasAnim;

    [HideInInspector] public bool started = false, paused = false, options = false, waitingKeybind = false, enemyStun = false;
    int currKeybindID;
    public GameObject pause, keybindsParent;
    public Player player;
    public KeyCode[] keybinds = { KeyCode.LeftShift, KeyCode.Space, KeyCode.Mouse1, KeyCode.Mouse0, KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.Escape };
    public float playerPunchDmg = 2;
    public Material hurtMat;
    public GameObject itemsParent;
    public Toggle timerToggle, keybindToggle;
    public Boss boss;
    GameObject interactText;
    /*
     * 0 - ability 1
     * 1 - ability 2
     * 2 - ability 3
     * 3 - punch
     * 4 - left
     * 5 - right
     * 6 - interact  
     * 7 - pause
     */
    public static bool coolstun;
    public bool duringbossfight;

    void Start()
    {
        duringbossfight = false;
        coolstun = false;
        player = GameObject.Find("player").GetComponent<Player>();
        StartCoroutine(enablePause());
        canvasAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        Time.timeScale = 0f;
        keybinds = new KeyCode[] { KeyCode.LeftShift, KeyCode.Space, KeyCode.Mouse1, KeyCode.Mouse0, KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.Escape }; // default
        for (int i = 0; i <= 7; i++)
        {
            if (PlayerPrefs.GetInt("keybind" + i) != 0)
            {
                keybinds[i] = (KeyCode)PlayerPrefs.GetInt("keybind" + i);
            }
            keybindsParent.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = getKeyName(keybinds[i]);
        }
        interactText = player.transform.GetChild(0).GetChild(0).gameObject;
        SetDescriptions();
        SFXManager.instance.changeMusic(1, player.transform);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keybinds[7]))
        {
            Pause();
        }
        if (waitingKeybind)
            {
                foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(vKey))
                    {
                        keybinds[currKeybindID] = vKey;
                        String keybindName = getKeyName(vKey);

                        keybindsParent.transform.GetChild(currKeybindID).GetChild(0).GetComponent<TextMeshProUGUI>().text = keybindName;
                        waitingKeybind = false;

                        PlayerPrefs.SetInt("keybind" + currKeybindID, (int)vKey);
                        print((KeyCode)PlayerPrefs.GetInt("keybind" + currKeybindID));
                        PlayerPrefs.Save();
                        currKeybindID = 999;

                        SetDescriptions();

                        break;
                    }

                }

            }
    }

    void LateUpdate()
    {
        interactText.transform.rotation = Quaternion.identity;
    }

    public void Play()
    {
        canvasAnim.SetTrigger("playButton");
        if (started)
        {
            if (!duringbossfight)
                SFXManager.instance.changeMusic(0, player.transform);
            paused = false;
            Time.timeScale = 1.0f;
            options = false;
            canvasAnim.SetTrigger("optionsClose");
        }
        else
        {
            //game loop start
            if (!duringbossfight)
                SFXManager.instance.changeMusic(0, player.transform);
            started = true;
            Time.timeScale = 1.0f;
            options = false;
            canvasAnim.SetTrigger("optionsClose");

        }
    }
    public void Options()
    {
        if (options)
        {
            options = false;
            canvasAnim.SetTrigger("optionsClose");
            return;
        }
        options = true;
        canvasAnim.SetTrigger("optionsOpen");
    }
    public void OptionsBack()
    {
        options = false;
        canvasAnim.SetTrigger("optionsClose");
    }
    public void Pause()
    {
        if (!duringbossfight)
            SFXManager.instance.changeMusic(1, player.transform);
        canvasAnim.SetTrigger("pause");
        paused = true;
        Time.timeScale = 0f;
    }

    public void SetKeybind(int id)
    {
        keybindsParent.transform.GetChild(id).GetChild(0).GetComponent<TextMeshProUGUI>().text = "...";
        currKeybindID = id;
        waitingKeybind = true;

    }

    string getKeyName(KeyCode key)
    {
        String keybindName = key.ToString();
        keybindName = keybindName.Replace("Control", "Ctrl");
        keybindName = keybindName.Replace("Return", "Entr");
        keybindName = keybindName.Replace("Escape", "Esc");
        keybindName = keybindName.Replace("Page", "Pg");
        keybindName = keybindName.Replace("Left", "L");
        keybindName = keybindName.Replace("Right", "R");
        keybindName = keybindName.Replace("Up", "U");
        keybindName = keybindName.Replace("Down", "D");
        keybindName = keybindName.Replace("Mouse0", "LC");
        keybindName = keybindName.Replace("Mouse1", "RC");
        keybindName = keybindName.Replace("Mouse2", "MC");
        if (keybindName.Length > 4)
            keybindName = keybindName.Substring(0, 4);
        return keybindName;
    }

    IEnumerator enablePause()
    {
        yield return new WaitForSeconds(0.5f);
        pause.SetActive(true);
    }
    public IEnumerator HitStun(enemyHurtbox enem)
    {
        //do funny thing with music like hollow knight
        enem.sprite.material = hurtMat;
        Time.timeScale = 0.001f;
        enemyStun = true;
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 1;
        enem.sprite.material = enem.ogMat;

        yield return new WaitForSecondsRealtime(0.2f);
        enemyStun = false;

    }

    public void clearPuddles()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("water"))
            Destroy(obj);
    }


    IEnumerator CoolHitStun()
    {
        //do funny thing with music like hollow knight
        Time.timeScale = 0f;
        enemyStun = true;
        coolstun = true;
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;
        coolstun = false;
        yield return new WaitForSecondsRealtime(0.2f);
        enemyStun = false;
    }

    public void abilityDescEnter(int id)
    {
        itemsParent.transform.GetChild(id).GetChild(0).gameObject.SetActive(true);
    }
    public void abilityDescExit(int id)
    {
        itemsParent.transform.GetChild(id).GetChild(0).gameObject.SetActive(false);
    }

    public void SetDescriptions()
    {
        for (int i = 0; i < 3; i++)
        {
            itemsParent.transform.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text = "[" + getKeyName(keybinds[i]) + "]";
        }

    }

    String getAbilityDesc(int id)
    {
        switch (id)
        {
            case 0:
                return "nothing";
            case 1:
                return "coffee";
            case 2:
                return "drug";
            case 3:
                return "pen";
            case 4:
                return "strength";
            case 5:
                return "jump";
            case 6:
                return "stick";
            case 7:
                return "scissor";
            case 8:
                return "slam";
            default:
                return "nothing";
        }
    }

    public void Toggle(int id)
    {
        if (id == 0)
        {
            //timerToggle.isOn
        }
        else
        {
            SetDescriptions();
            if (keybindToggle.isOn)
            {
                itemsParent.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 323, 0);
                for (int i = 0; i < 3; i++)
                {
                    itemsParent.transform.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().enabled = true;
                }
            }
            else
            {
                itemsParent.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 315, 0);
                for (int i = 0; i < 3; i++)
                {
                    itemsParent.transform.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }
    }

    public void bossStart()
    {
        boss.Cutscene();
        duringbossfight = true;
        SFXManager.instance.changeMusic(2, transform);
    }
    public void elevatorStart()
    {
        SFXManager.instance.changeMusic(3, transform);
    }
    public void elevatorStop()
    {
        SFXManager.instance.changeMusic(0, transform);
    }

}
