using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //prefab of the enemy
    [SerializeField]
    private GameObject _enemy;

    //prefabs of the powerups: 0 = triple shot, 1 = speed refill, 2 = shield, 3 = extra life, 4 = ammo refill, 5 = MD shot 
    [SerializeField]
    private GameObject[] _powerups;
    private List<GameObject> _common, _rare, _veryRare;

    //to store the instances of enemies
    [SerializeField]
    private GameObject _enemyContainer;

    //Space Limits for powerup
    private float _xLimit = 10f;
    private float _yLimit = 6.5f;

    //to check if spawning is active
    private bool _doNotSpawn = false;

    private void Start()
    {
        InitializePowerups();
    }

    ////////////////////////////////
    //PROPERTIES////////////////////
    ////////////////////////////////

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

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(2f);
        while (!_doNotSpawn)
        {
            Vector3 xPosToSpawn = new Vector3(Random.Range(-_xLimit, _xLimit), 6, 0);
            GameObject newEnemy = Instantiate(_enemy, xPosToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator SpawnPowerups()
    {
        yield return new WaitForSeconds(2f);
        while (!_doNotSpawn)
        {
            Vector3 initPos = new Vector3(Random.Range(-_xLimit, _xLimit), _yLimit, 0);
            Instantiate(ChoosePowerupToSpawn(), initPos, Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(5f, 10f));
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

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPowerups());
    }

    ////////////////////////////////
    //ONDESTROY/////////////////////
    ////////////////////////////////

    public void OnPlayerDeath()
    {
        _doNotSpawn = true;
    }
}
