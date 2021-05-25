using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //prefab of the enemy
    [SerializeField]
    private GameObject _enemy;

    //prefabs of the powerups
    [SerializeField]
    private GameObject[] _powerups;

    //to store the instances of enemies
    [SerializeField]
    private GameObject _enemyContainer;

    //Space Limits for powerup
    private float _xLimit = 10f;
    private float _yLimit = 6.5f;

    //to check if spawning is active
    private bool _doNotSpawn = false;

    void Start()
    {

    }

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
            int randomPowerup = Random.Range(0, 4);
            Instantiate(_powerups[randomPowerup], initPos, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(5f, 10f));
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPowerups());
    }

    public void OnPlayerDeath()
    {
        _doNotSpawn = true;
    }
}
