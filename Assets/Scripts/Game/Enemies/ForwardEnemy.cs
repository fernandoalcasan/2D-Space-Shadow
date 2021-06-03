using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardEnemy : EnemyBehavior
{
    private float _xBound;
    private float _yBound;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        _xBound = XBound;
        _yBound = YBound;
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

    private void SetNewTransform()
    {
        //choose the side of appearance (left, down, right or up)
        int side = Random.Range(1, 5);
        
        switch (side)
        {
            case 1: //left
                transform.position = new Vector3(-_xBound, GetRandomY(), 0);
                break;
            case 2: //down
                transform.position = new Vector3(GetRandomX(), -_yBound, 0);
                break;
            case 3: //right
                transform.position = new Vector3(_xBound, GetRandomY(), 0);
                break;
            case 4: //up
                transform.position = new Vector3(GetRandomX(), _yBound, 0);
                break;
        }

        transform.eulerAngles = new Vector3(0, 0, GetRandomAngle(side));
    }

    private float GetRandomX()
    {
        return Random.Range(-_xBound, _xBound);
    }

    private float GetRandomY()
    {
        return Random.Range(-_yBound, _yBound);
    }

    private float GetRandomAngle(int side)
    {
        float angle = (float)(90f * side) + (Random.Range(0f, 45f) * (Random.value > .5f ? -1f : 1f));
        return angle;
    }

    ////////////////////////////////
    //MOVEMENT//////////////////////
    ////////////////////////////////

    private void MoveEnemy()
    {
        transform.Translate(Vector3.down * Speed * Time.deltaTime);
        TeleportIfOutOfBounds();
    }

    private void TeleportIfOutOfBounds()
    {
        if (transform.position.y > _yBound || transform.position.y < -_yBound
            || transform.position.x > _xBound || transform.position.x < -_xBound)
        {
            SetNewTransform();
        }
    }
}
