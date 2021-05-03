using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //prefab of the enemy
    [SerializeField]
    private GameObject _enemy;

    [SerializeField]
    private GameObject _enemyContainer;

    private bool _doNotSpawn = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnEnemies()
    {
        while (!_doNotSpawn)
        {
            Vector3 xPosToSpawn = new Vector3(Random.Range(-10f, 10f), 6, 0);
            GameObject newEnemy = Instantiate(_enemy, xPosToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(2f);
        }
    }

    public void OnPlayerDeath()
    {
        _doNotSpawn = true;
    }

}
