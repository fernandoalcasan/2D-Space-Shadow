using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //prefab of the enemy
    [SerializeField]
    private GameObject _enemy;

    [SerializeField]
    private float _spawnDelay = 2f;
    private float _canSpawn = -1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > _canSpawn)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        _canSpawn = Time.time + _spawnDelay;
        GameObject newEnemy = Instantiate(_enemy, Vector3.zero, Quaternion.identity);
    }
}
