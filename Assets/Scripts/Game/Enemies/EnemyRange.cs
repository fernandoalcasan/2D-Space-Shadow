using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class EnemyRange : MonoBehaviour
{
    [SerializeField]
    private float _degreesPerSecond;
    private Coroutine _currentCoroutine;
    private bool _rotating;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Vector3 dir = other.transform.position - transform.parent.position;
            float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) + 90f;

            _currentCoroutine = StartCoroutine(RotateTowards(Quaternion.Euler(0, 0, angle)));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(_rotating)
            {
                StopCoroutine(_currentCoroutine);
                _rotating = false;
            }
        }
    }
}
