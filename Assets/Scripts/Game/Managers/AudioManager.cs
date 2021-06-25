using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioSource audioSource;

    private AudioSource _backgroundAudio;
    [SerializeField]
    private AudioClip _penultimateClip;
    [SerializeField]
    private AudioClip _lastClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        _backgroundAudio = GetComponentsInChildren<AudioSource>()[1];

        if (audioSource is null)
            Debug.LogError("Audio Source is NULL");
        if (_backgroundAudio is null)
            Debug.LogError("Audio Source in background is NULL");

        GameManager.OnNewWave += ChangeBackgroundAudio;
    }

    private void ChangeBackgroundAudio()
    {
        if (GameManager.currentWave < Mathf.CeilToInt(GameManager.maxWaves / 2f))
            return;

        _backgroundAudio.Stop();

        if(GameManager.currentWave == Mathf.CeilToInt(GameManager.maxWaves / 2f))
        {
            _backgroundAudio.clip = _penultimateClip;
            _backgroundAudio.volume = 0.15f;
        }

        if(GameManager.currentWave == GameManager.maxWaves)
        {
            _backgroundAudio.clip = _lastClip;
            _backgroundAudio.volume = 0.6f;
        }
            
        _backgroundAudio.Play();
    }

    private void OnDestroy()
    {
        GameManager.OnNewWave -= ChangeBackgroundAudio;
    }
}
