using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [SerializeField, Range(0,1)] float runVolume=0.5f;
    [SerializeField, Range(0,1)] float walkVolume=0.2f;
    [SerializeField] private AudioClip[] clips;
    private int lastClipIndex=0;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Step(){
        AudioClip clip = GetRandomClip();
        audioSource.volume = runVolume;
        audioSource.PlayOneShot(clip);
    }

    private void WalkStep(){
        AudioClip clip = GetRandomClip();
        audioSource.volume = walkVolume;
        audioSource.PlayOneShot(clip);
    }


    private AudioClip GetRandomClip(){
        int index = Random.Range(0, clips.Length);
        while(index==lastClipIndex)
            index = Random.Range(0, clips.Length);
        lastClipIndex = index;

        return clips[index];
    }
}
