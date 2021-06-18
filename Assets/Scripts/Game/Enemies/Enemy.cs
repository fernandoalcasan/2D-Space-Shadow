using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class Enemy : MonoBehaviour
{
    [Header("Enemy Properties")]
    [SerializeField]
    private float _speed;
    protected float Speed { get => _speed; }

    [SerializeField]
    private bool _isSmart;
    [SerializeField]
    private float _delayIfSmart;
    [SerializeField]
    private float _fireDelay;
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
    [SerializeField]
    private GameObject _smartShot;
    protected Animator anim;
    private Player _player;
    private GameManager _gameManager;
    
    private float _canShoot = -1f;
    private bool _dead;
    private WaitForSeconds _smartDelay;

    public virtual void Start()
    {
        _canShoot = _fireDelay;
        _player = GameObject.Find("Player").GetComponent<Player>();
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        _properAudioSource = GetComponent<AudioSource>();

        if (_isSmart)
        {
            _smartDelay = new WaitForSeconds(_delayIfSmart);
            StartCoroutine(CheckIfPlayerIsBehind());
        }

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
        if (Time.time > _canShoot && !_dead)
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
        _canShoot = Time.time + _fireDelay;

        Instantiate(shot, transform.position, transform.rotation);

        //Shot AUDIO
        PlayAudio(1);
    }

    private IEnumerator CheckIfPlayerIsBehind()
    {
        float range = _xLimit * 2f;

        while (!_dead)
        {
            yield return _smartDelay;

            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.up, range);

            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    //delay the default shot (front)
                    _canShoot = Time.time + _fireDelay;
                    anim.SetTrigger("SmartShot");
                }
            }
        }
    }

    private void ShootBehind()
    {
        Quaternion inverseRot = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - 180f);
        Instantiate(_smartShot, transform.position, inverseRot);

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

    void OnTriggerEnter2D(Collider2D other)
    {
        //Collition with player
        if (other.CompareTag("Player"))
        {
            int dir = _player.GetDamageDirection(transform.position);
            _player.GetDamage(dir);
            DeathSequence();
        }
        //Collition with Laser
        else if (other.CompareTag("PlayerShot"))
        {
            Destroy(other.gameObject);
            DeathSequence();
        }
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
