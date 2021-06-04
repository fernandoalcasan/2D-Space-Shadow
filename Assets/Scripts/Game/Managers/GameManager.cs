using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _gameOver;

    [SerializeField]
    private int _maxWaves;
    private int _currentWave;

    [SerializeField]
    private int _enemiesPerWave;
    private int _currentEnemies;

    private UIManager _uiManager;
    private SpawnManager _spawnManager;

    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        
        if(_uiManager is null)
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

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
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
        _currentEnemies = _enemiesPerWave * _currentWave;

        if(_currentWave == _maxWaves)
        {
            _uiManager.DisplayNewWave("Final Wave", _currentEnemies);
        }
        else
        {
            _uiManager.DisplayNewWave("Wave " + _currentWave, _currentEnemies);
        }
        yield return new WaitForSeconds(3f);

        float delayOfSpawn = 3f / (float)_currentWave;
        _spawnManager.SpawnNewWave(_currentEnemies, delayOfSpawn);
    }

    public void EnemyDestroyed()
    {
        _currentEnemies--;
        _uiManager.UpdateWaveEnemies(_currentEnemies, _enemiesPerWave * _currentWave);
        
        if(_currentEnemies == 0)
        {
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
