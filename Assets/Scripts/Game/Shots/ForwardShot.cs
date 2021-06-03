using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardShot : MonoBehaviour
{
    [SerializeField]
    private Shot _shot;
    private SpriteRenderer _sprite;
    
    private WaitForSeconds _delay;

    void Start()
    {
        SetOffsetPos();

        if(_shot.isMultiColor)
        {
            _sprite = GetComponent<SpriteRenderer>();
            _delay = new WaitForSeconds(_shot.delayIfMulticolor);

            if (_sprite is null)
            {
                Debug.LogError("Sprite renderer is NULL");
            }
            else
            {
                StartCoroutine(SetColor());
            }
        }
    }

    void Update()
    {
        if (_shot.isEnemyShot)
        {
            transform.Translate(Vector3.down * _shot.speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.up * _shot.speed * Time.deltaTime);
        }

        DestroyIfOutOfBounds();
    }

    ////////////////////////////////
    //POSITION/////////////////////
    ////////////////////////////////
    
    void SetOffsetPos()
    {
        //Get the respective offsets to set the origin of the shot relative to the spaceship
        Vector3 vertFix = transform.up * _shot.verticalOffset;
        Vector3 horiFix = transform.right * _shot.horizontalOffset;

        //The vertical offset is subtracted or added as the models for player and enemy are inverted (rotation)
        if(_shot.isEnemyShot)
        {
            vertFix *= -1;

            //Random sign is used in here as the enemy have 2 possible cannons to shoot (horizontally)
            horiFix *= Random.value < 0.5f ? 1 : -1;
        }

        Vector3 newPos = transform.position + vertFix + horiFix;

        transform.position = newPos;
    }


    ////////////////////////////////
    //ANIMATION/////////////////////
    ////////////////////////////////

    private IEnumerator SetColor()
    {
        while(true)
        {
            _sprite.color = Color.red;
            yield return _delay;
            _sprite.color = Color.green;
            yield return _delay;
            _sprite.color = Color.blue;
            yield return _delay;
        }
    }

    ////////////////////////////////
    //DAMAGE////////////////////////
    ////////////////////////////////

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (_shot.isEnemyShot && other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (!(player is null))
            {
                int quadrant = player.GetDamageDirection(transform.position);
                player.GetDamage(quadrant);
            }
            Destroy(gameObject);
        }
    }

    ////////////////////////////////
    //SET & GET/////////////////////
    ////////////////////////////////

    public float GetFireRate()
    {
        return _shot.fireRate;
    }

    ////////////////////////////////
    //ONDESTROY/////////////////////
    ////////////////////////////////

    void DestroyParent()
    {
        if (!(transform.parent is null))
        {
            if(transform.parent.childCount <= 1)
            {
                Destroy(transform.parent.gameObject);
            }
        }
    }

    private void DestroyShot()
    {
        DestroyParent();
        Destroy(gameObject);
    }

    void DestroyIfOutOfBounds()
    {
        if (transform.position.y > _shot.yLimit || transform.position.y < -_shot.yLimit
            || transform.position.x > _shot.xLimit || transform.position.x < -_shot.xLimit)
        {
            DestroyShot();
        }
    }
}
