using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : Projectile
{
    [SerializeField] private float _pullForce;

    private bool _hooked = false;
    private bool _dualPull = false; // if true, both hooked entity & self are pulled towards each other. If self, only self is pulled towards hooked entity. Think Mistborn iron allomancy 
    public GameObject originator;
    private GameObject _hookedEntity;

    private float _disengageDistance = 1.5f;
    private Vector2 previousPullToTargetAdditive;
    private Vector2 previousPullFromTargetAdditive;


    protected override void Start()
    {

        starting_mouseclick_position.z = 0;
        transform.right = starting_mouseclick_position - transform.position;

    }

    private void FixedUpdate()
    {
        _lifespan -= Time.deltaTime;


        if (_hooked == false)
        {
            _rigidbody.AddForce(transform.right * _movement_speed + new Vector3(velocity_additive.x, velocity_additive.y, 0));
        }

        else if (originator != null) // applies pulling force
        {
            Vector2 lineToHooked = transform.position - originator.transform.position;
            lineToHooked = lineToHooked.normalized * _pullForce;
            originator.transform.GetComponent<LivingEntity>().velocity_additive += lineToHooked - previousPullToTargetAdditive;
            if (_dualPull)
            {
                Vector2 lineToOriginator = originator.transform.position - transform.position;
                lineToOriginator = lineToOriginator.normalized * _pullForce;
                _hookedEntity.transform.GetComponent<LivingEntity>().velocity_additive += lineToOriginator - previousPullFromTargetAdditive;
                previousPullFromTargetAdditive = lineToOriginator;
            }

            previousPullToTargetAdditive = lineToHooked;


            if (Vector3.Distance(transform.position, originator.transform.position) < _disengageDistance)
            {
                Die();
            }

            if (_lifespan <= 0)
            {
                Die();
            }

        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.gameObject != originator) // can't hook yourself
        {
            _hookedEntity = collision.transform.gameObject;
            transform.SetParent(collision.transform);
            _rigidbody.simulated = false;
            _hooked = true;

            if (collision.transform.GetComponent<Boss>() == null && collision.transform.GetComponent<LivingEntity>() != null) // can't pull boss or wall towards self
            {
                _dualPull = true;
            }


        }
        else
        {
            Die();
        }


    }


    private void Die()
    {
        Destroy(gameObject);
        if (originator != null)
        {
            originator.transform.GetComponent<LivingEntity>().velocity_additive -= previousPullToTargetAdditive;
        }
        if (_dualPull && _hookedEntity != null)
        {
            _hookedEntity.transform.GetComponent<LivingEntity>().velocity_additive -= previousPullFromTargetAdditive;
        }
    }

}
