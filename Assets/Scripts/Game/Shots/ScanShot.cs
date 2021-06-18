using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanShot : ForwardShot
{
    [Header("Scan Shot Properties")]
    [SerializeField]
    private float _spinsPerSecond;
    private Transform _enemyScanned;

    public override void Update()
    {
        if (!(_enemyScanned is null))
        {
            RotateShot();
            float followStep = Speed * 1.5f * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _enemyScanned.position, followStep);
        }
        else
            base.Update();
    }

    private void FixedUpdate()
    {
        _enemyScanned = GetNearestEnemy();
    }

    ////////////////////////////////
    //ROTATION//////////////////////
    ////////////////////////////////

    private void RotateShot()
    {
        transform.Rotate(_spinsPerSecond * 360f * Time.deltaTime * Vector3.forward, Space.World);
    }

    ////////////////////////////////
    //FOLLOW////////////////////////
    ////////////////////////////////

    private Transform GetNearestEnemy()
    {
        if (!(SpawnManager.NearestEnemy is null))
            return SpawnManager.NearestEnemy(transform.position);
        return null;
    }
}
