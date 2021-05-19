using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Properties of the player
    [SerializeField]
    private float _speed = 5f;
    private float _speedBoost = 2f;
    [SerializeField]
    private int _lives = 3;
    private int _score;

    //Fire properties
    [SerializeField]
    private float _fireDelay = 0.2f;
    private float _canShoot = -1f;
    // 0 = triple shot, 1 = speed boost, 2 = shield
    private bool[] _powerupsEnabled = new bool[3];

    //bounds of field
    private float _xLimit = 10f;
    private float _yLimit = 5.5f;

    //prefabs for laser
    [SerializeField]
    private GameObject _laser;
    [SerializeField]
    private GameObject _tripleLaser;

    //prefab for shield
    [SerializeField]
    private GameObject _shield;

    //spawn manager connection
    private SpawnManager _spawnManager;
    // ui manager connection
    private UIManager _uiManager;

    //Damage VFX objects
    [SerializeField]
    private GameObject[] _damage = new GameObject[2];


    void Start()
    {
        // Set the player position = 0
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if(_spawnManager is null)
        {
            Debug.LogError("The spawn manager is NULL");
        }

        if(_uiManager is null)
        {
            Debug.LogError("The UI Manager is NULL");
        }

    }

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

        if(_powerupsEnabled[0])
        {
            Instantiate(_tripleLaser, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laser, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
        }
    }

    public void GetDamage(int quadrant)
    {
        // if shield is active
        if (_powerupsEnabled[2])
        {
            _shield.SetActive(false);
            _powerupsEnabled[2] = false;
            return;
        }
        
        _lives--;
        _uiManager.UpdateLives(_lives);

        switch(quadrant)
        {
            //right quadrants
            case 1: case 4:
                _damage[1].SetActive(true);
                break;
            //left quadrants
            case 2: case 3:
                _damage[0].SetActive(true);
                break;
            default:
                Debug.LogError("Quadrant not found");
                break;
        }
        
        if(_lives < 1)
        { 
            _spawnManager.OnPlayerDeath();
            _uiManager.OnPlayerDeath();
            Destroy(gameObject);
        }
    }

    public void EnablePowerup(int power, float time)
    {
        if(power < _powerupsEnabled.Length && power >= 0)
        {
            switch(power)
            {
                case 1:// speed boost
                    _speed *= _speedBoost;
                    break;
                case 2: // shield
                    _shield.SetActive(true);
                    break;
            }
            _powerupsEnabled[power] = true;
            if(time > 0f) //if the powerup is temporary
            {
                StartCoroutine(DisableTempPowerup(power, time));
            }
        }
        else
        {
            Debug.LogError("Index out of bounds");
        }
    }

    IEnumerator DisableTempPowerup(int power, float time)
    {
        yield return new WaitForSeconds(time);
        switch(power)
        {
            case 1:
                _speed /= _speedBoost;
                break;
        }
        _powerupsEnabled[power] = false;
    }

    public void IncreaseScore(int value)
    {
        _score += value;
        _uiManager.UpdateScore(_score);
    }

    public int GetDamageDirection(Vector3 otherPos)
    {
        Vector3 direction = otherPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        switch(angle)
        {
            //lower left quadrant
            case float x when x <= -90f:
                return 3;
            //lower right quadrant
            case float x when x <= 0f:
                return 4;
            //upper right quadrant
            case float x when x <= 90f:
                return 1;
            //upper left quadrant
            case float x when x <= 180f:
                return 2;
            default:
                return 0;
        }
    }
}