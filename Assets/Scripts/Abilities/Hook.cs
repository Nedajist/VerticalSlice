using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HookMode
{
    push,
    pull
}



public class Hook : Projectile
{
    [SerializeField] private float _selfPushPullForce;
    [SerializeField] private float _otherPushPullForce;

    [SerializeField] private HookMode _hookMode;
    private bool _hooked = false;
    private bool _dualPull = false; // if true, both hooked entity & self are pulled towards each other. If false, only self is pulled towards hooked entity. Think Mistborn iron 
    private bool _dualPush = false; // if true, both hooked entity and self are pulled AWAY from each other. If false, only self is pushed away from entity 
    public GameObject originator;
    private GameObject _hookedEntity;

    private float _disengageDistance = 1.5f;

    protected override void Start()
    {
        LivingEntity originatingEntity = originator.GetComponent<LivingEntity>();
        if (originatingEntity.velocity_additive_dict.ContainsKey(VelocityAdditiveType.hook) == false){
            originatingEntity.velocity_additive_dict[VelocityAdditiveType.hook] = GetHookVelocityAdditive();
        }

        starting_mouseclick_position.z = 0;
        transform.right = starting_mouseclick_position - transform.position;

    }

    private void FixedUpdate()
    {
        _lifespan -= Time.deltaTime;


        if (_hooked == false) // normal projectile movement 
        {
            _rigidbody.AddForce(transform.right * _movement_speed);
            AddVelocityAdditives();
            if (_rigidbody.velocity.magnitude < _min_movement_speed)
            {
                _rigidbody.velocity = _rigidbody.velocity.normalized;
                _rigidbody.velocity *= _min_movement_speed;
            }
        }

        else if (originator != null && _hooked == true) // 
        {
            Vector2 lineToHooked = transform.position - originator.transform.position;
            lineToHooked = lineToHooked.normalized * _selfPushPullForce;

            if (_hookMode == HookMode.push)
            {
                lineToHooked = lineToHooked * -1; // pushes if pushing 
            }

            originator.transform.GetComponent<LivingEntity>().velocity_additive_dict[VelocityAdditiveType.hook].additive_total += lineToHooked;
            if (_dualPull)
            {
                Vector2 lineToOriginator = originator.transform.position - transform.position;
                lineToOriginator = lineToOriginator.normalized * _otherPushPullForce;
                _hookedEntity.transform.GetComponent<LivingEntity>().velocity_additive_dict[VelocityAdditiveType.hook].additive_total += lineToOriginator;
            }

            else if (_dualPush)
            {
                Vector2 lineToTarget = transform.position - originator.transform.position;
                lineToTarget = lineToTarget.normalized * _otherPushPullForce;
                _hookedEntity.transform.GetComponent<LivingEntity>().velocity_additive_dict[VelocityAdditiveType.hook].additive_total += lineToTarget;
            }



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


            LivingEntity _hookedLivingEntity = collision.transform.GetComponent<LivingEntity>();
            if (collision.transform.GetComponent<Boss>() == null && _hookedLivingEntity != null) // can't pull/push boss or wall towards self
            {

                _hookedLivingEntity.velocity_additive_dict[VelocityAdditiveType.hook] = GetHookVelocityAdditive();

                if (_hookMode == HookMode.pull) // pull mode
                {
                    _dualPull = true;
                }
                else // push mode
                {
                    _dualPush = true;
                }
            }


        }
        else
        {
            Die(); // dies if hooks originator  
        }


    }


    private void Die()
    {
        Destroy(gameObject);
    }

    private VelocityAdditive GetHookVelocityAdditive()
    {
        VelocityAdditive hook_velocity_additive = new VelocityAdditive();
        hook_velocity_additive.additive_max_magnitude = _selfPushPullForce;
        return hook_velocity_additive;
    }

    private void OnDestroy()
    {
        if (originator != null)
        {
            originator.transform.GetComponent<LivingEntity>().velocity_additive_dict[VelocityAdditiveType.hook].additive_total = Vector2.zero;
        }
        if ((_dualPull || _dualPush) && _hookedEntity != null)
        {
            _hookedEntity.transform.GetComponent<LivingEntity>().velocity_additive_dict[VelocityAdditiveType.hook].additive_total = Vector2.zero;
        }
    }

}
