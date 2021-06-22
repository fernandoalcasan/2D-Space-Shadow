using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class Enemy : MonoBehaviour
{
    [Header("Enemy Properties")]
    [SerializeField]
    protected int _lives;
    [SerializeField]
    private float _speed;
    protected float Speed { get => _speed; }

    [SerializeField]
    private float _fireDelay;
    protected float FireDelay { get => _fireDelay; }

    [SerializeField]
    private int _scoreValue;
    [SerializeField]
    private float _deathTime;

    [Header("Space Bounds")]
    [SerializeField]
    private float _xLimit;
    [SerializeField]
    private float _yLimit;

    [Header("Audios")]
    [SerializeField]
    private AudioClip[] _audioClips;
    private AudioSource _properAudioSource;

    [Header("References")]
    [SerializeField]
    protected GameObject shot;
    protected Animator anim;
    protected Player _player;
    private GameManager _gameManager;
    
    protected float canShoot = -1f;

    private bool _dead;
    protected bool IsDead { get => _dead; }

    public virtual void Start()
    {
        canShoot = _fireDelay;
        _player = GameObject.Find("Player").GetComponent<Player>();
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        _properAudioSource = GetComponent<AudioSource>();

        if (_player is null)
        {
            Debug.LogError("The Player is NULL");
        }

        if (_gameManager is null)
        {
            Debug.LogError("The Game Manager is NULL");
        }

        if (anim is null)
        {
            Debug.LogError("The animator is NULL");
        }

        if (_properAudioSource is null)
        {
            Debug.LogError("AudioSource is NULL");
        }
    }

    public virtual void Update()
    {
        if (Time.time > canShoot && !_dead)
        {
            Shoot();
        }
    }

    protected virtual void MoveEnemy()
    {
        TeleportIfOutOfBounds();
    }

    ////////////////////////////////
    //TRANSFORM PROPERTIES//////////
    ////////////////////////////////

    protected void TeleportIfOutOfBounds()
    {
        if (transform.position.y > _yLimit || transform.position.y < -_yLimit
            || transform.position.x > _xLimit || transform.position.x < -_xLimit)
        {
            SetNewTransform();
        }
    }

    protected virtual void SetNewTransform()
    {
        // Teleport on limits
        // If X limit is reached appear in the opposite side
        if (transform.position.x > _xLimit || transform.position.x < -_xLimit)
        {
            transform.position = new Vector3(_xLimit * (transform.position.x > _xLimit ? -1 : 1), transform.position.y, 0);
        }

        // If Y limit is reached appear in the opposite side
        if (transform.position.y > _yLimit || transform.position.y < -_yLimit)
        {
            transform.position = new Vector3(transform.position.x, _yLimit * (transform.position.y > _yLimit ? -1 : 1), 0);
        }
    }

    protected void SetNewSide(int side)
    {
        switch (side)
        {
            case 1: //left
                transform.position = new Vector3(-_xLimit, GetRandomY(), 0);
                break;
            case 2: //down
                transform.position = new Vector3(GetRandomX(), -_yLimit, 0);
                break;
            case 3: //right
                transform.position = new Vector3(_xLimit, GetRandomY(), 0);
                break;
            case 4: //up
                transform.position = new Vector3(GetRandomX(), _yLimit, 0);
                break;
        }
    }

    protected float GetRandomX()
    {
        return Random.Range(-_xLimit, _xLimit);
    }

    protected float GetRandomY()
    {
        return Random.Range(-_yLimit, _yLimit);
    }

    ////////////////////////////////
    //SHOOTING//////////////////////
    ////////////////////////////////

    void Shoot()
    {
        //Update delay time for laser
        canShoot = Time.time + _fireDelay;

        Instantiate(shot, transform.position, transform.rotation);

        //Shot AUDIO
        PlayAudio(1);
    }

    public void PowerUpFound()
    {
        Shoot();
    }

    ////////////////////////////////
    //COLLISIONS////////////////////
    ////////////////////////////////

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (_dead)
            return;

        //Collition with player
        if (other.CompareTag("Player"))
        {
            int dir = _player.GetDamageDirection(transform.position);
            _player.GetDamage(dir);
            GetDamage();
        }
        //Collition with Laser
        else if (other.CompareTag("PlayerShot"))
        {
            Destroy(other.gameObject);
            GetDamage();
        }
    }

    public void GetDamage()
    {
        _lives--;
        if (_lives < 1)
            DeathSequence();
    }

    ////////////////////////////////
    //ONDESTROY/////////////////////
    ////////////////////////////////

    public void DeathSequence()
    {
        PlayAudio(0);

        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        SpawnManager.enemyPool.Remove(transform);

        //to avoid making damage again while doing animation
        Destroy(GetComponent<Collider2D>());
        _dead = true;
        anim.SetTrigger("OnEnemyDeath");
        _speed = 0f;

        _player.IncreaseScore(_scoreValue);
        _gameManager.EnemyDestroyed();

        Destroy(gameObject, _deathTime);
    }

    ////////////////////////////////
    //AUDIO/////////////////////////
    ////////////////////////////////

    protected void PlayAudio(int index)
    {
        if (index < _audioClips.Length && index >= 0)
        {
            _properAudioSource.clip = _audioClips[index];
            _properAudioSource.Play();
        }
    }
}
