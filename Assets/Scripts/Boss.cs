using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] GameObject _normal_sword_slash;
    [SerializeField] GameObject _wide_sword_slash;


    [SerializeField] float _degrees_per_second; // as pertaining to movement rotation. Might change during later phases. 
    [SerializeField] float _upper_degree_bound; // also determines lower bound

    [SerializeField] Vector3 starting_position;

    private float _target_x;
    private float _target_y;

    private float degree_change = 0; // how much the rat changes degrees every fixedupdate 

    // Start is called before the first frame update

    private void FixedUpdate()
    {
        
    }

    public override void ReceiveDamage(float amount)
    {
        if (_i_frames > 0)
        {
            return;
        }
        _i_frames = _iframe_duration;

        _health -= amount;
        StartCoroutine(FlashColor(0.1f, 0.1f, Color.red));
        ShakeHealthBar();
        if (_health <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected override void Start()
    {
        base.Start();
        _iframe_duration = -1; // boss doesn't have iframes
    }

    public void SummonSpiralProjectiles()
    {
        GameObject instantiated_spiral_bundle = Instantiate(_spiral_projectile_bundle, transform.position, Quaternion.identity);
        instantiated_spiral_bundle.transform.SetParent(transform);
    }

    public void SummonHomingProjectiles()
    {
        GameObject instantiated_homing_bundle = Instantiate(_homing_projectile_bundle, transform.position, Quaternion.identity);
        instantiated_homing_bundle.transform.SetParent(transform);
    }

    public void SummonInfectiousProjectiles()
    {
        GameController.instance.SummonInfectiousProjectileBundle(transform.position);
    }

    public void SummonGatlingProjectiles()
    {
        GameObject instantiated_gatling_bundle = Instantiate(_gatling_projectile_bundle, transform.position, Quaternion.identity);
        instantiated_gatling_bundle.transform.SetParent(transform);
    }

    public void SummonNormalSlash()
    {
        Quaternion starting_rotation = GetStartingQuaternion();
        GameObject _instantiated_tank_sword = Instantiate(_normal_sword_slash, transform.position, starting_rotation);
        _instantiated_tank_sword.GetComponent<RotatingSword>().center = transform.gameObject;
    }

    public void SummonWideSlash()
    {
        Quaternion starting_rotation = GetStartingQuaternion();
        GameObject _instantiated_tank_sword = Instantiate(_wide_sword_slash, transform.position, starting_rotation);
        _instantiated_tank_sword.GetComponent<RotatingSword>().center = transform.gameObject;
    }

    private Quaternion GetStartingQuaternion()
    {
        Vector3 line_to_mouse = Vector3.Normalize(_target_player.transform.position - transform.position);
        float angle = Vector3.Angle(line_to_mouse, transform.right);

        if (transform.position.y > transform.position.y) // attack is below player. the Vector3.angle function gets the SMALLEST angle between 2 angles, not the clockwise angle needed by the sword. 
        {
            angle = 180 + (180 - angle);
        }

        Quaternion starting_rotation = Quaternion.Euler(0, 0, angle - _normal_sword_slash.GetComponent<RotatingSword>().target_angles_traveled / 2); // starts tank sword at an angle such that the mouse position is the halfway point of the arc 
        return (starting_rotation);
    }

    public void MoveTowardsTargetPlayer() // beelines player. Best for melee.
    {
        _target_x = _target_player.transform.position.x;
        _target_y = _target_player.transform.position.y;
        Move();
        AddVelocityAdditives();
    }

    public void SlitherTowardsTargetPlayer() // Kinda rotates around player but also zigzaggy. Best for ranged. 
    {
        _target_x = _target_player.transform.position.x;
        _target_y = _target_player.transform.position.y;
        Move();
        degree_change += _degrees_per_second * Time.fixedDeltaTime;

        if (degree_change > _upper_degree_bound || degree_change < -_upper_degree_bound)
        {
            _degrees_per_second = -_degrees_per_second;
        }

        _rb.velocity = GameController.instance.RotateVector2(_rb.velocity, degree_change);
        AddVelocityAdditives();
    }

    private void Move() // sets rigidbody velocity DIRECTLY towards target x and y 
    {
        
        Vector2 _line_to_target = new Vector2(_target_x, _target_y) - (Vector2)transform.position;
        _line_to_target = _line_to_target.normalized;
        _rb.velocity = _line_to_target * _speed;
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
        StopAllCoroutines();
        transform.position = starting_position;
        transform.rotation = Quaternion.identity;
        _health = _max_health;
        _sprite.color = Color.white;
        CustomEvent.Trigger(transform.gameObject, "ReturnToPhase1");
    }

    public IEnumerator ChargeToTarget(float duration) // longer the charge, the more the boss accelerates
    {
        float _time = 0;
        while (_time < duration)
        {
            _time += Time.fixedDeltaTime;
            _velocity_multiplicative += 0.2f * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _time = 0;
        while (_time < 0.25f) // 0.5 is the deceleration period 
        {
            
            _time += Time.fixedDeltaTime;
            _velocity_multiplicative = Mathf.Lerp(_velocity_multiplicative, 1, 2f * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
        _velocity_multiplicative = 1;
        yield return null;
    }

    public IEnumerator MassAOE(float interval_between_attacks)
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
                GameController.instance.RefreshAllyList();
                duration = interval_between_attacks;
                GameObject _instantiated_AOE_circle = Instantiate(_boss_disease_AOE, GameController.instance.ally_list[i].transform.position, Quaternion.identity);
                yield return new WaitForFixedUpdate();
            }
        }

        yield return null;

    }

    protected override void ShakeHealthBar()
    {
        transform.GetComponent<HealthBar>().StartCoroutine(transform.GetComponent<HealthBar>().TempSizeChange(0.15f, 0.3f, 0.03f));
    }

}
