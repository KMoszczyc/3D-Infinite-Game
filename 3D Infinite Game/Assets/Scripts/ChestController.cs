using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChestController : MonoBehaviour
{
    public RuntimeAnimatorController open;
    public RuntimeAnimatorController close;

    public AudioClip openSound;
    public AudioClip closeSound;

    private Animator animator;
    private AudioSource audioSource;
    public TextMeshPro openCloseTextMesh;
    public TextMeshPro coinsTextMesh;    

    private ParticleSystem particleSystem;

    [HideInInspector] public bool isClosed = true;
    public bool showText = false;
    private int coins;

    [HideInInspector] public bool isLooted = false;

    void Start() { 
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        particleSystem = GetComponent<ParticleSystem>();
        coins = Random.Range(15, 100);
        transform.Rotate(0f,Random.Range(-180,180),0f);
    }

    void LateUpdate () {
        openCloseTextMesh.gameObject.SetActive(showText);
        openCloseTextMesh.transform.LookAt(Camera.main.transform.position);
        openCloseTextMesh.transform.Rotate(0, 180, 0);

        if(!isLooted) {
            coinsTextMesh.gameObject.SetActive(!isClosed);
            coinsTextMesh.transform.LookAt(Camera.main.transform.position);
            coinsTextMesh.transform.Rotate(0, 180, 0);
        }

        showText = false;
    }

    public void UpdateTextMesh(TextMeshPro textMesh) {
        textMesh.gameObject.SetActive(showText);
        textMesh.transform.LookAt(Camera.main.transform.position);
        textMesh.transform.Rotate(0, 180, 0);
    }

    public void Open() { 
        if(isClosed){
            animator.runtimeAnimatorController = open;
            audioSource.PlayOneShot(openSound);
            isClosed = false;
            particleSystem.Stop(true);
            StartCoroutine(ChangeTextAfterTime(0.9f, "close"));
        }
    }

    public void Close() { 
        if(!isClosed){
            animator.runtimeAnimatorController = close;
            audioSource.PlayOneShot(closeSound);
            isClosed = true;
            StartCoroutine(ChangeTextAfterTime(0.4f, "open"));
        }
    }

    public int Loot(){
        isLooted=true;
        coinsTextMesh.gameObject.SetActive(false);
        
        return coins;
    }

    IEnumerator ChangeTextAfterTime(float seconds, string text) 
    {
        yield return new WaitForSeconds(seconds);
        openCloseTextMesh.text = text;
    }
}
