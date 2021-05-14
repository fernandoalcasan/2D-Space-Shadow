using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 20f;
    private bool _isAsteroidDestroyed;

    //spawn manager reference
    private SpawnManager _spawnManager;

    //Animator reference
    private Animator _animator;

    //Player reference
    private Player _player;


    // Start is called before the first frame update
    void Start()
    {
        //set start position
        transform.position = new Vector3(0, 3f, 0);

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if(_spawnManager is null)
        {
            Debug.LogError("The spawn manager is NULL");
        }

        _animator = GetComponent<Animator>();

        if (_animator is null)
        {
            Debug.LogError("The animator is NULL");
        }

        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player is null)
        {
            Debug.LogError("The player is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        RotateAsteroid();
    }

    void RotateAsteroid()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(_isAsteroidDestroyed)
        {
            return;
        }

        if(other.tag == "SimpleLaser")
        {
            Destroy(other.gameObject);
            OnAsteroidDestruction();
        }
        else if (other.tag == "Player")
        {
            _player.GetDamage();
            OnAsteroidDestruction();
        }
    }

    void OnAsteroidDestruction()
    {
        _isAsteroidDestroyed = true;
        _spawnManager.StartSpawning();
        _animator.SetTrigger("OnAsteroidBoom");
        Destroy(gameObject, 1.185f);
    }

}
