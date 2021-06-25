using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public class EnemyFeature
{
    [SerializeField]
    private GameObject _feature;
    public GameObject Feature { get => _feature; }

    [SerializeField][Range(0f, 1f)]
    private float _probability;
    public float Probability { get => _probability; set => _probability = value; }

    [SerializeField][Range(0f, 1f)]
    private float _increasePercentage;
    public float IncreasingPercentage { get => _increasePercentage; }

    //array to store the index of enemy prefabs in spawn manager that are not allowed to have this feature
    [SerializeField]
    private int[] _enemiesNotAllowed;
    public bool IsEnemyNotAllowed(int id) 
    {
        return _enemiesNotAllowed.Contains(id);
    }
}

[System.Serializable]
public class PowerUpItem
{
    [SerializeField]
    private GameObject _item;
    public GameObject Item { get => _item; }

    public enum RarityLevel
    {
        Common,
        Rare,
        Very_Rare
    }

    [SerializeField]
    private RarityLevel _rarity;
    public RarityLevel Rarity { get => _rarity; }
}

public class SpawnManager : MonoBehaviour
{
    public static List<Transform> enemyPool;
    public static Func<Vector3, Transform> NearestEnemy;

    [Header("Spawning Properties")]
    [SerializeField]
    private float _delayToSpawnEnemy;
    private float _currentDelay;
    [SerializeField]
    [Range(0f, 1f)]
    private float _reduceDelayPercentage;

    [Header("Enemy features")]
    [SerializeField]
    private EnemyFeature[] _features;

    [Header("Prefabs to spawn")]
    //prefab of the enemy
    [SerializeField]
    private GameObject[] _enemies;
    [SerializeField]
    private GameObject _finalBoss;

    //prefabs of the powerups: 0 = triple shot, 1 = speed refill, 2 = shield,
    //3 = extra life, 4 = ammo refill, 5 = MD shot, 6 = Freeze player, 7 = Scan Shot
    [SerializeField]
    private PowerUpItem[] _powerupItems;
    private List<PowerUpItem> _commonItems, _rareItems, _veryRareItems;

    //to store the instances of enemies
    [SerializeField]
    private GameObject _enemyContainer;

    private WaitForSeconds _enemyDelay;
    private WaitForSeconds _powerupDelay;
    private bool _wavesEnabled;

    //to check if spawning is active
    private bool _doNotSpawn = false;

    private void Start()
    {
        _currentDelay = _delayToSpawnEnemy;
        enemyPool = new List<Transform>();
        NearestEnemy = (pos) => enemyPool.OrderBy(ePos => (ePos.position - pos).sqrMagnitude).FirstOrDefault();

        InitializePowerups();
        GameManager.OnNewWave += SpawnNewWave;
        GameManager.OnGameOver += GameOver;
    }

    ////////////////////////////////
    //PROPERTIES////////////////////
    ////////////////////////////////

    private void SpawnNewWave()
    {
        StartCoroutine(SpawnNewWaveAfter());
    }

    private IEnumerator SpawnNewWaveAfter()
    {
        yield return new WaitForSeconds(3f);
        if(GameManager.currentWave < GameManager.maxWaves)
            StartCoroutine(SpawnEnemies(GameManager.currentEnemies));
        else
            SpawnBoss();
    }

    void InitializePowerups()
    {
        _commonItems = _powerupItems.Where((powerup) => powerup.Rarity == PowerUpItem.RarityLevel.Common).ToList();
        _rareItems = _powerupItems.Where((powerup) => powerup.Rarity == PowerUpItem.RarityLevel.Rare).ToList();
        _veryRareItems = _powerupItems.Where((powerup) => powerup.Rarity == PowerUpItem.RarityLevel.Very_Rare).ToList();

        if(_commonItems.Count == 0 || _rareItems.Count == 0 || _veryRareItems.Count == 0)
        {
            Debug.Log("Please add at least one power-up of each rarity");
        }
    }

    ////////////////////////////////
    //SPAWNING//////////////////////
    ////////////////////////////////

    IEnumerator SpawnEnemies(int enemies)
    {
        UpdateSpawnTime();
        UpdateWaveProperties();

        if (!_wavesEnabled)
        {
            _wavesEnabled = true;
            StartCoroutine(SpawnPowerups());
        }

        while (enemies > 0 && !_doNotSpawn)
        {
            int enemyToSpawn = UnityEngine.Random.Range(0, _enemies.Length);
            GameObject newEnemy = Instantiate(_enemies[enemyToSpawn]);

            enemyPool.Add(newEnemy.transform);
            
            for (int i = 0; i < _features.Length; i++)
            {
                if(UnityEngine.Random.value <= _features[i].Probability)
                {
                    if(!_features[i].IsEnemyNotAllowed(enemyToSpawn))
                        Instantiate(_features[i].Feature, newEnemy.transform);
                }
            }

            newEnemy.transform.parent = _enemyContainer.transform;

            enemies--;

            yield return _enemyDelay;
        }
    }

    private void SpawnBoss()
    {
        GameObject newEnemy = Instantiate(_finalBoss);
        enemyPool.Add(newEnemy.transform);
        newEnemy.transform.parent = _enemyContainer.transform;
    }

    private IEnumerator SpawnPowerups()
    {
        while(!_doNotSpawn)
        {
            Instantiate(ChoosePowerupToSpawn());
            yield return _powerupDelay;
        }
    }

    GameObject ChoosePowerupToSpawn()
    {
        //Common rarity gets 60%
        //Rare gets 30%
        //Very Rare gets 10%
        
        float levelProb = UnityEngine.Random.value;
        switch(levelProb)
        {
            case float prob when (prob <= .6f): //common
                return _commonItems[UnityEngine.Random.Range(0, _commonItems.Count)].Item;
            case float prob when (prob <= .9f): //rare
                return _rareItems[UnityEngine.Random.Range(0, _rareItems.Count)].Item;
            case float prob when (prob <= 1f): //very rare
                return _veryRareItems[UnityEngine.Random.Range(0, _veryRareItems.Count)].Item;
            default:
                return null;
        }
    }

    private void UpdateSpawnTime()
    {
        _currentDelay -= (_currentDelay * _reduceDelayPercentage);

        if (_currentDelay < 0f)
            _currentDelay = 0;
    }

    ////////////////////////////////
    //PROPERTIES////////////////////
    ////////////////////////////////

    private void UpdateWaveProperties()
    {
        _enemyDelay = new WaitForSeconds(_currentDelay);
        _powerupDelay = new WaitForSeconds(_currentDelay * 1.5f);

        if (!_wavesEnabled)
            return;

        for (int i = 0; i < _features.Length; i++)
        {
            if (_features[i].Probability < 1f)
                _features[i].Probability *= 1f + _features[i].IncreasingPercentage;
            if (_features[i].Probability > 1f)
                _features[i].Probability = 1f;
        }
    }

    ////////////////////////////////
    //ONDESTROY/////////////////////
    ////////////////////////////////

    private void GameOver(bool win)
    {
        StopAllCoroutines();
        _doNotSpawn = true;
    }

    private void OnDestroy()
    {
        GameManager.OnNewWave -= SpawnNewWave;
        GameManager.OnGameOver -= GameOver;
    }
}
