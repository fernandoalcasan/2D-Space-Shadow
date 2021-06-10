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

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("PlayGround"))
            {
                if (hit.distance <= 0f)
                    break;
                _line.SetPosition(0, transform.position);
                _line.SetPosition(1, transform.position + (transform.up * hit.distance));
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
