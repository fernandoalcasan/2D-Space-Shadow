using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    //speed of the laser
    [SerializeField]
    private float _speed = 8f;

    private bool _isEnemyLaser = false;

    //Bounds for the laser
    private float _yLimit = 7f;

    // Update is called once per frame
    void Update()
    {        
        if(_isEnemyLaser)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
        }

        if(transform.position.y > _yLimit || transform.position.y < -_yLimit)
        {
            DestroyLaser();
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

    private void DestroyLaser()
    {
        DestroyParent();
        Destroy(gameObject);
    }

}
