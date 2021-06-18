using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardEnemy : Enemy
{
    public override void Start()
    {
        base.Start();
        SetNewTransform();
    }

    public override void Update()
    {
        base.Update();
        MoveEnemy();
    }

    ////////////////////////////////
    //TRANSFORM PROPERTIES//////////
    ////////////////////////////////

    protected override void SetNewTransform()
    {
        //choose the side of appearance (left, down, right or up)
        int side = Random.Range(1, 5);
        SetNewSide(side);
        transform.eulerAngles = new Vector3(0, 0, GetRandomAngle(side));
    }

    private float GetRandomAngle(int side)
    {
        float angle = (float)(90f * side) + (Random.Range(0f, 45f) * (Random.value > .5f ? -1f : 1f));
        return angle;
    }

    ////////////////////////////////
    //MOVEMENT//////////////////////
    ////////////////////////////////

    protected override void MoveEnemy()
    {
        base.MoveEnemy();
        transform.Translate(Vector3.down * Speed * Time.deltaTime);
    }
}
