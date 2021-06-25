using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FinalBoss : Enemy
{
    public static Action<float> OnBossDamage;

    [Header("Boss Properties")]
    [SerializeField]
    private float _delayOnBehavior;
    [SerializeField]
    private float _delayOnShot;
    [SerializeField] [Range(0f, 1f)]
    private float _difficultyFactor;
    [SerializeField]
    private float _degPerSecond;
    
    [Header("Portals")]
    [SerializeField]
    private GameObject _portal;
    [SerializeField]
    private GameObject _endPortal;
    
    private WaitForSeconds _delayBetBehav;
    private WaitForSeconds _delayBetShots;
    private float _maxLives;
    private Vector3 _portalPos;
    private bool _shooting;

    public override void Start()
    {
        base.Start();
        _maxLives = _lives;
        transform.position = new Vector3(0, 9f, 0);
        StartCoroutine(EnemyEntrance(new Vector3(0, 2.9f, 0)));
        _delayBetBehav = new WaitForSeconds(_delayOnBehavior);
        _delayBetShots = new WaitForSeconds(_delayOnShot);
    }

    public override void Update()
    {
        if(!_shooting && !IsDead)
            transform.Rotate(_degPerSecond * Time.deltaTime * Vector3.forward, Space.World);
    }

    protected override void MoveEnemy()
    {
        transform.position = _portalPos;
    }

    private void ShootOnBothSides()
    {
        Quaternion inverseRot = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - 180f);
        Instantiate(shot, transform.position, inverseRot);
        Instantiate(shot, transform.position, transform.rotation);

        //Shot AUDIO
        AudioManager.audioSource.PlayOneShot(_audioClips[1], .5f);
    }

    private void DisplayPortals()
    {
        GameObject ownPortal = Instantiate(_portal, transform.position, Quaternion.identity);
        _portalPos = new Vector3(GetRandomX(), GetRandomY(), 0);
        GameObject endPortal = Instantiate(_endPortal, _portalPos, Quaternion.identity);
        Destroy(ownPortal, .5f);
        Destroy(endPortal, 1f);
    }

    private IEnumerator EnemyEntrance(Vector3 dir)
    {
        while (transform.position != dir)
        {
            transform.position = Vector3.MoveTowards(transform.position, dir, Speed * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(ChooseNewBehavior());
    }

    private IEnumerator ChooseNewBehavior()
    {
        while(!IsDead)
        {
            yield return _delayBetBehav;
            int behavior = UnityEngine.Random.Range(0, 2);
            switch (behavior)
            {
                case 0:
                    _shooting = true;
                    yield return _delayBetShots;
                    anim.SetTrigger("Shoot");
                    yield return _delayBetShots;
                    _shooting = false;
                    break;
                case 1:
                    anim.SetTrigger("Teleport");
                    break;
            }
        }
    }

    private void AddDifficulty()
    {
        _degPerSecond *= 1f + (_difficultyFactor * 2f);
        _delayOnBehavior *= 1f - _difficultyFactor;
        _delayOnShot *= 1f - _difficultyFactor;

        _delayBetBehav = new WaitForSeconds(_delayOnBehavior);
        _delayBetShots = new WaitForSeconds(_delayOnShot);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!(OnBossDamage is null))
            {
                float currentLife = (_lives - 1) / _maxLives;
                OnBossDamage(currentLife);
            }
            if ((_lives - 1) % 10 == 0)
                AddDifficulty();
        }
        else if (other.CompareTag("PlayerShot"))
        {
            AudioManager.audioSource.PlayOneShot(_audioClips[2], 0.2f);
            if (!(OnBossDamage is null))
            {
                float currentLife = (_lives - 1) / _maxLives;
                OnBossDamage(currentLife);
            }
            if ((_lives - 1) % 10 == 0)
                AddDifficulty();
        }

        base.OnTriggerEnter2D(other);
    }

    private void OnDestroy()
    {
        OnBossDamage = null;
    }
}
