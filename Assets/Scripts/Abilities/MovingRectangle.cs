using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingRectangle : Rectangle
{
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] float _movement_speed;
    [SerializeField] float _max_speed;
    [SerializeField] float _min_speed;

    [HideInInspector] public Vector3 starting_mouseclick_position;


    protected override void Start()
    {
        base.Start();
        starting_mouseclick_position.z = 0;
        transform.right = starting_mouseclick_position - transform.position;


    }


    private void FixedUpdate()
    {
        _damage_timer -= Time.fixedDeltaTime;
        _rb.AddForce(_movement_speed * transform.right);

        if (_rb.velocity.magnitude < _min_speed)
        {
            _rb.velocity = _min_speed * transform.right;
        }

        else if (_rb.velocity.magnitude > _max_speed)
        {
            _rb.velocity = _max_speed * transform.right;
        }

    }


}
