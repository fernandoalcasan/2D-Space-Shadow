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

    //Collection audio
    [SerializeField]
    private AudioClip _collectedAudio;

    void Start()
    {
        transform.position = new Vector3(Random.Range(-_xLimit, _xLimit), _yLimit, 0);
    }

    void Update()
    {
        MovePowerup();
    }

    ////////////////////////////////
    //MOVEMENT//////////////////////
    ////////////////////////////////

    void MovePowerup()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if(transform.position.y < -_yLimit)
        {
            Destroy(gameObject);
        }
    }

    ////////////////////////////////
    //ONDESTROY/////////////////////
    ////////////////////////////////

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if(!(player is null))
            {
                switch(_powerID)
                {
                    case 0: //triple shot
                        player.EnablePowerup(_powerID, 5f);
                        break;
                    case 1: // speed boost
                        player.EnablePowerup(_powerID, 5f);
                        break;
                    case 2: //shield
                        player.EnablePowerup(_powerID, -1f);
                        break;
                }
            }
            AudioSource.PlayClipAtPoint(_collectedAudio, transform.position);
            Destroy(gameObject);
        }
    }
}
