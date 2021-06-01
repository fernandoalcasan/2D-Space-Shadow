using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Properties of the player
    [SerializeField]
    private float _speed = 5f;
    private float _speedBoost = 2f;
    private int _lives;
    [SerializeField]
    private int _maxLives = 3;
    private int _score;
    [SerializeField]
    private int _magazine = 15;
    private int _maxMagazine;

    //thruster properties
    [SerializeField]
    private float _thrusterEnergy = 100f;
    private float _maxThrusterEnergy;
    private float _boostDelay = 2f;
    private bool _canBoost = true;
    private float _maxBoost = 15f;
    private float _currentBoost = 0f;
    private float _energyPerFrame = 0.05f;

    //Fire properties
    [SerializeField]
    private float _fireDelay = 0.2f;
    private float _canShoot = -1f;

    //thrusters use
    private bool _isBoosting;

    //bounds of field
    private float _xLimit = 10f;
    private float _yLimit = 5.5f;

    //prefabs for shots
    [SerializeField]
    private GameObject[] _shots;
    [SerializeField]
    private GameObject _laser;
    [SerializeField]
    private GameObject _tripleLaser;
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

    //Audioclips
    [SerializeField]
    private AudioClip[] _audioClips;
    //AudioSource for proper trigger sounds
    private AudioSource _properAudioSource;
    //AudioSource for constant movement sounds
    private AudioSource _movementAudioSource;    

    void Start()
    {
        // Set the player position = 0
        transform.position = new Vector3(0, 0, 0);

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _properAudioSource = GetComponent<AudioSource>();
        _movementAudioSource = GameObject.Find("Player_Movement_Sounds").GetComponent<AudioSource>();
        _shieldBehavior = _shield.GetComponent<Shield>();
        _camShake = Camera.main.GetComponent<CameraShake>();

        _maxThrusterEnergy = _thrusterEnergy;
        _maxMagazine = _magazine;

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

        if (_movementAudioSource is null)
        {
            Debug.LogError("Movement Audio Source is NULL");
        }

        if (_shieldBehavior is null)
        {
            Debug.LogError("Shield script is NULL");
        }

        if(_camShake is null)
        {
            Debug.LogError("ShakeCamera script is NULL");
        }

        _lives = _maxLives;
        _currentShot = _shots[0];
        _uiManager.UpdateLives(_lives);
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
        if (Input.GetKey(KeyCode.LeftShift) && _canBoost)
        {
            //stop movement sound if it's playing
            if (!_isBoosting)
            {
                _movementAudioSource.Stop();
                _isBoosting = true;
            }

            //play the boost sound if it's not playing
            if (!_movementAudioSource.isPlaying)
            {
                PlayMovementAudio(3);
            }
            
            transform.Translate(dir * _speed * _speedBoost * Time.deltaTime);

            //waste energy
            _thrusterEnergy -= _energyPerFrame;

            //energy available?
            if (_thrusterEnergy <= 0f)
            {
                _canBoost = false;
                _thrusterEnergy = 0f;
            }

            //sum wasted energy to reach the threshold
            _currentBoost += _energyPerFrame;

            //threshold reached
            if(_currentBoost >= _maxBoost)
            {
                StartCoroutine(ThrusterNeedsBreak());
            }

            //Update UI
            _uiManager.UpdateBarEnergy(_thrusterEnergy, _maxThrusterEnergy);
            _uiManager.UpdateThresholdEnergy(_currentBoost, _maxBoost);
        }
        else
        {
            //stop boost sound if it's playing
            if (_isBoosting)
            {
                _movementAudioSource.Stop();
                _isBoosting = false;
            }

            //play the movement sound if it's not playing
            if (!_movementAudioSource.isPlaying)
            {
                PlayMovementAudio(4);
            }
            
            transform.Translate(dir * _speed * Time.deltaTime);

            //lower the energy wasted in a single boost
            if(_currentBoost > 0f)
            {
                _currentBoost -= (_energyPerFrame * 2);
                //Update UI
                _uiManager.UpdateThresholdEnergy(_currentBoost, _maxBoost);
            }
        }
    }

    IEnumerator ThrusterNeedsBreak()
    {
        //Deactivate thrusters
        _canBoost = false;

        //play animation
        _uiManager.ThresholdReached();

        //wait for the delay to activate thrusters
        yield return new WaitForSeconds(_boostDelay);

        //energy remaining? activate thrusters
        if (_thrusterEnergy > 0f)
        {
            _canBoost = true;
        }
        _currentBoost = 0f;
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
    }

    ////////////////////////////////
    //SHOOTING//////////////////////
    ////////////////////////////////

    void ShootLaser()
    {
        if(_magazine <= 0)
        {
            //Play empty mag audio
            PlayTriggerAudio(5);
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
            _fireDelay = forwardShot.GetFireRate();
        }
    }

    ////////////////////////////////
    //DAMAGE////////////////////////
    ////////////////////////////////

    public void GetDamage(int quadrant)
    {
        // if shield is active
        if (_shield.activeSelf)
        {
            bool stillsActive = _shieldBehavior.DamageShield();
            _shield.SetActive(stillsActive);
            return;
        }
        
        _lives--;
        _uiManager.UpdateLives(_lives);

        //Play damage audio
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
            case 1:// thruster energy unit refill
                _thrusterEnergy += _maxBoost;
                if (_thrusterEnergy > _maxThrusterEnergy)
                {
                    _thrusterEnergy = _maxThrusterEnergy;
                }
                _uiManager.UpdateBarEnergy(_thrusterEnergy, _maxThrusterEnergy);
                _canBoost = true;
                break;
            case 2: // shield
                if (_shield.activeSelf)
                {
                    _shieldBehavior.RestoreShield();
                }
                else
                {
                    _shield.SetActive(true);
                }
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
        }

        if (time > 0f) //if the powerup is temporary
        {
            StartCoroutine(DisableTempPowerup(power, time));
        }
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
            case 5: //Multidirectional shot
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

    void PlayMovementAudio(int index)
    {
        if (index < _audioClips.Length && index >= 0)
        {
            _movementAudioSource.clip = _audioClips[index];
            _movementAudioSource.Play();
        }
    }
}