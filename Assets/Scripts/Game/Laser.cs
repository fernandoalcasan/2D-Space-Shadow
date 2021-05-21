using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    //speed of the laser
    [SerializeField]
    private float _speed = 8f;

    private bool _isEnemyLaser = false;

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("laser dir is: " + new Vector3(0,,90).normalized);
        
        if(_isEnemyLaser)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
        }
    }

    ////////////////////////////////
    //ASSIGN PROPERTIES/////////////
    ////////////////////////////////

    public void SetEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    ////////////////////////////////
    //DAMAGE///////////////////////
    ////////////////////////////////

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_isEnemyLaser && other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if(!(player is null))
            {
                int quadrant = player.GetDamageDirection(transform.position);
                player.GetDamage(quadrant);
            }
            Destroy(gameObject);
        }
    }



    ////////////////////////////////
    //ONDESTROY/////////////////////
    ////////////////////////////////

    void DestroyParent()
    {
        if(!(transform.parent is null))
        {
            Destroy(transform.parent.gameObject);
        }
    }

    // if the laser goes off the camera gets destroyed
    private void OnBecameInvisible()
    {
        DestroyParent();
        Destroy(gameObject);
    }
}
