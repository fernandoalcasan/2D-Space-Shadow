using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MDShot : ForwardShot
{
    [Header("Pellet Shot prefab")]
    [SerializeField]
    private GameObject _mdPelletShot;

    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Instantiate(_mdPelletShot, transform.position + transform.up, transform.rotation);
        }
    }
}
