using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class EnemyRange : MonoBehaviour
{
    [Header("Features")]
    [SerializeField]
    private bool _follow;
    [SerializeField]
    private bool _avoid;

    [Header("Features to follow")]
    [SerializeField]
    private float _degreesPerSecond;
    private Coroutine _currentCoroutine;
    private bool _rotating;

    [Header("Features to avoid")]
    [SerializeField]
    private float _avoidingRange;
    [SerializeField]
    private float _avoidSpeed;
    private bool _avoiding;
    private float _avoidFor;

    ////////////////////////////////
    //COROUTINES////////////////////
    ////////////////////////////////

    private IEnumerator MoveTowards(Vector3 dir)
    {
        _avoiding = true;
        _avoidFor = .2f;

        while (_avoidFor > 0f)
        {
            Vector3 enemyPos = transform.parent.position;
            transform.parent.position = Vector3.MoveTowards(enemyPos, enemyPos + dir, _avoidSpeed * Time.deltaTime);
            _avoidFor -= Time.deltaTime;
            yield return null;
        }

        _avoiding = false;
    }

    private IEnumerator RotateTowards(Quaternion objective)
    {
        _rotating = true;
        while (transform.parent.rotation != objective)
        {
            transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, objective, _degreesPerSecond * Time.deltaTime);
            yield return null;
        }
        _rotating = false;
    }

    ////////////////////////////////
    //ON TRIGGERS///////////////////
    ////////////////////////////////

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_follow && other.CompareTag("Player"))
        {
            Vector3 dir = other.transform.position - transform.parent.position;
            float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) + 90f;

            _currentCoroutine = StartCoroutine(RotateTowards(Quaternion.Euler(0, 0, angle)));
        }
        else if(_avoid && other.CompareTag("PlayerShot"))
        {
            //if enemy is avoiding another shot already
            if (_avoiding)
                return;

            Vector3 enemyDirFromShot = transform.parent.position - other.transform.position;
            Vector3 rightPerpDir = Vector3.Cross(enemyDirFromShot, Vector3.forward).normalized;

            //Choose to move left or right
            Vector3 perpDir = Random.value < 0.5f ? -rightPerpDir : rightPerpDir;

            StartCoroutine(MoveTowards(perpDir));
        }
    }

    private void StopRotationIfActive()
    {
        if(_rotating)
        {
            StopCoroutine(_currentCoroutine);
            _rotating = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            StopRotationIfActive();
        }
    }
}
