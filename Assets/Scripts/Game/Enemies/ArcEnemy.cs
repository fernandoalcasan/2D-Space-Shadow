using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcEnemy : Enemy
{
    [SerializeField]
    private float _rotationSpeed;
    [SerializeField] [Range(0f, 360f)]
    private float _rotationAngle;

    private Vector3 _transDir;
    private Vector3 _transRot;

    public override void Start()
    {
        base.Start();
        //choose the side of appearance (left, down, right or up)
        SetNewSide(Random.Range(1,5));
        SetNewRotAndDir();
    }

    public override void Update()
    {
        MoveEnemy();
    }

    protected override void MoveEnemy()
    {
        base.MoveEnemy();
        transform.Translate(_transDir * Speed * Time.deltaTime);
        transform.Rotate(_transRot, _rotationAngle * _rotationSpeed * Time.deltaTime);
    }

    protected void SetNewRotAndDir()
    {
        int dir = Random.Range(0, 4);
        int rot = Random.Range(0, 2);

        switch(dir)
        {
            case 0:
                _transDir = Vector3.down;
                break;
            case 1:
                _transDir = Vector3.up;
                break;
            case 2:
                _transDir = Vector3.right;
                break;
            case 3:
                _transDir = Vector3.left;
                break;
        }

        if (rot == 0)
            _transRot = Vector3.forward;
        else
            _transRot = Vector3.back;
    }
}
