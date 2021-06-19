using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardEnemy : Enemy
{
    [Header("Forward Enemy Properties")]
    [SerializeField]
    private bool _isSmart;
    [SerializeField]
    private float _delayIfSmart;
    private WaitForSeconds _smartDelay;
    [SerializeField]
    private GameObject _smartShot;

    public override void Start()
    {
        base.Start();
        SetNewTransform();

        if (_isSmart)
        {
            _smartDelay = new WaitForSeconds(_delayIfSmart);
            StartCoroutine(CheckIfPlayerIsBehind());
        }
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

    ////////////////////////////////
    //SHOOTING//////////////////////
    ////////////////////////////////

    private IEnumerator CheckIfPlayerIsBehind()
    {
        float range = 26f;

        while (!IsDead)
        {
            yield return _smartDelay;

            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.up, range);

            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    //delay the default shot (front)
                    canShoot = Time.time + FireDelay;
                    anim.SetTrigger("SmartShot");
                }
            }
        }
    }

    private void ShootBehind()
    {
        Quaternion inverseRot = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - 180f);
        Instantiate(_smartShot, transform.position, inverseRot);

        //Shot AUDIO
        PlayAudio(1);
    }
}
