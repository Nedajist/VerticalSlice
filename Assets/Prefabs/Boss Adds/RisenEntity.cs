using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisenEntity : Enemy
{
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] protected float _targeting_cooldown;
    [SerializeField] protected float _ability_cooldown;
    [SerializeField] protected Vector3 _starting_position = Vector3.zero;
    [SerializeField] float _degrees_per_second; // as pertaining to movement rotation. Might change during later phases. 
    [SerializeField] float _upper_degree_bound; // also determines lower bound
    [SerializeField] GameObject _target;

    protected float _ability_timer;
    protected float _targeting_timer;
    private float degree_change = 0; // how much the entity changes degrees every fixedupdate 

    public PlayerClass risen_class;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Projectile"))
        {
            if (collision.transform.CompareTag("Projectile"))
            {
                ReceiveDamage(collision.transform.GetComponent<Projectile>().damage);
            }

        }
    }

    protected void MoveToTarget()
    {
        Vector2 _line_to_target =  (Vector2) _target_player.transform.position - (Vector2) transform.position;
        _rb.velocity = (_line_to_target + _velocity_additive) * _velocity_multiplicative;

        degree_change += _degrees_per_second * Time.fixedDeltaTime;

        if (degree_change > _upper_degree_bound || degree_change < -_upper_degree_bound)
        {
            _degrees_per_second = -_degrees_per_second;
        }

        _rb.velocity = GameController.instance.RotateVector2(_rb.velocity, degree_change);

    }







}
    


