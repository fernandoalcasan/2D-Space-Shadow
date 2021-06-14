using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField]
    private bool _isEnemyShield;
    [SerializeField]
    private int _resistance;
    private int _maxResistance;
    private SpriteRenderer _shieldRenderer;
    private Color _normal, _damage1, _damage2;

    // Start is called before the first frame update
    void Start()
    {
        _maxResistance = _resistance;
        _shieldRenderer = GetComponent<SpriteRenderer>();
        _normal = _shieldRenderer.color;
        _damage1 = new Color32(255, 191, 255, 195);
        _damage2 = new Color32(234, 48, 123, 195);

        if(_shieldRenderer == null)
        {
            Debug.LogError("Shield renderer is NULL");
        }

        SetShieldColor();
    }

    public void DamageShield()
    {
        _resistance--;
        SetShieldColor();

        if(_resistance == 0)
        {
            _resistance = _maxResistance;
            gameObject.SetActive(false);
        }
    }

    void SetShieldColor()
    {
        switch (_resistance)
        {
            case 1:
                _shieldRenderer.color = _damage2;
                break;
            case 2:
                _shieldRenderer.color = _damage1;
                break;
            default: //shield broken or normal
                _shieldRenderer.color = _normal;
                break;
        }
    }

    public void RestoreShield()
    {
        _resistance = _maxResistance;
        SetShieldColor();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isEnemyShield)
        {
            if (other.CompareTag("EnemyShield"))
            {
                DamageShield();
            }
            else if (other.CompareTag("EnemyShot"))
            {
                Destroy(other.gameObject);
                DamageShield();
            }
            else if (other.CompareTag("Enemy"))
            {
                if (other.TryGetComponent<EnemyBehavior>(out var enemy))
                    enemy.DeathSequence();
                else
                    Debug.LogError("Enemy reference is NULL");
                DamageShield();
            }
        }
        else
        {
            if (other.CompareTag("PlayerShield"))
            {
                DamageShield();
            }
            else if (other.CompareTag("PlayerShot"))
            {
                Destroy(other.gameObject);
                DamageShield();
            }
            else if (other.CompareTag("Player"))
            {
                if (other.TryGetComponent<Player>(out var player))
                {
                    int dir = player.GetDamageDirection(transform.position);
                    player.GetDamage(dir);
                }
                else
                    Debug.LogError("Player reference is NULL");

                DamageShield();
            }
        }

    }
}
