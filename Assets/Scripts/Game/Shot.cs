using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Shot
{
    [Header("Shot Attributes")]
    public float speed;
    public bool isEnemyShot;
    public bool isMultiColor;
    public float fireRate;
    public float verticalOffset;
    public float horizontalOffset;

    [Header("Set delay if Multicolor")]
    public float delayIfMulticolor;

    [Header("Space Bounds")]
    public float yLimit;
    public float xLimit;
}
