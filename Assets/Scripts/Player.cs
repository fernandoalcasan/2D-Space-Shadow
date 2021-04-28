using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Speed of the player
    private float _speed = 5f;

    //bounds of field
    private float _xLimit = 10f;
    private float _yLimit = 5.5f;

    //prefab for laser
    [SerializeField]
    private GameObject _laser;

    // Start is called before the first frame update
    void Start()
    {
        // Set the player position = 0
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        LimitSpace();

        //Instantiate laser prefab with spacebar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(_laser, transform.position + new Vector3(0,0.8f,0), Quaternion.identity);
        }
    }

    void MovePlayer()
    {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");
        // get direction of movement
        Vector3 dir = new Vector3(hInput, vInput, 0);

        // move player
        transform.Translate(dir * _speed * Time.deltaTime);
    }

    void LimitSpace()
    {
        // Teleport on limits
        // If X limit is reached appear in the opposite side
        if (transform.position.x > _xLimit | transform.position.x < -_xLimit)
        {
            transform.position = new Vector3(_xLimit * (transform.position.x > _xLimit ? -1 : 1), transform.position.y, 0);
        }

        // If Y limit is reached appear in the opposite side
        if (transform.position.y > _yLimit | transform.position.y < -_yLimit)
        {
            transform.position = new Vector3(transform.position.x, _yLimit * (transform.position.y > _yLimit ? -1 : 1), 0);
        }

        // Stop on limits
        //transform.position = new Vector3(Mathf.Clamp(transform.position.x, -_xLimit, _xLimit), Mathf.Clamp(transform.position.y, -_yLimit, _yLimit), 0);
    }
}
