using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;




public class Player : Ally
{

    [SerializeField] InputActionReference _movement_inputs;
    [SerializeField] InputActionReference _ability_inputs;
    [SerializeField] InputActionReference _mouse_inputs;
    [SerializeField] InputActionReference _right_mouse_inputs;
    [SerializeField] InputActionReference _mouse_movements;
    [SerializeField] public Camera _camera;


    InputData _move_data; // InputData is the scriptableobject class. These are created and added to _list_of_inputs
    InputData _mouse_click_data;

    private Vector3 _mouse_position;
    private Vector2 _move_input;
    private float _mouse_input;
    private float _right_mouse_input;

    private Vector2 _previous_move_input;
    private float _previous_mouse_input;
    private float _previous_right_mouse_input;


    // Start is called before the first frame update
    protected override void Start()
    {
        transform.position = starting_position;
        _camera = GameController.instance.main_camera;
        SetColor();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSelf();
        }
        //Debug.Log(velocity_additive);

    }


    protected override void FixedUpdate()
    {
        _physics_frames += 1;
        _ability_1_timer -= Time.fixedDeltaTime;
        _ability_2_timer -= Time.fixedDeltaTime;
        _i_frames -= Time.fixedDeltaTime;


        _move_input = _movement_inputs.action.ReadValue<Vector2>(); // vector 2 made by WASD
        _mouse_input = _mouse_inputs.action.ReadValue<float>(); // 0 or 1 made by no click and LMB  
        _right_mouse_input = _right_mouse_inputs.action.ReadValue<float>(); // 0 or 1 made by no click and RMB 
        _mouse_position = _camera.ScreenToWorldPoint(_mouse_movements.action.ReadValue<Vector2>()); // constantly set, Vector3

        if (_move_input != Vector2.zero) // checks if moved while not executing
        {
            if (_move_input != _previous_move_input)  // creates new scriptable objects if input is different from last one
            {
                _move_data = ScriptableObject.CreateInstance<InputData>();
                _move_data.inputType = "Movement";
                _move_data.movementData = _move_input;
                _move_data.inputFrame = _physics_frames;
                _list_of_inputs.Add(_move_data);
            }
            else // if previous movement input is same as new one, adds 1 to previous scriptableobject's lifespan instead of creating new scriptable object
            {
                _move_data.heldFrames += 1;
            }
            HandleMovementInput(_move_input);
        }
        else
        {
            FreezeVelocity();
        }

        if (_mouse_input == 1 && _previous_mouse_input != 1) // LMB clicked AND WAS NOT CLICKED LAST TURN. Abilities cannot be automatically activated by spamming mouse
        {
            _mouse_click_data = ScriptableObject.CreateInstance<InputData>();
            _mouse_click_data.inputType = "LeftMouseClick";
            _mouse_click_data.mousePosition = _mouse_position;
            _mouse_click_data.inputFrame = _physics_frames;
            _list_of_inputs.Add(_mouse_click_data);
            HandleLeftMouseInput(_mouse_position);
        }

        if (_right_mouse_input == 1 && _previous_right_mouse_input != 1) // RMB clicked AND WAS NOT CLICKED LAST TURN. Abilities cannot be automatically activated by spamming mouse
        {
            _mouse_click_data = ScriptableObject.CreateInstance<InputData>();
            _mouse_click_data.inputType = "RightMouseClick";
            _mouse_click_data.mousePosition = _mouse_position;
            _mouse_click_data.inputFrame = _physics_frames;
            _list_of_inputs.Add(_mouse_click_data);
            HandleRightMouseInput(_mouse_position);

        }


        _previous_move_input = _move_input; // stores previous frame's inputs. If they are null these will be null as well. 
        _previous_mouse_input = _mouse_input;
        _previous_right_mouse_input = _right_mouse_input;

        if (seconds_of_infection > 0) // takes DOT damage if infected. This bypasses i-frames 
        {
            //Debug.Log("Receieved " + _infection_damage * Time.fixedDeltaTime + " infection damage");
            seconds_of_infection -= Time.fixedDeltaTime;
            ReceiveInfectionDamage(_infection_damage * Time.fixedDeltaTime);
        }

    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Projectile") || collision.transform.CompareTag("Enemy"))
        {
            if (collision.transform.CompareTag("Projectile"))
            {
                Projectile projectile = collision.transform.GetComponent<Projectile>();
                ReceiveDamage(projectile.damage);

                if (projectile.infectious == true)
                {
                    seconds_of_infection += projectile.seconds_of_DOT;
                }
            }

            if (collision.transform.CompareTag("Enemy"))
            {
                Enemy enemy = collision.transform.GetComponent<Enemy>();
                if (enemy == null) return; // boss ears are tagged as enemy but only has livingEntity script 
                ReceiveDamage(enemy.contact_damage);
            }
        }


    }

    public override void ReceiveDamage(float amount)
    {
        if (_i_frames > 0) // if has iframes, doesn't take damage
        {
            return;
        }
        _i_frames = _iframe_duration;

        _health -= amount;
        ShakeHealthBar();
        ShakeCamera(0.15f);
        StartCoroutine(FlashColor(0.08f, 0.08f, Color.red));

        if (_health <= 0) // die
        {
            //InputData _death_data = ScriptableObject.CreateInstance<InputData>();
            //_death_data.inputType = "Die";
            //_death_data.inputFrame = _physics_frames;
            //_list_of_inputs.Add(_death_data);
            ResetSelf();
        }
    }

    public override void ReceiveInfectionDamage(float amount)
    {
        _health -= amount;
        if (_health <= 0)
        {
            ResetSelf();
        }
        else
        {
            StartCoroutine(FlashColor(0.08f, 0.08f, Color.magenta));
        }

    }


    private void ResetSelf()
    {
        GameController.instance.AddNewClone(starting_position, _list_of_inputs, _class, _speed, _max_health, _ability_1_cooldown, _ability_2_cooldown);
        Debug.Log(_class);
        GameController.instance.main_camera.transform.SetParent(GameController.instance.transform);
        GameController.instance.TransitionGameState(GameState.selecting);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void ShakeCamera(float duration)
    {
        _camera.transform.GetComponent<MainCamera>().seconds_of_camera_shake += duration;
    }

}
