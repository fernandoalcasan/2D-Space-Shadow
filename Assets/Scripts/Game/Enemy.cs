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

    //Player reference
    private Player _player;

    //Animator reference
    private Animator _anim;

    //Death indicator
    private bool _isEnemyDead;

    //Audioclips
    [SerializeField]
    private AudioClip[] _audioClips;
    //AudioSource
    private AudioSource _audioSource;

    void Start()
    {
        SetNewPos();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        if (_player is null)
        {
            Debug.LogError("The Player is NULL");
        }

        if(_anim is null)
        {
            Debug.LogError("The animator is NULL");
        }

        if (_audioSource is null)
        {
            Debug.LogError("AudioSource is NULL");
        }
    }

    void Update()
    {
        //move towards the player
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        //Teleport enemy if it reaches the bottom
        if(transform.position.y < -_yLimit)
        {
            SetNewPos();
        }
    }

    ////////////////////////////////
    //POSITION//////////////////////
    ////////////////////////////////

    void SetNewPos()
    {
        float randX = Random.Range(-10f, 10f);
        transform.position = new Vector3(randX, _yLimit, 0);
    }

    ////////////////////////////////
    //COLLISIONS////////////////////
    ////////////////////////////////

    void OnTriggerEnter2D(Collider2D other)
    {
        //to avoid making damage again while doing animation
        if(_isEnemyDead)
        {
            return;
        }

        //Collition with player
        if (other.tag == "Player")
        {
            //EXPLOSION AUDIO
            PlayAudio(0);
            int dir = _player.GetDamageDirection(transform.position);
            _player.GetDamage(dir);
            DeathSequence();
        }
        //Collition with Laser
        else if (other.tag == "SimpleLaser")
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
        _isEnemyDead = true;
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
            _audioSource.clip = _audioClips[index];
            _audioSource.Play();
        }
    }

}
