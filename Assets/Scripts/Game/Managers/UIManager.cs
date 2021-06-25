using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private GameObject _pauseMenu;

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

    //Final Boss life mask reference
    [SerializeField]
    private Image _bossLifeMask;

    //Audio Source & clips
    private AudioSource _uiAudioPlayer;

    [Header("Audio Elements")]
    [SerializeField]
    private AudioClip[] _uiSounds;

    private int _score;

    void Start()
    {
        _scoreText.text = "Score: 0";
        _currentAmmoImg = 0;
        _gameOverText.gameObject.SetActive(false);
        _thresholdAnim = GameObject.Find("Threshold_img").GetComponent<Animator>();
        _uiAudioPlayer = GameObject.Find("UI_Sounds").GetComponent<AudioSource>();

        if (_thresholdAnim is null)
        {
            Debug.LogError("Threshold animator is NULL");
        }
        if (_uiAudioPlayer is null)
        {
            Debug.LogError("UI Audio Source is NULL");
        }

        Thrusters.OnThrusterUsage += UpdateEnergyUIs;
        FinalBoss.OnBossDamage += UpdateBossLife;
        GameManager.OnGamePause += EnablePauseMenu;
        GameManager.OnNewWave += DisplayWave;
        GameManager.OnNewWave += UpdateWaveEnemies;
        GameManager.OnGameOver += GameOver;
        Enemy.OnEnemyDestroyed += UpdateEnemyProperties;
        Player.OnPlayerShot += UpdateAmmo;
        Player.OnPlayerSpecialShot += UpdateCurrentShot;
        Player.OnPlayerlives += UpdateLives;

        _uiAudioPlayer.ignoreListenerPause = true;
    }

    ////////////////////////////////
    //UPDATE UI/////////////////////
    ////////////////////////////////

    private void UpdateEnemyProperties(int scoreValue)
    {
        if(scoreValue > 0)
        {
            _score += scoreValue;
            UpdateScore();
        }
        UpdateWaveEnemies();
    }

    private void UpdateLives(int lives)
    {
        if(lives >= 0 && lives <= 3)
        {
            _livesImg.sprite = _liveSprites[lives];
            _extraLivesText.text = "";
            if (lives == 0)
                UpdateEnergyUIs(1f, 0f);
        }
        else
            _extraLivesText.text = "+" + (lives - 3);
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

    private void UpdateAmmo(int value)
    {
        if(value > _maxAmmo)
            _maxAmmo = value;

        _ammoText.text = value + "/" + _maxAmmo;
    }

    private void ThresholdReached()
    {
        _thresholdAnim.SetTrigger("Overheat");
        AudioManager.audioSource.PlayOneShot(_uiSounds[0], 1f);
    }

    private void GameOver(bool win)
    {
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartTextAnimation(ref _gameOverText, -1f);

        if (win)
            _gameOverText.text = "You Win!";
    }

    private void UpdateCurrentShot(int index)
    {
        _ammoImgs[_currentAmmoImg].gameObject.SetActive(false);
        _ammoImgs[index].gameObject.SetActive(true);
        _currentAmmoImg = index;
    }

    private void DisplayWave()
    {
        _waveText.gameObject.SetActive(true);

        if (GameManager.currentWave == GameManager.maxWaves)
        {
            StartCoroutine(EnableBossUI());
            _waveText.text = "Final Wave";
        }
        else
            _waveText.text = "Wave " + GameManager.currentWave;

        StartTextAnimation(ref _waveText, 3f);
        _currentWaveText.text = "Wave " + GameManager.currentWave;
    }

    private void UpdateWaveEnemies()
    {
        _currentEnemiesText.text = "Enemies: " + GameManager.currentEnemies + "/" + GameManager.maxEnemies;
    }

    private void UpdateScore()
    {
        _scoreText.text = "Score: " + _score;
    }

    private IEnumerator EnableBossUI()
    {
        yield return new WaitForSeconds(3f);
        _bossLifeMask.transform.parent.gameObject.SetActive(true);
    }

    private void UpdateBossLife(float life)
    {
        _bossLifeMask.fillAmount = life;

        if (life == 0f)
            _bossLifeMask.transform.parent.gameObject.SetActive(false);
    }

    ////////////////////////////////
    //PAUSE MENU////////////////////
    ////////////////////////////////

    private void EnablePauseMenu()
    {
        if(!_pauseMenu.activeSelf)
            _uiAudioPlayer.PlayOneShot(_uiSounds[1], 1f);
        _pauseMenu.SetActive(!_pauseMenu.activeSelf);
    }

    public void ResumeBtn()
    {
        if (!(GameManager.OnGamePause is null))
            GameManager.OnGamePause();
        else
            Debug.LogError("Game Manager pause action is NULL");
    }

    public void RestartBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        GameManager.OnGamePause -= EnablePauseMenu;
        if (!(GameManager.OnGamePause is null))
            GameManager.OnGamePause();
        else
            Debug.LogError("Game Manager pause action is NULL");

    }
    
    public void MainMenuBtn()
    {
        SceneManager.LoadScene(0);

        GameManager.OnGamePause -= EnablePauseMenu;
        if (!(GameManager.OnGamePause is null))
            GameManager.OnGamePause();
        else
            Debug.LogError("Game Manager pause action is NULL");
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
    //ON DESTROY////////////////////
    ////////////////////////////////

    private void OnDestroy()
    {
        Thrusters.OnThrusterUsage -= UpdateEnergyUIs;
        FinalBoss.OnBossDamage -= UpdateBossLife;
        GameManager.OnGamePause -= EnablePauseMenu;
        GameManager.OnNewWave -= DisplayWave;
        GameManager.OnNewWave -= UpdateWaveEnemies;
        GameManager.OnGameOver -= GameOver;
        Enemy.OnEnemyDestroyed -= UpdateEnemyProperties;
        Player.OnPlayerShot -= UpdateAmmo;
        Player.OnPlayerSpecialShot -= UpdateCurrentShot;
        Player.OnPlayerlives -= UpdateLives;
    }
}
