using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Thrusters : MonoBehaviour
{
    public static Action<float, float> OnThrusterUsage;

    [Header("Thruster Properties")]
    [SerializeField]
    private float _thrusterEnergy;
    private float _maxThrusterEnergy;
    [SerializeField]
    private float _maxBoost;
    private float _currentBoost;
    [SerializeField]
    private float _energyPerSecond;
    private bool _isBoosting;
    [SerializeField]
    private float _boostDelay;
    private WaitForSeconds _delay;

    public bool CanBoost { get; private set; }
    public bool AreHot { get ; private set; }

    [SerializeField]
    private AudioClip[] _audioClips;
    private AudioSource _movementAudioSource;

    void Start()
    {
        CanBoost = true;
        _movementAudioSource = GameObject.Find("Player_Movement_Sounds").GetComponent<AudioSource>();

        if (_movementAudioSource is null)
            Debug.LogError("Movement Audio Source is NULL");

        _maxThrusterEnergy = _thrusterEnergy;
        _delay = new WaitForSeconds(_boostDelay);
    }

    public void RefillPower()
    {
        _thrusterEnergy += _maxBoost * 2f;

        if (_thrusterEnergy > _maxThrusterEnergy)
            _thrusterEnergy = _maxThrusterEnergy;

        //to avoid enabling the boost if the thrusters need a break
        if (_currentBoost <= 0f)
            CanBoost = true;

        UpdateUI();
    }    
    
    public void UseThrusters()
    {
        //stop movement sound if it's playing
        if (!_isBoosting)
        {
            _movementAudioSource.Stop();
            _isBoosting = true;
        }

        //play the boost sound if it's not playing
        if (!_movementAudioSource.isPlaying)
            PlayMovementAudio(0);

        //waste energy
        float powerWasted = _energyPerSecond * Time.deltaTime;
        _thrusterEnergy -= powerWasted;

        //sum wasted energy to reach the threshold
        _currentBoost += powerWasted;

        if (!AreHot)
            AreHot = true;

        //energy available?
        if (_thrusterEnergy <= 0f)
        {
            CanBoost = false;
            _thrusterEnergy = 0f;
        }

        //threshold reached
        if (_currentBoost >= _maxBoost)
            StartCoroutine(ThrusterNeedsBreak());

        UpdateUI();
    }

    public void FreezeThrusters()
    {
        _currentBoost -= (_energyPerSecond * Time.deltaTime * 2f);

        UpdateUI();

        if (_currentBoost <= 0f)
        {
            _currentBoost = 0f;
            AreHot = false;
        }

        //stop boost sound if it's playing
        if (_isBoosting)
        {
            _movementAudioSource.Stop();
            _isBoosting = false;
        }

        //play the movement sound if it's not playing
        if (!_movementAudioSource.isPlaying)
            PlayMovementAudio(1);
    }

    IEnumerator ThrusterNeedsBreak()
    {
        //Deactivate thrusters
        CanBoost = false;

        //wait for the delay to activate thrusters
        yield return _delay;

        //energy remaining? activate thrusters
        if (_thrusterEnergy > 0f)
            CanBoost = true;

        _currentBoost = 0f;
        AreHot = false;
    }

    private void UpdateUI()
    {
        if (!(OnThrusterUsage is null))
        {
            float energy = _thrusterEnergy / _maxThrusterEnergy;
            float usage = _currentBoost / _maxBoost;
            OnThrusterUsage(energy, usage);
        }
    }

    void PlayMovementAudio(int index)
    {
        if (index < _audioClips.Length && index >= 0)
        {
            if (_movementAudioSource.isPlaying)
                _movementAudioSource.Stop();

            _movementAudioSource.clip = _audioClips[index];
            _movementAudioSource.Play();
        }
    }

    private void OnDestroy()
    {
        OnThrusterUsage = null;
        Destroy(_movementAudioSource);
    }
}
