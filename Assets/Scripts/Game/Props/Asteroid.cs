using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 20f;
    private bool _isAsteroidDestroyed;

    //spawn manager reference
    private GameManager _gameManager;

    //Animator reference
    private Animator _animator;

    //Player reference
    private Player _player;

    //Audioclips
    [SerializeField]
    private AudioClip[] _audioClips;
    //AudioSource
    private AudioSource _properAudioSource;


    // Start is called before the first frame update
    void Start()
    {
        //set start position
        transform.position = new Vector3(0, 3f, 0);

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _animator = GetComponent<Animator>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _properAudioSource = GetComponent<AudioSource>();

        if (_gameManager is null)
        {
            Debug.LogError("The spawn manager is NULL");
        }

        if (_animator is null)
        {
            Debug.LogError("The animator is NULL");
        }

        if (_player is null)
        {
            Debug.LogError("The player is NULL");
        }

        if (_properAudioSource is null)
        {
            Debug.LogError("AudioSource is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        RotateAsteroid();
    }

    ////////////////////////////////
    //MOVEMENT//////////////////////
    ////////////////////////////////

    void RotateAsteroid()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime, Space.World);
    }

    ////////////////////////////////
    //DAMAGE////////////////////////
    ////////////////////////////////

    void OnTriggerEnter2D(Collider2D other)
    {
        if(_isAsteroidDestroyed)
        {
            return;
        }

        if(other.CompareTag("PlayerShot"))
        {
            Destroy(other.gameObject);
            OnAsteroidDestruction();
        }
        else if (other.CompareTag("Player"))
        {
            int dir = _player.GetDamageDirection(transform.position);
            _player.GetDamage(dir);
            OnAsteroidDestruction();
        }
    }

    ////////////////////////////////
    //ONDESTROY/////////////////////
    ////////////////////////////////

    void OnAsteroidDestruction()
    {
        //PLAY EXPLOSION AUDIO
        PlayAudio(0);
        _isAsteroidDestroyed = true;
        _gameManager.StartGame();
        _animator.SetTrigger("OnAsteroidBoom");
        Destroy(gameObject, 1.185f);
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
