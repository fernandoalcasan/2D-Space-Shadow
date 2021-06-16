using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //prefab of enemy view
    [SerializeField]
    private GameObject _enemyView;

    [SerializeField]
    [Range(0f, 1f)]
    private float _viewProbability;

    //prefab of enemy shield
    [SerializeField]
    private GameObject _enemyShield;

    [SerializeField] [Range(0f, 1f)]
    private float _shieldProbability;
    
    //prefab of enemy follows
    [SerializeField]
    private GameObject _enemyAggressive;

    [SerializeField] [Range(0f, 1f)]
    private float _aggressiveProbability;

    //prefab of enemy avoids
    [SerializeField]
    private GameObject _enemyAvoids;

    [SerializeField] [Range(0f, 1f)]
    private float _avoidShotsProbability;

    //prefab of the enemy
    [SerializeField]
    private GameObject[] _enemies;

    //prefabs of the powerups: 0 = triple shot, 1 = speed refill, 2 = shield,
    //3 = extra life, 4 = ammo refill, 5 = MD shot, 6 = Freeze player 
    [SerializeField]
    private GameObject[] _powerups;
    private List<GameObject> _common, _rare, _veryRare;

    //to store the instances of enemies
    [SerializeField]
    private GameObject _enemyContainer;

    //to check if spawning is active
    private bool _doNotSpawn = false;

    private void Start()
    {
        InitializePowerups();
    }

    ////////////////////////////////
    //PROPERTIES////////////////////
    ////////////////////////////////

    public void SpawnNewWave(int num, float delay)
    {
        StartCoroutine(SpawnEnemies(num, delay));
    }

    void InitializePowerups()
    {
        _common = new List<GameObject>();
        _rare = new List<GameObject>();
        _veryRare = new List<GameObject>();
        for (int i = 0; i < _powerups.Length; i++)
        {
            if (_powerups[i].TryGetComponent<Powerup>(out var PowerupRef))
            {
                switch (PowerupRef.GetRarity())
                {
                    case Powerup.RaritySelector.Common:
                        _common.Add(_powerups[i]);
                        break;
                    case Powerup.RaritySelector.Rare:
                        _rare.Add(_powerups[i]);
                        break;
                    case Powerup.RaritySelector.Very_Rare:
                        _veryRare.Add(_powerups[i]);
                        break;
                }
            }
            else
            {
                Debug.LogError("Powerup Script component is NULL");
            }
        }

        if(_common.Count == 0 || _rare.Count == 0 || _veryRare.Count == 0)
        {
            Debug.Log("Please add at least one power-up of each rarity");
        }
    }

    ////////////////////////////////
    //SPAWNING//////////////////////
    ////////////////////////////////

    IEnumerator SpawnEnemies(int enemies, float delayToSpawn)
    {
        while (enemies > 0 && !_doNotSpawn)
        {
            int enemyToSpawn = Random.Range(0, _enemies.Length);
            GameObject newEnemy = Instantiate(_enemies[enemyToSpawn]);

            if(Random.value <= _shieldProbability)
                Instantiate(_enemyShield, newEnemy.transform);

            if (enemyToSpawn != 1 && Random.value <= _viewProbability)
                Instantiate(_enemyView, newEnemy.transform);

            if (enemyToSpawn != 1 && Random.value <= _aggressiveProbability)
                Instantiate(_enemyAggressive, newEnemy.transform);

            if (Random.value <= _avoidShotsProbability)
                Instantiate(_enemyAvoids, newEnemy.transform);

            newEnemy.transform.parent = _enemyContainer.transform;

            //If enemy spawned is a pair number, spawn a powerup
            if (enemies % 2 == 0)
                Instantiate(ChoosePowerupToSpawn());

            enemies--;

            yield return new WaitForSeconds(delayToSpawn);
        }
    }

    GameObject ChoosePowerupToSpawn()
    {
        //Common rarity gets 60%
        //Rare gets 30%
        //Very Rare gets 10%
        
        float levelProb = Random.value;
        switch(levelProb)
        {
            case float prob when (prob <= .6f): //common
                return _common[Random.Range(0, _common.Count)];
            case float prob when (prob <= .9f): //rare
                return _rare[Random.Range(0, _rare.Count)];
            case float prob when (prob <= 1f): //very rare
                return _veryRare[Random.Range(0, _veryRare.Count)];
            default:
                return null;
        }
    }

    ////////////////////////////////
    //ONDESTROY/////////////////////
    ////////////////////////////////

    public void OnPlayerDeath()
    {
        _doNotSpawn = true;
    }
}
