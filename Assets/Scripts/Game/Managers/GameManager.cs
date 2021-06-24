using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static Action OnGamePause;

    private bool _gameOver;

    [SerializeField]
    private int _maxWaves;
    private int _currentWave;

    [SerializeField]
    private int _enemiesPerWave;
    private int _currentEnemies;

    [SerializeField]
    private float _delayToSpawnEnemy;
    private float _currentDelay;
    [SerializeField][Range(0f,1f)]
    private float _reduceDelayPercentage;

    private UIManager _uiManager;
    private SpawnManager _spawnManager;

    private void Start()
    {
        _currentDelay = _delayToSpawnEnemy;
        OnGamePause = () =>
        {
            Time.timeScale = Time.timeScale == 1 ? 0 : 1;
            AudioListener.pause = !AudioListener.pause;
        };
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_uiManager is null)
        {
            Debug.LogError("UI Manager is NULL");
        }

        if(_spawnManager is null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R) && _gameOver)
        {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!(OnGamePause is null))
                OnGamePause();
            //Application.Quit();
        }
    }

    ////////////////////////////////
    //GAME MECHANICS////////////////
    ////////////////////////////////

    public void StartGame()
    {
        StartCoroutine(NextWave());
    }

    public IEnumerator NextWave()
    {
        _currentWave++;

        if(_currentWave == _maxWaves)
        {
            _currentEnemies = 1;
            _uiManager.DisplayNewWave("Final Wave", _currentEnemies);
            yield return new WaitForSeconds(3f);
            _uiManager.EnableBossUI();
            _spawnManager.SpawnNewWave(_currentEnemies, 0f, true);
        }
        else
        {
            _currentEnemies = _enemiesPerWave * _currentWave;
            _uiManager.DisplayNewWave("Wave " + _currentWave, _currentEnemies);
            yield return new WaitForSeconds(3f);
            UpdateSpawnTime();
            _spawnManager.SpawnNewWave(_currentEnemies, _currentDelay, false);
        }
    }

    private void UpdateSpawnTime()
    {
        _currentDelay -= (_currentDelay * _reduceDelayPercentage);

        if (_currentDelay < 0f)
            _currentDelay = 0;
    }

    public void EnemyDestroyed()
    {
        _currentEnemies--;
        if (_currentWave != _maxWaves)
            _uiManager.UpdateWaveEnemies(_currentEnemies, _enemiesPerWave * _currentWave);
        else
            _uiManager.UpdateWaveEnemies(_currentEnemies, 1);

        if (_currentEnemies == 0)
        {
            if(_currentWave == _maxWaves)
            {
                //End Game
                return;
            }
            StartCoroutine(NextWave());
        }
    }

    ////////////////////////////////
    //ON GAME OVER//////////////////
    ////////////////////////////////

    public void GameOver()
    {
        _gameOver = true;
    }
}
