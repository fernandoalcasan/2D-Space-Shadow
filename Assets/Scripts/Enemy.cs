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

    void Start()
    {
        SetNewPos();
        _player = GameObject.Find("Player").GetComponent<Player>();

        if(_player is null)
        {
            Debug.LogError("The Player is NULL");
        }

        _anim = GetComponent<Animator>();

        if(_anim is null)
        {
            Debug.LogError("The animator is NULL");
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

    void SetNewPos()
    {
        float randX = Random.Range(-10f, 10f);
        transform.position = new Vector3(randX, _yLimit, 0);
    }

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
            _player.GetDamage();
            DeathSequence();
        }
        //Collition with Laser
        else if (other.tag == "SimpleLaser")
        {
            Destroy(other.gameObject);
            _player.IncreaseScore(10);
            DeathSequence();
        }
    }

    void DeathSequence()
    {
        _isEnemyDead = true;
        _anim.SetTrigger("OnEnemyDeath");
        _speed = 0f;
        Destroy(gameObject, 1.19f);
    }
}
