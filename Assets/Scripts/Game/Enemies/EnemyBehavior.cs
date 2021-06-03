using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class EnemyBehavior : MonoBehaviour
{
    [SerializeField]
    private Enemy _enemy;

    protected float Speed
    {
        get { return _enemy.speed; }
        private set { _enemy.speed = value; }
    }
    protected float XBound { get; private set; }
    protected float YBound { get; private set; }

    private float _canShoot = -1f;
    private bool _dead;

    private Player _player;

    public virtual void Start()
    {
        XBound = _enemy.xLimit;
        YBound = _enemy.yLimit;

        _canShoot = _enemy.fireDelay;
        _player = GameObject.Find("Player").GetComponent<Player>();
        _enemy.anim = GetComponent<Animator>();
        _enemy.properAudioSource = GetComponent<AudioSource>();

        if (_player is null)
        {
            Debug.LogError("The Player is NULL");
        }

        if (_enemy.anim is null)
        {
            Debug.LogError("The animator is NULL");
        }

        if (_enemy.properAudioSource is null)
        {
            Debug.LogError("AudioSource is NULL");
        }
    }

    public virtual void Update()
    {
        if (Time.time > _canShoot && !_dead)
        {
            Shoot();
        }
    }

    ////////////////////////////////
    //SHOOTING//////////////////////
    ////////////////////////////////

    void Shoot()
    {
        //Update delay time for laser
        _canShoot = Time.time + _enemy.fireDelay;

        Instantiate(_enemy.shot, transform.position, transform.rotation);

        //Shot AUDIO
        PlayAudio(1);
    } 

    ////////////////////////////////
    //COLLISIONS////////////////////
    ////////////////////////////////

    void OnTriggerEnter2D(Collider2D other)
    {
        //Collition with player
        if (other.CompareTag("Player"))
        {
            //EXPLOSION AUDIO
            PlayAudio(0);
            int dir = _player.GetDamageDirection(transform.position);
            _player.GetDamage(dir);
            _player.IncreaseScore(_enemy.scoreValue);
            DeathSequence();
        }
        //Collition with Laser
        else if (other.CompareTag("SimpleLaser"))
        {
            //EXPLOSION AUDIO
            PlayAudio(0);
            Destroy(other.gameObject);
            _player.IncreaseScore(_enemy.scoreValue);
            DeathSequence();
        }
    }

    ////////////////////////////////
    //ONDESTROY/////////////////////
    ////////////////////////////////

    void DeathSequence()
    {
        //to avoid making damage again while doing animation
        Destroy(GetComponent<Collider2D>());
        _dead = true;
        _enemy.anim.SetTrigger("OnEnemyDeath");
        Speed = 0f;
        Destroy(gameObject, 1.19f);
    }

    ////////////////////////////////
    //AUDIO/////////////////////////
    ////////////////////////////////

    void PlayAudio(int index)
    {
        if (index < _enemy.audioClips.Length && index >= 0)
        {
            _enemy.properAudioSource.clip = _enemy.audioClips[index];
            _enemy.properAudioSource.Play();
        }
    }
}
