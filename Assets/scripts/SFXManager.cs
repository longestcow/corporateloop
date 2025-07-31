using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    public AudioSource sfxObject;
    public AudioClip[] abilitiesSFX;

    public void Awake(){
        DontDestroyOnLoad(this.gameObject);
        if(instance==null)
            instance=this;
    }

    public void playSFX(AudioClip clip, Transform spawnpoint, float volume){
        AudioSource source = Instantiate(sfxObject, spawnpoint.position, Quaternion.identity);
        source.clip = clip;
        source.volume=volume/1.7f;
        source.Play();
        Destroy(source.gameObject, source.clip.length);
    }

    public void playRandomSFX(AudioClip[] clips, Transform spawnpoint, float volume){
        int rand = Random.Range(0, clips.Length);
        AudioSource source = Instantiate(sfxObject, spawnpoint.position, Quaternion.identity);
        source.clip = clips[rand];
        source.volume=volume/1.7f;
        source.Play();
        Destroy(source.gameObject, source.clip.length);
    }

}
