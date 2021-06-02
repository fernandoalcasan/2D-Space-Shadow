using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField]
    private int _resistance = 3;
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
    }

    public bool DamageShield()
    {
        _resistance--;
        SetShieldColor();

        //return if shield stills active
        switch(_resistance)
        {
            case 1: case 2:
                return true;
            default:
                _resistance = _maxResistance;
                return false;
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
}
