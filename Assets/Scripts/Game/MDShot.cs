using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MDShot : ForwardShot
{
    [SerializeField]
    private GameObject _mdPelletShot;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Instantiate(_mdPelletShot, transform.position + transform.up, transform.rotation);
        }
    }
}
