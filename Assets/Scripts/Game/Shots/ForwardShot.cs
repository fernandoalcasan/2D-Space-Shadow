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
            transform.Translate(Vector3.down * Speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.up * Speed * Time.deltaTime);
        }
    }
}
