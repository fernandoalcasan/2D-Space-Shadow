using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // UI score text
    [SerializeField]
    private Text _scoreText;

    //lives img
    [SerializeField]
    private Image _livesImg;

    //array for sprites
    [SerializeField]
    private Sprite[] _liveSprites;

    // Game over ui text
    [SerializeField]
    private Text _gameOverText;

    //Restart key ui text
    [SerializeField]
    private Text _restartText;

    // extra lives text
    [SerializeField]
    private Text _extraLivesText;

    //Ammo
    [SerializeField]
    private Text _ammoText;
    [SerializeField]
    private Image[] _ammoImgs;
    private int _currentAmmoImg;
    private int _maxAmmo;

    //Game Manager reference
    private GameManager _gameManager;

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

    //Audio Source & clips
    private AudioSource _uiAudioPlayer;
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

    public void UpdateBarEnergy(float value, float maxValue)
    {
        float amount = value / maxValue;
        _thrusterBarMask.fillAmount = amount;

        //change color
        switch(amount)
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

    public void UpdateThresholdEnergy(float value, float maxValue)
    {
        float amount = value / maxValue;
        _thrusterThresholdFill.fillAmount = amount;
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
        StartCoroutine(GameOverEffect());
        _gameManager.GameOver();
    }

    public void UpdateCurrentShot(int index)
    {
        _ammoImgs[_currentAmmoImg].gameObject.SetActive(false);
        _ammoImgs[index].gameObject.SetActive(true);
        _currentAmmoImg = index;
    }

    IEnumerator GameOverEffect()
    {
        while(true)
        {
            _gameOverText.text = "Game Over";
            yield return new WaitForSeconds(0.3f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
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
