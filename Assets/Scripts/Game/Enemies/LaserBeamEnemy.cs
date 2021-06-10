using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamEnemy : ArcEnemy
{
    [SerializeField]
    private float _waitToShoot;
    private WaitForSeconds _delay;
    private WaitForSeconds _shotDuration;
    private bool _isShooting;

    public override void Start()
    {
        base.Start();
        _delay = new WaitForSeconds(_waitToShoot);

        if (shot.TryGetComponent<LaserBeamShot>(out var laserBeam))
            _shotDuration = new WaitForSeconds(laserBeam.Duration);
        else
            Debug.LogError("Laser beam shot is NULL");

        StartCoroutine(ShootingSequence());
    }

    protected override void MoveEnemy()
    {
        if (!_isShooting)
        {
            base.MoveEnemy();
        }
    }

    private IEnumerator ShootingSequence()
    {
        yield return _delay;

        _isShooting = true;
        anim.SetTrigger("Shoot");
    }

    private IEnumerator StopShootingSequence()
    {
        yield return _shotDuration;

        anim.speed = 1;
        anim.SetTrigger("StopShooting");
        SetNewRotAndDir();
        _isShooting = false;
        StartCoroutine(ShootingSequence());
    }

    //Method called from animation clip to shoot
    private void ShootFromSide(int side)
    {
        bool shoot = Random.value > 0.33f ? true : false;

        if (side != 2 && !shoot)
            return;

        _isShooting = true;
        anim.speed = 0;

        Vector3 beamPos = transform.position + (transform.up * .15f);
        Quaternion rightRot = Quaternion.Euler(new Vector3(0, 0, transform.eulerAngles.z - 90f));
        Quaternion leftRot = Quaternion.Euler(new Vector3(0, 0, transform.eulerAngles.z + 90f));

        switch (side)
        {
            case 0: //right side
                Instantiate(shot, beamPos, rightRot);
                break;
            case 1: //left side
                Instantiate(shot, beamPos, leftRot);
                break;
            default: //both sides
                Instantiate(shot, beamPos, rightRot);
                Instantiate(shot, beamPos, leftRot);
                break;
        }
        PlayAudio(1);

        StartCoroutine(StopShootingSequence());
    }
}
