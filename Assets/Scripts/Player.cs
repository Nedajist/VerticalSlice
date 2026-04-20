using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.InputSystem;




public class Player : Ally
{

    [SerializeField] InputActionReference _movement_inputs;
    [SerializeField] InputActionReference _ability_inputs;
    [SerializeField] InputActionReference _mouse_inputs;
    [SerializeField] InputActionReference _mouse_movements;
    [SerializeField] Camera _camera;


    InputData _move_data; // InputData is the scriptableobject class. These are created and added to _list_of_inputs
    InputData _ability_select_data; 
    InputData _mouse_click_data;

    private Vector3 _mouse_position;
    private Vector2 _move_input;
    private float _mouse_input;
    private float _ability_input;

    private Vector2 _previous_move_input;
    private float _previous_mouse_input;
    private float _previous_ability_input;


    // Start is called before the first frame update
    protected override void Start()
    {
        transform.position = _starting_position;
    }

    // Update is called once per frame
    protected override void Update()
    {
        _ability_1_timer -= Time.deltaTime;
        _ability_2_timer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSelf();
        }

        _move_input = _movement_inputs.action.ReadValue<Vector2>(); // vector 2 made by WASD
        _mouse_input = _mouse_inputs.action.ReadValue<float>(); // 0 or 1 made by no click and mouse click  
        _ability_input = _ability_inputs.action.ReadValue<float>(); // -1 or 1 made by 1 and 2
        _mouse_position = _camera.ScreenToWorldPoint(_mouse_movements.action.ReadValue<Vector2>()); // constantly set, Vector3

        if (_move_input != Vector2.zero && _executing == false) // checks if moved while not executing
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
        else if (_executing == false) // sets velocity to zero. Otherwise after letting go of WASD player keeps moving
        {
            FreezeVelocity();
        }


        if (_ability_input != 0) // checks if ability 1 (-1) or ability 2 (1) has been selected 
        {
            if (_ability_input != _previous_ability_input) // checks if input same as last one
            {
                _ability_select_data = ScriptableObject.CreateInstance<InputData>();
                _ability_select_data.inputType = "AbilitySelect";
                _ability_select_data.abilityData = _ability_input;
                _ability_select_data.inputFrame = _physics_frames;
                _list_of_inputs.Add(_ability_select_data);
            }
            else // if same as last one, adds 1 to previous scriptable object's lifespan instead of creating a new one 
            {
                _ability_select_data.heldFrames += 1;
            }


            HandleAbilityInput(_ability_input);

        }

        if (_mouse_input == 1 && _previous_mouse_input != 1) // LMB clicked AND WAS NOT CLICKED LAST TURN. Abilities cannot be automatically activated by spamming mouse
        {
            _mouse_click_data = ScriptableObject.CreateInstance<InputData>();
            _mouse_click_data.inputType = "LeftMouseClick";
            _mouse_click_data.mousePosition = _mouse_position;
            _mouse_click_data.inputFrame = _physics_frames;
            _list_of_inputs.Add(_mouse_click_data);

            HandleMouseInput(_mouse_position);

        }

        _previous_move_input = _move_input; // stores previous frame's inputs. If they are null these will be null as well. 
        _previous_mouse_input = _mouse_input;
        _previous_ability_input = _ability_input;
        _previous_mouse_input = _mouse_input;

    }


    protected override void FixedUpdate()
    {

        _physics_frames += 1;

    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Projectile") || collision.transform.CompareTag("Enemy"))
        {
            if (collision.transform.CompareTag("Projectile"))
            {
                ReceiveDamage(collision.transform.GetComponent<Projectile>().damage);
            }
            else
            {
                ReceiveDamage(_max_health);
            }
        }


    }

    public override void ReceiveDamage(float amount)
    {
        _health -= amount;
        if (_health <= 0) // die
        {
            InputData _death_data = ScriptableObject.CreateInstance<InputData>();
            _death_data.inputType = "Die";
            _death_data.inputFrame = _physics_frames;
            _list_of_inputs.Add(_death_data);


            ResetSelf();
        }
    }


    private void ResetSelf()
    {
        GameController.instance.CullClones();
        GameController.instance.AddNewClone(_starting_position, _list_of_inputs, _class, _speed, _max_health, _ability_1_cooldown, _ability_2_cooldown);
        GameController.instance.SummonAllClones();
        _list_of_inputs = new List<InputData>();
        _physics_frames = 0;
        _starting_position += new Vector3(1, 0, 0);
        _health = _max_health;
        transform.position = _starting_position;
    }

}
