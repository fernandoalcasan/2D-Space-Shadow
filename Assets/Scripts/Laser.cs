using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    //speed of the laser
    [SerializeField]
    private float _speed = 8f;

    //Bounds for the laser}
    private float _yLimit = 7f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Laser goes up
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        // if the laser goues off the limits gets destroyed
        if(transform.position.y > _yLimit)
        {
            DestroyParent();
            Destroy(gameObject);
        }
    }

    void DestroyParent()
    {
        if(!(transform.parent is null))
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
