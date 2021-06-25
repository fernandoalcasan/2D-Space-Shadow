using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static Action OnGamePause;
    public static Action OnNewWave;
    public static Action<bool> OnGameOver;
    public static bool gamePaused;
    public static int currentWave, maxWaves, currentEnemies, maxEnemies;

    private bool _gameOver;

    [SerializeField]
    private int _maxWaves;

    [SerializeField]
    private int _enemiesPerWave;

    private void Start()
    {
        maxWaves = _maxWaves;
        OnGamePause += () =>
        {
            gamePaused = !gamePaused;
            Time.timeScale = Time.timeScale == 1 ? 0 : 1;
            AudioListener.pause = !AudioListener.pause;
        };

        OnGameOver += (value) => _gameOver = true;

        Player.OnPlayerlives += IsGameOver;
        Enemy.OnEnemyDestroyed += EnemyDestroyed;
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
        NextWave();
    }

    private void NextWave()
    {
        currentWave++;

        if(currentWave == maxWaves)
            currentEnemies = 1;
        else
            currentEnemies = _enemiesPerWave * currentWave;

        maxEnemies = currentEnemies;

        if (!(OnNewWave is null))
            OnNewWave();
    }
    private void IsGameOver(int lives)
    {
        if (lives < 1 && !(OnGameOver is null))
            OnGameOver(false);
    }

    private void EnemyDestroyed(int scoreValue)
    {
        currentEnemies--;

        if (currentEnemies == 0)
        {
            if(currentWave == maxWaves)
            {
                if (!(OnGameOver is null))
                    OnGameOver(true);
                return;
            }
            NextWave();
        }
    }

    ////////////////////////////////
    //ON DESTROY////////////////////
    ////////////////////////////////

    private void OnDestroy()
    {
        Player.OnPlayerlives -= IsGameOver;
        Enemy.OnEnemyDestroyed -= EnemyDestroyed;
        OnGamePause = null;
        OnGameOver = null;
        OnNewWave = null;
        currentWave = 0;
        currentEnemies = 0;
        maxEnemies = 0;
        gamePaused = false;
    }
}
