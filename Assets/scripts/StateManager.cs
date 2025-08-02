using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
    public Animator canvasAnim;

    [HideInInspector] public bool started = false, paused = false, options = false, waitingKeybind = false;
    int currKeybindID;
    public GameObject pause, keybindsParent;
    public Player player;
    public KeyCode[] keybinds = { KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.E, KeyCode.Space, KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.Escape };
    public float playerPunchDmg = 2;
    /*
     * 0 - ability 1
     * 1 - ability 2
     * 2 - ability 3
     * 3 - punch
     * 4 - left
     * 5 - right
     * 6 - interact  
     * 7 - pause (not implemented yet)
     */

    void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
        StartCoroutine(enablePause());
        canvasAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        Time.timeScale = 0f; //                                        RPC LOOK AT THIS IF YOURE CONFUSED ABOUT ANYTHING THE GAME IS PAUSED AT THE VERY START ALWAYS
        for (int i = 0; i <= 7; i++)
        {
            print(PlayerPrefs.GetInt("keybind" + i));
            if (PlayerPrefs.GetInt("keybind" + i) != 0)
            {
                keybinds[i] = (KeyCode)PlayerPrefs.GetInt("keybind" + i);
                print(keybinds[i]);
            }
            keybindsParent.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = getKeyName(keybinds[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
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
                    currKeybindID = 999;
                    
                    PlayerPrefs.SetInt("keybind" + currKeybindID, (int)vKey);
                    print((KeyCode)PlayerPrefs.GetInt("keybind" + currKeybindID));
                    PlayerPrefs.Save();
                    break;
                }

            }

        }
    }

    public void Play()
    {
        canvasAnim.SetTrigger("playButton");
        if (started)
        {
            paused = false;
            Time.timeScale = 1.0f;
            options = false;
            canvasAnim.SetTrigger("optionsClose");
        }
        else
        {
            //game loop start
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
    IEnumerator HitStun()
    {
        //do funny thing with music like hollow knight
        Time.timeScale = 0f;
        paused = true;
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(0.2f);
        paused = false;


    }

}
