using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Boss : Enemy
{

    [SerializeField] Rigidbody2D _rb;
    [SerializeField] public Vector2 _velocity_additive;
    [SerializeField] public Vector2 _dash_additive;
    [SerializeField] GameObject _spiral_projectile_bundle;
    [SerializeField] GameObject _gatling_projectile_bundle;
    [SerializeField] GameObject _homing_projectile_bundle;

    [SerializeField] Vector3 starting_position;

    private float _target_x;
    private float _target_y;
    private float _distance_to_target_x;
    private float _distance_to_target_y;
    private float _vertical_movement_additive;
    private float _horizontal_movement_additive;

    private float _projectile_timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        _projectile_timer -= Time.deltaTime;
        if (_projectile_timer < 0)
        {
            GameObject instantiated_spiral_bundle = Instantiate(_homing_projectile_bundle, transform.position, Quaternion.identity);
            instantiated_spiral_bundle.transform.SetParent(transform);
            _projectile_timer = 3;
        }
    }

    private void FixedUpdate()
    {
        TargetNearestPlayer();
        MoveTowardsTargetPlayer();
    }

    private void MoveTowardsTargetPlayer()
    {
        _target_x = _target_player.transform.position.x;
        _target_y = _target_player.transform.position.y;
        Move();
    }

    private void Move()
    {

        _distance_to_target_x = Mathf.Abs(_target_x - transform.position.x);
        _distance_to_target_y = Mathf.Abs(_target_y - transform.position.y);

        if (_target_y > transform.position.y) // movement multiplier is +/- if _target_y is above/below the current position 
        {
            _vertical_movement_additive = _distance_to_target_y / (_distance_to_target_x + _distance_to_target_y) * _speed;
        }
        else
        {
            _vertical_movement_additive = -1f * _distance_to_target_y / (_distance_to_target_x + _distance_to_target_y) * _speed;
        }

        if (_target_x > transform.position.x) // movement multiplier is +/- if _target_x is to the right/left of the current position 
        {
            _horizontal_movement_additive = _distance_to_target_x / (_distance_to_target_x + _distance_to_target_y) * _speed;
        }
        else
        {
            _horizontal_movement_additive = -1f * _distance_to_target_x / (_distance_to_target_x + _distance_to_target_y) * _speed;
        }

        _rb.velocity = (new Vector2(_horizontal_movement_additive, _vertical_movement_additive) + _velocity_additive);


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Projectile"))
        {
            _health -= (collision.transform.GetComponent<Projectile>().damage);
        }
    }

    public void ResetSelf()
    {
        transform.position = starting_position;
        transform.rotation = Quaternion.identity;
        _projectile_timer = 0f;
        _health = _max_health;


    }

}
