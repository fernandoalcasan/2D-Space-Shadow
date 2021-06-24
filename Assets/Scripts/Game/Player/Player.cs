using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public delegate void OnAttract();
    public static event OnAttract onAttract;

    // Properties of the player
    [SerializeField]
    private float _speed = 5f;
    private float _speedBoost = 2f;
    private int _lives;
    [SerializeField]
    private int _maxLives = 3;
    private int _score;
    [SerializeField]
    private int _magazine = 30;
    private int _maxMagazine;

    //Thrusters
    private Thrusters _thrusters;

    //Fire properties
    [SerializeField]
    private float _fireDelay = 0.2f;
    private float _canShoot = -1f;

    //bounds of field
    private float _xLimit = 10f;
    private float _yLimit = 5.5f;

    //prefabs for shots
    [SerializeField]
    private GameObject[] _shots;
    private GameObject _currentShot;

    //prefab for shield
    [SerializeField]
    private GameObject _shield;
    private Shield _shieldBehavior;

    //spawn manager connection
    private SpawnManager _spawnManager;
    // ui manager connection
    private UIManager _uiManager;

    // Camera shake reference
    private CameraShake _camShake;

    //Damage VFX objects
    [SerializeField]
    private GameObject[] _damage = new GameObject[2];

    //Animations
    private Animator _animator;

    //Audioclips
    [SerializeField]
    private AudioClip[] _audioClips;
    //AudioSource for proper trigger sounds
    private AudioSource _properAudioSource;

    void Start()
    {
        // Set the player position = 0
        transform.position = new Vector3(0, 0, 0);

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _properAudioSource = GetComponent<AudioSource>();
        _shieldBehavior = _shield.GetComponent<Shield>();
        _animator = GetComponent<Animator>();
        _camShake = Camera.main.GetComponent<CameraShake>();
        _thrusters = GetComponentInChildren<Thrusters>();

        if (_spawnManager is null)
        {
            Debug.LogError("The spawn manager is NULL");
        }

        if(_uiManager is null)
        {
            Debug.LogError("The UI Manager is NULL");
        }

        if(_properAudioSource is null)
        {
            Debug.LogError("Trigger Audio Source is NULL");
        }

        if (_shieldBehavior is null)
        {
            Debug.LogError("Shield script is NULL");
        }

        if (_animator is null)
        {
            Debug.LogError("Player animator is NULL");
        }

        if (_camShake is null)
        {
            Debug.LogError("ShakeCamera script is NULL");
        }
        
        if (_thrusters is null)
        {
            Debug.LogError("Thrusters script is NULL");
        }

        _maxMagazine = _magazine;
        _lives = _maxLives;
        _currentShot = _shots[0];
        _uiManager.UpdateLives(_lives);
        _uiManager.UpdateAmmo(_magazine);
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

        if(Input.GetKeyDown(KeyCode.C) || Input.GetKeyUp(KeyCode.C))
        {
            if(!(onAttract is null))
            {
                onAttract();
            }
        }
    }

    ////////////////////////////////
    //MOVEMENT//////////////////////
    ////////////////////////////////

    void MovePlayer()
    {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        // get direction of movement
        Vector3 dir = new Vector3(hInput, vInput, 0);

        // move player with or without boost
        if (Input.GetKey(KeyCode.LeftShift) && _thrusters.CanBoost)
        {
            transform.Translate(_speed * _speedBoost * Time.deltaTime * dir);
            _thrusters.UseThrusters();
        }
        else
        {
            transform.Translate(_speed * Time.deltaTime * dir);
            if (_thrusters.AreHot)
                _thrusters.FreezeThrusters();
        }
    }

    void LimitSpace()
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

    ////////////////////////////////
    //SHOOTING//////////////////////
    ////////////////////////////////

    void ShootLaser()
    {
        if(_magazine <= 0)
        {
            //Play empty mag audio
            PlayTriggerAudio(3);
            return;
        }

        Instantiate(_currentShot, transform.position, transform.rotation);
        UpdateFireRate();
        UpdateAmmo(--_magazine);
        PlayTriggerAudio(0);
    }

    void UpdateFireRate()
    {
        //Update delay time to shoot again
        _canShoot = Time.time + _fireDelay;
    }
    
    void UpdateAmmo(int value)
    {
        _magazine = value;
        _uiManager.UpdateAmmo(_magazine);
    }

    void SetCurrentShot(int index)
    {
        _currentShot = _shots[index];
        _uiManager.UpdateCurrentShot(index);
        SetCurrentShotBehavior(index);
    }

    void SetCurrentShotBehavior(int index)
    {
        SetShotFireRate(index);
    }

    void SetShotFireRate(int index)
    {
        if (_shots[index].TryGetComponent<ForwardShot>(out var forwardShot))
        {
            _fireDelay = forwardShot.FireRate;
        }
    }

    ////////////////////////////////
    //DAMAGE////////////////////////
    ////////////////////////////////

    public void GetDamage(int quadrant)
    {
        if (_shield.activeSelf)
            return;

        _lives--;
        _uiManager.UpdateLives(_lives);

        //Play damage audio
        if (_lives > 0)
            PlayTriggerAudio(1);

        //shake camera
        StartCoroutine(_camShake.ShakeCamera());

        //If lives are less or equal to the number of damage visualizers
        if (_lives <= _damage.Length)
        {
            ActivateDamageVisualizer(quadrant);
        }
        
        if (_lives < 1)
        {
            //Play explosion audio
            AudioSource.PlayClipAtPoint(_audioClips[2], Camera.main.transform.position);
            _spawnManager.OnPlayerDeath();
            _uiManager.OnPlayerDeath();
            Destroy(gameObject);
        }
    }

    void ActivateDamageVisualizer(int quadrant)
    {
        switch (quadrant)
        {
            //right quadrants
            case 1: case 4:
                if(_damage[1].activeSelf)
                {
                    _damage[0].SetActive(true);
                }
                else
                {
                    _damage[1].SetActive(true);
                }
                break;
            //left quadrants
            case 2: case 3:
                if (_damage[0].activeSelf)
                {
                    _damage[1].SetActive(true);
                }
                else
                {
                    _damage[0].SetActive(true);
                }
                break;
            default:
                Debug.LogError("Quadrant not found");
                break;
        }
    }

    public int GetDamageDirection(Vector3 otherPos)
    {
        Vector3 direction = otherPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        switch (angle)
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

    ////////////////////////////////
    //POWERUPS//////////////////////
    ////////////////////////////////

    public void EnablePowerup(int power, float time)
    {
        switch (power)
        {
            case 0: //Triple shot
                SetCurrentShot(1);
                break;
            case 1:// thruster energy refill
                _thrusters.RefillPower();
                break;
            case 2: // shield
                if (_shield.activeSelf)
                    _shieldBehavior.RestoreShield();
                else
                    _shield.SetActive(true);
                break;
            case 3: //extra life
                _lives++;
                _uiManager.UpdateLives(_lives);
                RestoreOneHealthVisualizer();
                break;
            case 4: //ammo refill
                _magazine = _maxMagazine;
                _uiManager.UpdateAmmo(_magazine);
                break;
            case 5: //Multidirectional shot
                SetCurrentShot(2);
                break;
            case 6: //freeze player powerup
                _speed = 0f;
                _animator.SetTrigger("Freeze");
                break;
            case 7: //scan shot powerup
                SetCurrentShot(3);
                break;
        }

        if (time > 0f) //if the powerup is temporary
            StartCoroutine(DisableTempPowerup(power, time));
    }

    void RestoreOneHealthVisualizer()
    {
        for (int i = 0; i < _damage.Length; i++)
        {
            if(_damage[i].activeSelf)
            {
                _damage[i].SetActive(false);
                return;
            }
        }
    }

    IEnumerator DisableTempPowerup(int power, float time)
    {
        yield return new WaitForSeconds(time);
        switch(power)
        {
            case 0: //triple shot
                if (_currentShot == _shots[1])
                    SetCurrentShot(0);
                break;
            case 5: //Multidirectional shot
                if (_currentShot == _shots[2])
                    SetCurrentShot(0);
                break;
            case 6: //Unfreeze player
                _speed = 5f;
                break;
            case 7: //Scan shot
                if (_currentShot == _shots[3])
                    SetCurrentShot(0);
                break;
        }
    }

    ////////////////////////////////
    //SCORE/////////////////////////
    ////////////////////////////////

    public void IncreaseScore(int value)
    {
        _score += value;
        _uiManager.UpdateScore(_score);
    }

    ////////////////////////////////
    //AUDIO/////////////////////////
    ////////////////////////////////
    
    void PlayTriggerAudio(int index)
    {
        if(index < _audioClips.Length && index >= 0)
        {
            _properAudioSource.clip = _audioClips[index];
            _properAudioSource.Play();
        }
    }
}