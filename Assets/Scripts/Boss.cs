using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Boss : Enemy
{

    [SerializeField] Rigidbody2D _rb;
    [SerializeField] public Vector2 _dash_additive;
    [SerializeField] GameObject _spiral_projectile_bundle;
    [SerializeField] GameObject _gatling_projectile_bundle;
    [SerializeField] GameObject _homing_projectile_bundle;
    [SerializeField] GameObject _boss_disease_AOE;
    [SerializeField] float _degrees_per_second; // as pertaining to movement rotation. Might change during later phases. 
    [SerializeField] float _upper_degree_bound; // also determines lower bound

    [SerializeField] Vector3 starting_position;

    private float _target_x;
    private float _target_y;
    private float _distance_to_target_x;
    private float _distance_to_target_y;
    private float _vertical_movement_additive;
    private float _horizontal_movement_additive;

    private float _projectile_timer = 0;
    private float _charge_timer = 0;

    private float degree_change = 0; // how much the rat changes degrees every fixedupdate 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _charge_timer -= Time.deltaTime;
        _projectile_timer -= Time.deltaTime;
        if (_projectile_timer < 0)
        {
            GameObject instantiated_spiral_bundle = Instantiate(_spiral_projectile_bundle, transform.position, Quaternion.identity);
            instantiated_spiral_bundle.transform.SetParent(transform);
            _projectile_timer = 3; // temp value
            
        }

        if (_charge_timer < 0)
        {
            StartCoroutine(MassAOE(0.1f));
            _charge_timer = 3; // temp value
        }

    }

    private void FixedUpdate()
    {
        TargetNearestPlayer();
        SlitherTowardsTargetPlayer();
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
    }

    private void Move() // sets rigidbody velocity DIRECTLY towards target x and y 
    {

        Vector2 _line_to_target = new Vector2(_target_x, _target_y) - (Vector2)transform.position;
        _rb.velocity = (_line_to_target + _velocity_additive) * _velocity_multiplicative;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Projectile"))
        {
            ReceiveDamage(collision.transform.GetComponent<Projectile>().damage);
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

    IEnumerator ChargeToTarget(float duration) // longer the charge, the more the boss accelerates
    {
        float _time = 0;
        while (_time < duration)
        {
            _time += Time.fixedDeltaTime;
            _velocity_multiplicative = 0.5f * Mathf.Pow(6, _time / 3.3f) ;
            yield return new WaitForFixedUpdate();
        }

        _time = 0;
        while (_time < 0.5f) // 0.5 is the deceleration period 
        {
            
            _time += Time.fixedDeltaTime;
            _velocity_multiplicative = Mathf.Lerp(_velocity_multiplicative, 1, 2f * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
        _velocity_multiplicative = 1;
        yield return null;
    }

    IEnumerator MassAOE(float interval_between_attacks)
    {
        float duration = interval_between_attacks;

        GameController.instance.RefreshAllyList();
        for (int i = 0; i < GameController.instance.ally_list.Count; i++)
        {
            if (i < GameController.instance.ally_list.Count == false) 
            {
                break;
            }

            duration -= Time.fixedDeltaTime;
            if (duration > 0)
            {
                yield return new WaitForFixedUpdate();
            }

            if (GameController.instance.ally_list[i] != null)
            {
                duration = interval_between_attacks;
                GameObject _instantiated_AOE_circle = Instantiate(_boss_disease_AOE, GameController.instance.ally_list[i].transform.position, Quaternion.identity);
                yield return new WaitForFixedUpdate();
            }
        }

        yield return null;

    }



}
