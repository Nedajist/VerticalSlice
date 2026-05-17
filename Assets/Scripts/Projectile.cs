using UnityEngine;


public class Projectile: Enemy
{
    [SerializeField] protected float _lifespan = 6;
    [SerializeField] protected float _movement_speed = 4; // right now Speed is redundant and not used by projectile 
    [SerializeField] protected float _min_movement_speed = 2;
    [SerializeField] protected float _seconds_of_homing_time;
    [SerializeField] bool _homing;
    [SerializeField] bool _rotating;
    [SerializeField] public bool clockwise = true;
    [SerializeField] float _angles_per_second = 5; // also needed for homing
    [SerializeField] bool _initial_lock_on = true; // if true, locks on to player at start
    [SerializeField] public float damage;
    [SerializeField] public bool infectious;
    [SerializeField] public int infectious_target_index;
    [SerializeField] public float seconds_of_DOT;
    
    private float _angle;
    public Vector3 starting_mouseclick_position;
    public float starting_angle;

    [SerializeField] protected Rigidbody2D _rigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        _angle = starting_angle;

        if (_homing == true || _initial_lock_on == true && infectious == false) // only used by enemies/boss
        {
            TargetNearestPlayer();
            transform.right = _target_player.transform.position - transform.position;
        }


        if (infectious == true)
        {
            switch (infectious_target_index)
            {
                case 0:
                    TargetNearestPlayer();
                    break;
                case 1:
                    TargetWeakestPlayer();
                    break;
                case 2:
                    TargetStrongestPlayer();
                    break;
                default:
                    TargetFarthestPlayer();
                    break;
            }
            transform.right = _target_player.transform.position - transform.position;
        }


        if (_rotating == true && _initial_lock_on == false)
        {
            transform.right = new Vector3(Mathf.Cos(_angle * Mathf.Deg2Rad), Mathf.Sin(_angle * Mathf.Deg2Rad), 0);
        }

        if (starting_mouseclick_position != Vector3.zero) // only used by player
        {
            starting_mouseclick_position.z = 0;
            transform.right = starting_mouseclick_position - transform.position;
        }
        
    }



    private void FixedUpdate()
    {
        _lifespan -= Time.deltaTime;
        if (_rotating)
        {
            if (clockwise)
            {
                _angle += _angles_per_second * Time.fixedDeltaTime;

            }
            else
            {
                _angle -= _angles_per_second * Time.fixedDeltaTime;
            }

            _angle %= 360;
        }

        if (_lifespan <= 0)
        {
            Destroy(gameObject);
        }

        
        if (_rigidbody.velocity.magnitude < _min_movement_speed)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _min_movement_speed;
        }
        
        if (_homing == true && _seconds_of_homing_time > 0 && _target_player != null)
        {
            _seconds_of_homing_time -= Time.deltaTime;
            transform.right = transform.right + ((_target_player.transform.position - transform.position) - transform.right) * (Time.deltaTime * _angles_per_second / 360f);
            _rigidbody.velocity = (transform.right * _movement_speed); // this is where projectiles move forward if homing;
        }
        else // this is where projectiles move forward if not homing;
        {
            _rigidbody.AddForce(transform.right * _movement_speed);
        }


        if (_rotating == true)
        {
            transform.eulerAngles = new Vector3(0, 0, _angle);
        }

        AddVelocityAdditives();

    }

    public void TargetLivingEntity(LivingEntity target)
    {
        transform.right = target.transform.position - transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);

    }

    public void FreezeVelocity()
    {
        _rigidbody.velocity = Vector2.zero;
    }


}