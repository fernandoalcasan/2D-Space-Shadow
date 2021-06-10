using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class EnemyBehavior : MonoBehaviour
{
    [SerializeField]
    private Enemy enemy;

    [Header("Behavior Properties")]
    [SerializeField]
    private float _speed;
    protected float Speed
    {
        get => _speed;
        private set { _speed = value; }
    }

    private float _canShoot = -1f;
    private bool _dead;

    [Header("References")]
    [SerializeField]
    protected GameObject shot;
    protected Animator anim;
    private AudioSource _properAudioSource;
    private Player _player;
    private GameManager _gameManager;

    public virtual void Start()
    {
        _canShoot = enemy.FireDelay;
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
        if (transform.position.y > enemy.YBound || transform.position.y < -enemy.YBound
            || transform.position.x > enemy.XBound || transform.position.x < -enemy.XBound)
        {
            SetNewTransform();
        }
    }

    protected virtual void SetNewTransform()
    {
        // Teleport on limits
        // If X limit is reached appear in the opposite side
        if (transform.position.x > enemy.XBound || transform.position.x < -enemy.XBound)
        {
            transform.position = new Vector3(enemy.XBound * (transform.position.x > enemy.XBound ? -1 : 1), transform.position.y, 0);
        }

        // If Y limit is reached appear in the opposite side
        if (transform.position.y > enemy.YBound || transform.position.y < -enemy.YBound)
        {
            transform.position = new Vector3(transform.position.x, enemy.YBound * (transform.position.y > enemy.YBound ? -1 : 1), 0);
        }
    }

    protected void SetNewSide(int side)
    {
        switch (side)
        {
            case 1: //left
                transform.position = new Vector3(-enemy.XBound, GetRandomY(), 0);
                break;
            case 2: //down
                transform.position = new Vector3(GetRandomX(), -enemy.YBound, 0);
                break;
            case 3: //right
                transform.position = new Vector3(enemy.XBound, GetRandomY(), 0);
                break;
            case 4: //up
                transform.position = new Vector3(GetRandomX(), enemy.YBound, 0);
                break;
        }
    }

    protected float GetRandomX()
    {
        return Random.Range(-enemy.XBound, enemy.XBound);
    }

    protected float GetRandomY()
    {
        return Random.Range(-enemy.YBound, enemy.YBound);
    }

    ////////////////////////////////
    //SHOOTING//////////////////////
    ////////////////////////////////

    void Shoot()
    {
        //Update delay time for laser
        _canShoot = Time.time + enemy.FireDelay;

        Instantiate(shot, transform.position, transform.rotation);

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
            _player.IncreaseScore(enemy.ScoreValue);
            DeathSequence();
        }
        //Collition with Laser
        else if (other.CompareTag("SimpleLaser"))
        {
            if (other.TryGetComponent<Shot>(out var shot))
            {
                if (shot.IsEnemyShot)
                    return;
            }
            else
                Debug.LogError("Shot component is NULL");

            //EXPLOSION AUDIO
            PlayAudio(0);
            Destroy(other.gameObject);
            _player.IncreaseScore(enemy.ScoreValue);
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
        _gameManager.EnemyDestroyed();
        anim.SetTrigger("OnEnemyDeath");
        Speed = 0f;
        Destroy(gameObject, enemy.DeathTime);
    }

    ////////////////////////////////
    //AUDIO/////////////////////////
    ////////////////////////////////

    protected void PlayAudio(int index)
    {
        if (index < enemy.AudioClips.Length && index >= 0)
        {
            _properAudioSource.clip = enemy.AudioClips[index];
            _properAudioSource.Play();
        }
    }
}
