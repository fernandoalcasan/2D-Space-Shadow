using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //speed of the enemy
    [SerializeField]
    private float _speed = 4f;

    //Upper and inferior space limit
    private float _yLimit = 6f;

    //Fire properties
    [SerializeField]
    private float _fireDelay = 3f;
    private float _canShoot = -1f;

    [SerializeField]
    private GameObject _enemyLaser;

    //Player reference
    private Player _player;

    //Animator reference
    private Animator _anim;

    //Audioclips
    [SerializeField]
    private AudioClip[] _audioClips;
    //AudioSource
    private AudioSource _properAudioSource;

    void Start()
    {
        SetNewPos();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _properAudioSource = GetComponent<AudioSource>();

        if (_player is null)
        {
            Debug.LogError("The Player is NULL");
        }

        if(_anim is null)
        {
            Debug.LogError("The animator is NULL");
        }

        if (_properAudioSource is null)
        {
            Debug.LogError("AudioSource is NULL");
        }
    }

    void Update()
    {
        MoveEnemy();

        if (Time.time > _canShoot)
        {
            ShootLaser();
        }

    }

    ////////////////////////////////
    //SHOOTING//////////////////////
    ////////////////////////////////

    void ShootLaser()
    {
        _fireDelay = Random.Range(3f, 7f);
        //Update delay time for laser
        _canShoot = Time.time + _fireDelay;

        //Choose the side of the laser shoot
        Vector3 laserSide =transform.position - (transform.up * 1.33f) + (Vector3.right * (Random.Range(0,2) == 0 ? 0.184f : -0.184f));
        GameObject laser = Instantiate(_enemyLaser, laserSide, transform.rotation);
        
        //Set the laser as enemy's to change the behavior
        laser.GetComponent<Laser>().SetEnemyLaser();
        
        //Laser AUDIO
        PlayAudio(1);
    }

    ////////////////////////////////
    //MOVEMENT//////////////////////
    ////////////////////////////////

    void SetNewPos()
    {
        float randX = Random.Range(-10f, 10f);
        transform.position = new Vector3(randX, _yLimit, 0);
    }

    void MoveEnemy()
    {
        //move towards the player
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        
        //Teleport enemy if it reaches the bottom
        if (transform.position.y < -_yLimit)
        {
            SetNewPos();
        }
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
            DeathSequence();
        }
        //Collition with Laser
        else if (other.CompareTag("SimpleLaser"))
        {
            //EXPLOSION AUDIO
            PlayAudio(0);
            Destroy(other.gameObject);
            _player.IncreaseScore(10);
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
        _anim.SetTrigger("OnEnemyDeath");
        _speed = 0f;
        Destroy(gameObject, 1.19f);
    }

    ////////////////////////////////
    //AUDIO/////////////////////////
    ////////////////////////////////

    void PlayAudio(int index)
    {
        if (index < _audioClips.Length && index >= 0)
        {
            _properAudioSource.clip = _audioClips[index];
            _properAudioSource.Play();
        }
    }

}
