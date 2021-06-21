using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Text Elements")]
    // UI score text
    [SerializeField]
    private Text _scoreText;

    // Game over ui text
    [SerializeField]
    private Text _gameOverText;

    //Restart key ui text
    [SerializeField]
    private Text _restartText;

    // extra lives text
    [SerializeField]
    private Text _extraLivesText;

    //Wave texts
    [SerializeField]
    private Text _waveText;
    [SerializeField]
    private Text _currentWaveText;
    [SerializeField]
    private Text _currentEnemiesText;

    //Ammo
    [SerializeField]
    private Text _ammoText;

    [Header("Image Elements")]
    [SerializeField]
    private Image[] _ammoImgs;
    private int _currentAmmoImg;
    private int _maxAmmo;

    //lives img
    [SerializeField]
    private Image _livesImg;

    //array for sprites
    [SerializeField]
    private Sprite[] _liveSprites;

    //Thruster bar mask reference
    [SerializeField]
    private Image _thrusterBarMask;

    //Thruster bar reference
    [SerializeField]
    private Image _thrusterBarFill;

    //Thruster threshold fill reference
    [SerializeField]
    private Image _thrusterThresholdFill;

    //Animator reference
    private Animator _thresholdAnim;
    
    //Game Manager reference
    private GameManager _gameManager;

    //Audio Source & clips
    private AudioSource _uiAudioPlayer;

    [Header("Audio Elements")]
    [SerializeField]
    private AudioClip[] _uiSounds;


    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: 0";
        _currentAmmoImg = 0;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _thresholdAnim = GameObject.Find("Threshold_img").GetComponent<Animator>();
        _uiAudioPlayer = GameObject.Find("UI_Sounds").GetComponent<AudioSource>();

        if (_gameManager is null)
        {
            Debug.LogError("Game Manager is NULL");
        }
        if (_thresholdAnim is null)
        {
            Debug.LogError("Threshold animator is NULL");
        }
        if (_uiAudioPlayer is null)
        {
            Debug.LogError("UI Audio Source is NULL");
        }

        Thrusters.OnThrusterUsage += UpdateEnergyUIs;
    }

    ////////////////////////////////
    //UPDATE UI/////////////////////
    ////////////////////////////////

    public void UpdateScore(int value)
    {
        _scoreText.text = "Score: " + value;
    }

    public void UpdateLives(int lives)
    {
        if(lives >= 0 && lives <= 3)
        {
            _livesImg.sprite = _liveSprites[lives];
            _extraLivesText.text = "";
        }
        else
        {
            _extraLivesText.text = "+" + (lives - 3);
        }
    }

    private void UpdateEnergyUIs(float energy, float usage)
    {
        _thrusterBarMask.fillAmount = energy;
        _thrusterThresholdFill.fillAmount = usage;

        UpdateEnergyColor(energy);
        if (usage >= 1f)
            ThresholdReached();
    }

    private void UpdateEnergyColor(float value)
    {
        //change color
        switch (value)
        {
            case float x when x <= 0.3f:
                _thrusterBarFill.color = Color.red;
                break;
            case float x when x <= 0.75f:
                _thrusterBarFill.color = Color.yellow;
                break;
            case float x when x > 0.75f:
                _thrusterBarFill.color = Color.green;
                break;
        }
    }

    public void UpdateAmmo(int value)
    {
        if(value > _maxAmmo)
        {
            _maxAmmo = value;
        }
        _ammoText.text = value + "/" + _maxAmmo;
    }

    public void ThresholdReached()
    {
        _thresholdAnim.SetTrigger("Overheat");
        PlayAudio(0);
    }

    public void OnPlayerDeath()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartTextAnimation(ref _gameOverText, -1f);
        _gameManager.GameOver();
    }

    public void UpdateCurrentShot(int index)
    {
        _ammoImgs[_currentAmmoImg].gameObject.SetActive(false);
        _ammoImgs[index].gameObject.SetActive(true);
        _currentAmmoImg = index;
    }

    public void DisplayNewWave(string value, int enemies)
    {
        _waveText.gameObject.SetActive(true);
        _waveText.text = value;
        StartTextAnimation(ref _waveText, 3f);
        
        _currentWaveText.text = value;
        UpdateWaveEnemies(enemies, enemies);
    }

    public void UpdateWaveEnemies(int value, int maxValue)
    {
        _currentEnemiesText.text = "Enemies: " + value + "/" + maxValue;
    }

    ////////////////////////////////
    //ANIMATIONS////////////////////
    ////////////////////////////////

    void StartTextAnimation(ref Text toAnim, float duration)
    {
        if(toAnim.TryGetComponent<Animator>(out var anim))
        {
            anim.SetTrigger("Start"); 
            //If the animation was temporary, stop it
            if(duration > 0f)
            {
                StartCoroutine(WaitFor(duration, () => anim.SetTrigger("Stop")));
            }
        }
        else
        {
            Debug.LogError("Text animator is NULL");
        }
    }

    IEnumerator WaitFor(float seconds, Action finished = null)
    {
        yield return new WaitForSeconds(seconds);
        
        if(!(finished is null))
            finished();
    }

    ////////////////////////////////
    //AUDIO/////////////////////////
    ////////////////////////////////

    void PlayAudio(int index)
    {
        if (index < _uiSounds.Length && index >= 0)
        {
            _uiAudioPlayer.clip = _uiSounds[index];
            _uiAudioPlayer.Play();
        }
    }
}
