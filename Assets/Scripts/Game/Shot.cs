using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Shot
{
    [Header("Shot Attributes")]
    public float _speed;
    public bool _isEnemyShot;
    public bool _isMultiColor;
    public float _fireRate;
    public float _verticalOffset;

    [Header("Space Bounds")]
    public float _yLimit;
    public float _xLimit;

    public Shot(float speed, bool isEnemyShot, bool isMultiColor, float fireRate, float verticalOffset, Vector2 bounds)
    {
        _speed = speed;
        _isEnemyShot = isEnemyShot;
        _isMultiColor = isMultiColor;
        _fireRate = fireRate;
        _verticalOffset = verticalOffset;
        _xLimit = bounds.x;
        _yLimit = bounds.y;
    }
}
