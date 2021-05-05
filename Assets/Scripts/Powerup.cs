using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    //Properties of powerup
    [SerializeField]
    private float _speed = 3f;

    //Space Limits
    private float _xLimit = 10f;
    private float _yLimit = 6.5f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(Random.Range(-_xLimit, _xLimit), _yLimit, 0);
    }

    // Update is called once per frame
    void Update()
    {
        MovePowerup();

    }

    void MovePowerup()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if(transform.position.y < -_yLimit)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if(!(player is null))
            {
                player.enableTripleShot();
            }
            Destroy(gameObject);
        }
    }
}
