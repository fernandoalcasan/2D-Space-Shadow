using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    private Transform _player;
    private bool _attracted;

    [Header("Power-Up Properties")]
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

    private void OnEnable()
    {
        Player.onAttract += ChangeAttraction;
    }

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Transform>();

        if (_player is null)
            Debug.LogError("Player transform is NULL");

        transform.position = new Vector3(Random.Range(-_xLimit, _xLimit), _yLimit, 0);
    }

    void Update()
    {
        if (!_attracted)
            MovePowerup();
        else
            MoveTowardsPlayer();
    }

    ////////////////////////////////
    //PROPERTIES////////////////////
    ////////////////////////////////

    public RaritySelector GetRarity()
    {
        return _rarity;
    }

    private void ChangeAttraction()
    {
        _attracted = !_attracted;
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

    private void MoveTowardsPlayer()
    {
        float step = _speed * Time.deltaTime * 2;
        transform.position = Vector3.MoveTowards(transform.position, _player.position, step);
    }

    ////////////////////////////////
    //ONDESTROY/////////////////////
    ////////////////////////////////

    private void OnDestroy()
    {
        Player.onAttract -= ChangeAttraction;
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
                    case 6: //freeze player powerup
                        player.EnablePowerup(_powerID, 2f);
                        break;
                }
            }
            AudioSource.PlayClipAtPoint(_collectedAudio, Camera.main.transform.position);
            Destroy(gameObject);
        }
    }
}
