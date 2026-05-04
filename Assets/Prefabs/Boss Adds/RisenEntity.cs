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
    [SerializeField] float _minimum_distance_from_characters = 3;

    protected float _ability_timer;
    protected float _targeting_timer;
    private float _degree_change = 0; // how much the entity changes degrees every fixedupdate 
    private float _repulsion_factor = 2;
    private Vector2 _previous_avoidance_velocity = Vector2.zero;

    public PlayerClass risen_class;

    protected override void Start()
    {
        SetColor();
    }

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
        _line_to_target = _line_to_target.normalized;
        _rb.velocity = (_line_to_target * _speed);

        _degree_change += _degrees_per_second * Time.fixedDeltaTime;

        if (_degree_change > _upper_degree_bound || _degree_change < -_upper_degree_bound)
        {
            _degrees_per_second = -_degrees_per_second;
        }

        AvoidanceCheck();
        _rb.velocity = GameController.instance.RotateVector2(_rb.velocity, _degree_change) + velocity_additive;

    }


    private void AvoidanceCheck()
    {
        Vector2 linetoself = Vector2.zero;
        RaycastHit2D[] collider_list = Physics2D.CircleCastAll(transform.position, _minimum_distance_from_characters, Vector2.zero);
        foreach (RaycastHit2D hit in collider_list)
        {
            if (hit.transform.gameObject != _target && (hit.transform.GetComponent<Enemy>() != null || hit.transform.GetComponent<Ally>() != null))
            {
                linetoself = transform.position - hit.transform.position;
                linetoself = linetoself.normalized;
                linetoself = linetoself * _repulsion_factor;
            }

        }

        
        linetoself = linetoself.normalized;
        linetoself *= _repulsion_factor;
        _previous_avoidance_velocity = linetoself;
        velocity_additive += linetoself - _previous_avoidance_velocity;
            
    }
    }




    


