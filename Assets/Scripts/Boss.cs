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
    [SerializeField] float _degrees_per_second; // as pertaining to movement rotation
    [SerializeField] float _upper_degree_bound; // also determines lower bound

    [SerializeField] Vector3 starting_position;

    private float _target_x;
    private float _target_y;
    private float _distance_to_target_x;
    private float _distance_to_target_y;
    private float _vertical_movement_additive;
    private float _horizontal_movement_additive;

    private float _projectile_timer = 0;


    private float degree_change = 0; // how much the rat changes degrees every fixedupdate 

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


    private void MoveTowardsTargetPlayer() // beelines player. Best for melee.
    {
        _target_x = _target_player.transform.position.x;
        _target_y = _target_player.transform.position.y;
        Move();
    }

    private void SlitherTowardsTargetPlayer() // Kinda rotates around player but also zigzaggy. Best for ranged. 
    {
        _target_x = _target_player.transform.position.x;
        _target_y = _target_player.transform.position.y;
        Move();
        degree_change += _degrees_per_second * Time.deltaTime;

        if (degree_change > _upper_degree_bound || degree_change < -_upper_degree_bound)
        {
            _degrees_per_second = -_degrees_per_second;
        }

        _rb.velocity = RotateVector2(_rb.velocity, degree_change);
        Debug.Log(degree_change);
    }

    private void Move() // sets rigidbody velocity DIRECTLY towards target x and y 
    {

        Vector2 _line_to_target = new Vector2(_target_x, _target_y) - (Vector2)transform.position;
        _rb.velocity = (_line_to_target + _velocity_additive);
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
    private Vector2 RotateVector2(Vector2 inputVector2, float degrees)
    {
        float rotationRadians = degrees * Mathf.Deg2Rad;
        Vector2 newVector2 = Vector2.zero;
        newVector2.x = inputVector2.x * Mathf.Cos(rotationRadians) + inputVector2.y * Mathf.Sin(rotationRadians);
        newVector2.y = -inputVector2.x * Mathf.Sin(rotationRadians) + inputVector2.y * Mathf.Cos(rotationRadians);

        return (newVector2);
    }
}
