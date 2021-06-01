using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardShot : MonoBehaviour
{
    [SerializeField]
    private Shot _shot;
    private SpriteRenderer _sprite;
    
    [Header("Set delay if Multicolor")]
    [SerializeField]
    private float _delayIfMulticolor;
    private WaitForSeconds _delay;

    void Start()
    {
        SetPos();

        if(_shot._isMultiColor)
        {
            _sprite = GetComponent<SpriteRenderer>();
            _delay = new WaitForSeconds(_delayIfMulticolor);

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
        if (_shot._isEnemyShot)
        {
            transform.Translate(Vector3.down * _shot._speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.up * _shot._speed * Time.deltaTime);
        }

        DestroyIfOutOfBounds();
    }

    ////////////////////////////////
    //POSITION/////////////////////
    ////////////////////////////////
    
    void SetPos()
    {
        Vector3 newPos = new Vector3(transform.position.x, (transform.position.y + _shot._verticalOffset), transform.position.z);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_shot._isEnemyShot && other.CompareTag("Player"))
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
        return _shot._fireRate;
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
        if (transform.position.y > _shot._yLimit || transform.position.y < -_shot._yLimit
            || transform.position.x > _shot._xLimit || transform.position.x < -_shot._xLimit)
        {
            DestroyShot();
        }
    }
}
