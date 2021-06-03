using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    //Properties of powerup
    [SerializeField]
    private float _speed = 3f;

    public enum RaritySelector
    {
        Common,
        Rare,
        Very_Rare
    }

    [SerializeField]
    private RaritySelector _rarity;

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
    //PROPERTIES////////////////////
    ////////////////////////////////

    public RaritySelector GetRarity()
    {
        return _rarity;
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
                    case 1: // speed energy refill
                        player.EnablePowerup(_powerID, -1f);
                        break;
                    case 2: //shield
                        player.EnablePowerup(_powerID, -1f);
                        break;
                    case 3: //extra life
                        player.EnablePowerup(_powerID, -1f);
                        break;
                    case 4: //Ammo refill
                        player.EnablePowerup(_powerID, -1f);
                        break;
                    case 5: //Multidirectional shot
                        player.EnablePowerup(_powerID, 5f);
                        break;
                }
            }
            AudioSource.PlayClipAtPoint(_collectedAudio, Camera.main.transform.position);
            Destroy(gameObject);
        }
    }
}
