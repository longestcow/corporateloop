using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    public AudioSource sfxObject;
    public AudioClip[] music; // 0 main, 1 pause, 2 boss
    public AudioClip[] abilitiesSFX;
    public Slider sfxVol;
    public Slider musicVol;
    public AudioSource currentMusic = null;
    public int currentMusicIndex = 1, prevMusicIndex = 0;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
            instance = this;
    }

    public void playSFX(AudioClip clip, Transform spawnpoint, float volume)
    {
        AudioSource source = Instantiate(sfxObject, spawnpoint.position, Quaternion.identity);
        source.clip = clip;
        source.volume = sfxVol.value / 1.7f;
        source.Play();
        Destroy(source.gameObject, source.clip.length);
    }

    public void playRandomSFX(AudioClip[] clips, Transform spawnpoint)
    {
        int rand = Random.Range(0, clips.Length);
        AudioSource source = Instantiate(sfxObject, spawnpoint.position, Quaternion.identity);
        source.clip = clips[rand];
        source.volume = sfxVol.value / 1.7f;
        source.Play();
        Destroy(source.gameObject, source.clip.length);
    }

    public void changeMusic(int id, Transform spawnpoint)
    {
        float time = 0;
        if (currentMusic != null)
        {
            if ((currentMusicIndex == 0 && id == 1) || (currentMusicIndex == 1 && id == 0))
                time = currentMusic.time;

            Destroy(currentMusic);
            currentMusic = null;
        }
        prevMusicIndex = currentMusicIndex;
        currentMusic = Instantiate(sfxObject, spawnpoint.position, Quaternion.identity);
        currentMusic.clip = music[id];
        currentMusic.volume = musicVol.value / (id == 1 ? 1f : 1);
        currentMusic.loop = true;
        currentMusic.Play();
        currentMusic.time = time;
        currentMusicIndex = id;

    }

    public void musicVolChange()
    {
        if (currentMusic != null)
        {
            currentMusic.volume = musicVol.value / (currentMusicIndex == 1 ? 1f : 1);
        }
    }

    public void fadeOut()
    {
        StartCoroutine(FadeOutCor());
    }

    IEnumerator FadeOutCor()
    {
        float startVolume = currentMusic.volume;
        float targetVolume = 0f;

        float timer = 0f;
        while (timer < 0.8f)
        {
            timer += Time.deltaTime;
            currentMusic.volume = Mathf.Lerp(startVolume, targetVolume, timer / 0.8f);
            yield return null; // Wait for the next frame
        }
        currentMusic.volume = targetVolume; // Ensure volume is exactly 0 at the end
        currentMusic.Stop(); // Stop the audio after fading out completely
    }

    public void fadeIn()
    {
        StartCoroutine(FadeInCor());
    }

    IEnumerator FadeInCor()
    {
        float startVolume = 0f;
        float targetVolume = musicVol.value;
        currentMusic.volume = startVolume;
        currentMusic.Play();

        float timer = 0f;
        while (timer < 0.8f)
        {
            timer += Time.deltaTime;
            currentMusic.volume = Mathf.Lerp(startVolume, targetVolume, timer / 0.8f);
            yield return null; // Wait for the next frame
        }
        currentMusic.volume = targetVolume;
    }
}
