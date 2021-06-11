using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamShot : Shot
{
    [Header("Time in Screen")]
    [SerializeField]
    private float _duration;
    public float Duration { get => _duration; }

    private float _range;
    private LineRenderer _line;

    public override void Start()
    {
        base.Start();
        _range = XBound * 2f;
        _line = GetComponent<LineRenderer>();

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.up, _range);

        //last collider will always be the playground, which is laser beam length -1
        if (hits[hits.Length-1].distance > 0f)
        {
            _line.SetPosition(0, transform.position);
            _line.SetPosition(1, transform.position + (transform.up * hits[hits.Length-1].distance));
        }

        //check for damage between first collider and playground
        foreach (var hit in hits)
        {
            if(hit.collider.CompareTag("PlayerShield"))
            {
                if (hit.transform.TryGetComponent<Shield>(out var shield))
                    shield.DamageShield();
                else
                    Debug.LogError("Shield reference is NULL");
                //if shield was detected break the loop
                break;
            }

            if (hit.collider.CompareTag("Player"))
            {
                DamagePlayer(hit.collider);
            }
        }

        Destroy(gameObject, _duration);
    }

    public override void SetOffsetPos()
    {
        Vector3 vertFix = transform.up * VertOffset;
        Vector3 horiFix = transform.right * HorOffset;
        transform.position += vertFix + horiFix;
    }
}
