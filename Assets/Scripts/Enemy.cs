using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //speed of the enemy
    [SerializeField]
    private float _speed = 4f;

    //Upper and inferior space limit
    private float _yLimit = 6f;

    void Start()
    {
        SetNewPos();
    }

    void Update()
    {
        //move towards the player
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        //Teleport enemy if it reaches the bottom
        if(transform.position.y < -_yLimit)
        {
            SetNewPos();
        }
    }

    void SetNewPos()
    {
        float randX = Random.Range(-10f, 10f);
        transform.position = new Vector3(randX, _yLimit, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Collided with: " + other.transform.name);

        //Collition with player
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (!(player is null))
            {
                player.GetDamage();
            }
            Destroy(gameObject);
        }
        //Collition with Laser
        else if (other.tag == "SimpleLaser")
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }

    }
}
