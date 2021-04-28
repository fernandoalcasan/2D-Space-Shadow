using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //speed of the enemy
    [SerializeField]
    private float _speed = 4f;

    private float _yLimit = 6f;

    // Start is called before the first frame update
    void Start()
    {
        SetNewPos();
    }

    // Update is called once per frame
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
}
