using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisenEntity : Enemy
{
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] protected float _targeting_cooldown;
    [SerializeField] protected float _ability_cooldown;
    [SerializeField] protected Vector3 _starting_position = Vector3.zero;
    [SerializeField] GameObject _target;

    protected float _ability_timer;
    protected float _targeting_timer;

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
    }
}
    


