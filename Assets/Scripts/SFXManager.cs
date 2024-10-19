using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    public AudioClip playerAttack;
    public AudioClip playerDie;
    public AudioClip playerWalk;
    public AudioClip Fireball;
    public AudioClip Power;
    public AudioClip Heal;
    public AudioClip playerJump;
    public AudioClip Teleport;
    AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.5f;
    }

    public void PlaySound(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
