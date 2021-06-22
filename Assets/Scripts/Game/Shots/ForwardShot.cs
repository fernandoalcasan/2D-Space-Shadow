using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardShot : Shot
{

    public override void Update()
    {
        base.Update();
        if (IsEnemyShot)
        {
            transform.Translate(Speed * Time.deltaTime * Vector3.down);
        }
        else
        {
            transform.Translate(Speed * Time.deltaTime * Vector3.up);
        }
    }
}
