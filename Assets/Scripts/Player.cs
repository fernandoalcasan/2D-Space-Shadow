using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Properties of the player
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private int _lives = 3;

    //Fire properties
    [SerializeField]
    private float _fireDelay = 0.2f;
    private float _canShoot = -1f;
    [SerializeField]
    private bool _tripleShotEnabled = false;

    //bounds of field
    private float _xLimit = 10f;
    private float _yLimit = 5.5f;

    //prefabs for laser
    [SerializeField]
    private GameObject _laser;
    [SerializeField]
    private GameObject _tripleLaser;

    //spawn manager connection
    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        // Set the player position = 0
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if(_spawnManager is null)
        {
            Debug.LogError("The spawn manager is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        LimitSpace();

        //Instantiate laser prefab with spacebar if not in cool down
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canShoot)
        {
            ShootLaser();
        }
    }

    void MovePlayer()
    {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");
        // get direction of movement
        Vector3 dir = new Vector3(hInput, vInput, 0);

        // move player
        transform.Translate(dir * _speed * Time.deltaTime);
    }

    void LimitSpace()
    {
        // Teleport on limits
        // If X limit is reached appear in the opposite side
        if (transform.position.x > _xLimit | transform.position.x < -_xLimit)
        {
            transform.position = new Vector3(_xLimit * (transform.position.x > _xLimit ? -1 : 1), transform.position.y, 0);
        }

        // If Y limit is reached appear in the opposite side
        if (transform.position.y > _yLimit | transform.position.y < -_yLimit)
        {
            transform.position = new Vector3(transform.position.x, _yLimit * (transform.position.y > _yLimit ? -1 : 1), 0);
        }

        // Stop on limits
        //transform.position = new Vector3(Mathf.Clamp(transform.position.x, -_xLimit, _xLimit), Mathf.Clamp(transform.position.y, -_yLimit, _yLimit), 0);
    }

    void ShootLaser()
    {
        //Update delay time for laser
        _canShoot = Time.time + _fireDelay;

        if(_tripleShotEnabled)
        {
            Instantiate(_tripleLaser, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laser, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
        }
    }

    public void GetDamage()
    {
        _lives--;

        if(_lives < 1)
        {
            if(!(_spawnManager is null))
            {
                _spawnManager.OnPlayerDeath();
            }
            Destroy(gameObject);
        }
    }

    public void EnableTripleShot()
    {
        _tripleShotEnabled = true;
        StartCoroutine(DisablePowerup());
    }

    IEnumerator DisablePowerup()
    {
        yield return new WaitForSeconds(5f);
        _tripleShotEnabled = false;
    }
}
