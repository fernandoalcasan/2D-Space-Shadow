using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy
{
    [Header("Static Properties")]
    [SerializeField]
    private float _fireDelay;
    [SerializeField]
    private int _scoreValue;
    [SerializeField]
    private float _deathTime;

    public float FireDelay { get => _fireDelay; }
    public int ScoreValue { get => _scoreValue; }
    public float DeathTime { get => _deathTime; }

    [Header("Space Bounds")]
    [SerializeField]
    private float _xLimit;
    [SerializeField]
    private float _yLimit;

    public float XBound { get => _xLimit;}
    public float YBound { get => _yLimit; }

    [Header("Audios")]
    [SerializeField]
    private AudioClip[] _audioClips;

    public AudioClip[] AudioClips { get => _audioClips; }
}
