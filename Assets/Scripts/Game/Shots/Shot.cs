using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    [Header("Shot Attributes")]
    [SerializeField]
    private float _speed;
    [SerializeField]
    private bool _isEnemyShot;
    [SerializeField]
    private bool _isMultiColor;
    [SerializeField]
    private float _fireRate;
    [SerializeField]
    private float _verticalOffset;
    [SerializeField]
    private float _horizontalOffset;

    public float Speed { get => _speed; }
    public bool IsEnemyShot { get => _isEnemyShot; }
    public bool IsMultiColor { get => _isMultiColor; }
    public float FireRate { get => _fireRate; }
    public float VertOffset { get => _verticalOffset; }
    public float HorOffset { get => _horizontalOffset; }


    [Header("Set delay if Multicolor")]
    [SerializeField]
    private float _delayIfMulticolor;

    public float DelayIfMultiColor { get => _delayIfMulticolor; }

    [Header("Space Bounds")]
    [SerializeField]
    private float _yLimit;
    [SerializeField]
    private float _xLimit;

    public float YBound { get => _yLimit; }
    public float XBound { get => _xLimit; }

    private SpriteRenderer _sprite;
    private WaitForSeconds _delay;

    public virtual void Start()
    {
        SetOffsetPos();

        if (_isMultiColor)
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

    public virtual void Update()
    {
        DestroyIfOutOfBounds();
    }

    ////////////////////////////////
    //POSITION/////////////////////
    ////////////////////////////////

    public virtual void SetOffsetPos()
    {
        //Get the respective offsets to set the origin of the shot relative to the spaceship
        Vector3 vertFix = transform.up * _verticalOffset;
        Vector3 horiFix = transform.right * _horizontalOffset;

        //Random sign is used in here as the enemy have 2 possible cannons to shoot (horizontally)
        if (_isEnemyShot)
            horiFix *= Random.value < 0.5f ? 1 : -1;

        Vector3 newPos = transform.position + vertFix + horiFix;

        transform.position = newPos;
    }


    ////////////////////////////////
    //ANIMATION/////////////////////
    ////////////////////////////////

    private IEnumerator SetColor()
    {
        while (true)
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
        if (_isEnemyShot && other.CompareTag("Player"))
        {
            DamagePlayer(other);
            Destroy(gameObject);
        }
    }

    protected void DamagePlayer(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (!(player is null))
        {
            int quadrant = player.GetDamageDirection(transform.position);
            player.GetDamage(quadrant);
        }
    }

    ////////////////////////////////
    //ONDESTROY/////////////////////
    ////////////////////////////////

    void DestroyParent()
    {
        if (!(transform.parent is null))
        {
            if (transform.parent.childCount <= 1)
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
        if (transform.position.y > _yLimit || transform.position.y < -_yLimit
            || transform.position.x > _xLimit || transform.position.x < -_xLimit)
        {
            DestroyShot();
        }
    }
}
