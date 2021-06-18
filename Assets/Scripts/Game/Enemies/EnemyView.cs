using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    private Enemy _enemy;

    void Start()
    {
        _enemy = GetComponentInParent<Enemy>();

        if (_enemy is null)
            Debug.LogError("Enemy behavior is NULL");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("PowerUp"))
        {
            _enemy.PowerUpFound();
        }
    }
}
