using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
    public Animator canvasAnim;

    [HideInInspector] public bool started = false, paused = false, options = false, waitingKeybind = false;
    int currKeybindID;
    public GameObject pause, keybindsParent;
    [HideInInspector] public KeyCode[] keybinds = {KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Q, KeyCode.E, KeyCode.Space, KeyCode.A, KeyCode.D, KeyCode.W};

    void Start()
    {
        StartCoroutine(enablePause());
        canvasAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        Time.timeScale = 0f; //                                        RPC LOOK AT THIS IF YOURE CONFUSED ABOUT ANYTHING THE GAME IS PAUSED AT THE VERY START ALWAYS
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
                    keybindsParent.transform.GetChild(currKeybindID).GetChild(0).GetComponent<TextMeshProUGUI>().text = vKey.ToString();
                    waitingKeybind = false;
                    currKeybindID = 999;
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
        }
        else
        {
            //game loop start
            started = true;
            Time.timeScale = 1.0f;

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

    IEnumerator enablePause() {
        yield return new WaitForSeconds(0.5f);
        pause.SetActive(true);
    }

}
