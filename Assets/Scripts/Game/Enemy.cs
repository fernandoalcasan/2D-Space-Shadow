using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy
{
    [Header("Properties")]
    public float speed;
    public float fireDelay;
    public int scoreValue;

    [Header("Space Bounds")]
    public float xLimit;
    public float yLimit;

    [Header("References")]
    public GameObject shot;
    [HideInInspector]
    public Animator anim;

    [Header("Audios")]
    public AudioClip[] audioClips;
    [HideInInspector]
    public AudioSource properAudioSource;
}
