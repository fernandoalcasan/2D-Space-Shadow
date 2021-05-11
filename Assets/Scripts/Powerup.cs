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

    //Power-up IDs
    [SerializeField]
    private int _powerID;

    void Start()
    {
        transform.position = new Vector3(Random.Range(-_xLimit, _xLimit), _yLimit, 0);
    }

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
                switch(_powerID)
                {
                    case 0:
                        player.EnablePowerup(_powerID, 5f);
                        break;
                    case 1:
                        player.EnablePowerup(_powerID, 5f);
                        break;
                    case 2:
                        player.EnablePowerup(_powerID, -1f);
                        break;
                }
            }
            Destroy(gameObject);
        }
    }
}
