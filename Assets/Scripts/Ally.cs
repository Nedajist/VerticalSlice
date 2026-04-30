using System.Collections.Generic;
using UnityEngine;
public enum PlayerClass
{
    Mage,
    Tank,
    Healer
}
public class Ally : LivingEntity // the clone
{
    [SerializeField] protected Rigidbody2D _rb;
    [SerializeField] protected PlayerClass _class;
    [SerializeField] protected int _selected_ability = 1; // 1 or 2 
    [SerializeField] protected float _ability_1_cooldown;
    [SerializeField] protected float _ability_2_cooldown;

    [SerializeField] public Vector3 starting_position = Vector3.zero;

    public bool is_clone = true;
    public float seconds_of_infection;
    public bool infected = false;


    protected List<InputData> _list_of_inputs = new List<InputData>();
    protected List<InputData> _list_of_unchanging_inputs = new List<InputData>(); // this list is used to clone the given list of inputdata when _list_of_inputs runs out 


    protected float _ability_1_timer = 0;
    protected float _ability_2_timer = 0;

    protected int _physics_frames;
    protected bool _executing = false;
    protected float _infection_damage = 20;

    private bool _dead = false;



    // Start is called before the first frame update
    protected override void Start()
    {
        transform.position = starting_position;
        _physics_frames = 0;
        SetColor();
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {
        _i_frames -= Time.fixedDeltaTime;
        _ability_1_timer -= Time.fixedDeltaTime;
        _ability_2_timer -= Time.fixedDeltaTime;
        _physics_frames += 1;
        if (_executing) // calls _execute() once every physics frame, so 0-1 inputs are executed each second. 
        {
            _execute();
        }

        if (seconds_of_infection > 0)
        {
            //Debug.Log("Receieved " + _infection_damage * Time.fixedDeltaTime + " infection damage");
            seconds_of_infection -= Time.fixedDeltaTime;
            ReceiveInfectionDamage(_infection_damage * Time.fixedDeltaTime); 
        }
    }

    protected void _execute() // executes all inputs that start on this frame or on a previous frame but have not finished executing. Each individual input is executed only once per frame. This method is only called once per frame.
    {
        bool moved_this_frame = false;

        for (int i = 0; i < _list_of_inputs.Count; i++) // while there are still inputs happening THIS FRAME to execute
        {
            //Debug.Log("INDEX 0 INPUT FRAME: "+ _list_of_inputs[0].inputFrame);
            InputData current_input = _list_of_inputs[i]; // gets current input scriptable object

            if (current_input.inputFrame != _physics_frames && current_input.startedExecution == false) // if the selected input does not happen this frame / is not a previous input which stretches on to this frame, ends the loop
            {
                //Debug.Log("RETURNED. CURRENT FRAME: " + _physics_frames + ". INPUT FRAME: " + current_input.inputFrame + "STARTED EXECUTION: " + current_input.startedExecution);
                return;
            }

            current_input.startedExecution = true; // tells the scriptableobject that it has started execution
            //Debug.Log("STARTED EXECUTION. CURRENT FRAME: " + _physics_frames + "INPUT FRAME:" + current_input.inputFrame);

            switch (current_input.inputType) // element i of input list matches current frame, input will be executed
            {
                case "Movement":
                    moved_this_frame = true; // ensures velocity isn't frozen
                    HandleMovementInput(current_input.movementData);
                    break;
                case "LeftMouseClick":
                    HandleLeftMouseInput(current_input.mousePosition);
                    break;
                case "RightMouseClick":
                    HandleRightMouseInput(current_input.mousePosition);
                    break;
            }

            if (moved_this_frame == false)
            {
                FreezeVelocity(); // resets velocity if not executing movement input this turn to preserve determinism
            }

            current_input.heldFrames -= 1; // subtracts 1 from its lifespan 
            if (current_input.heldFrames <= 0)
            {
                _list_of_inputs.RemoveAt(i); // removes executed input if its lifespan has reached 
                i--;
            }
            //Debug.Log(_list_of_inputs.Count + "inputs remaining!");
            //Debug.Log("current input: "+ current_input.inputType + "heldframes remaining: "+ current_input.heldFrames);
        }

        if (_list_of_inputs.Count == 0)
        {
            ResetInputs();
        }

    }

    protected void HandleMovementInput(Vector2 direction) // moves this player by direction
    {
        _rb.velocity = (direction * _speed) + velocity_additive;
    }


    protected void HandleLeftMouseInput(Vector3 mouse_position)
    {
        if (_ability_1_timer <= 0)
        {
            _ability_1_timer = _ability_1_cooldown;
            ActivateAbility1(mouse_position);
        }
    }

    protected void HandleRightMouseInput(Vector3 mouse_position)
    { 
        if (_ability_2_timer <= 0)
        {
            _ability_2_timer = _ability_2_cooldown;
            ActivateAbility2(mouse_position);
        }

    }

    protected virtual void ActivateAbility1(Vector3 mouse_position)
    {
        transform.GetComponent<ClassAbility>().Ability1(mouse_position);
    }

    protected virtual void ActivateAbility2(Vector3 mouse_position)
    {
        transform.GetComponent<ClassAbility>().Ability2(mouse_position);
    }

    protected void FreezeVelocity()
    {
        _rb.velocity = Vector2.zero + velocity_additive;
    }

    public void StartExecuting()
    {
        _executing = true;
        transform.position = starting_position;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
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
                ReceiveDamage(enemy.contact_damage);
            }

        }
    }

    public override void ReceiveDamage(float amount)
    {
        if (_i_frames > 0 || _dead == true)
        {
            return;
        }

        _i_frames = _iframe_duration;
        _health -= amount;
        ShakeHealthBar();
        StartCoroutine(FlashColor(0.08f, 0.08f, Color.red));
        if (_health <= 0) // die
        {
            for (int i = 0; i < GameController.instance.ally_list.Count; i++)
            {
                if (GameController.instance.ally_list[i].GetInstanceID() == transform.GetInstanceID())
                {
                    GameController.instance.ally_list.RemoveAt(i); // removes self from ally list upon death
                    break;
                }
            }

            if (seconds_of_infection > 0)
            {
                GameController.instance.SummonRisen(transform.position, _class);
                GameController.instance.SummonInfectiousProjectileBundle(transform.position);
            }

            _dead = true;
            Destroy(gameObject);
        }
    }

    public virtual void ReceiveInfectionDamage(float amount) // ignores iframes
    {
        if (_dead == true)
        {
            return;
        }

        _health -= amount;

        if (_health <= 0)
        {
            _dead = true;
            GameController.instance.SummonRisen(transform.position, _class);
            GameController.instance.SummonInfectiousProjectileBundle(transform.position);
            Destroy(gameObject);
        }

        StartCoroutine(FlashColor(0.03f, 0.03f, Color.magenta));

    }


    public void GiveLife(Vector3 start_position, List<InputData> list_of_inputs)
    {
        starting_position = start_position;
        _physics_frames = 0;

        for (int i = 0; i < list_of_inputs.Count; i++) // true copies mutable values 
        {
            InputData new_input_data = ScriptableObject.CreateInstance<InputData>();
            new_input_data.SetValues(list_of_inputs[i]);
            _list_of_unchanging_inputs.Add(new_input_data);
        }


    }

    public void AddIFrames(float seconds_of_iframes)
    {
        _i_frames += seconds_of_iframes;
    }

    private void ResetInputs()
    {
        for (int i = 0; i < _list_of_unchanging_inputs.Count; i++)
        {
            InputData new_input_data = ScriptableObject.CreateInstance<InputData>();
            new_input_data.SetValues(_list_of_unchanging_inputs[i]);
            _list_of_inputs.Add(new_input_data);
        }
        _physics_frames = 0;
        FreezeVelocity();
    }



    public float GetAbility1Timer()
    {
        return _ability_1_timer;
    }
    public float GetAbility2Timer()
    {
        return _ability_2_timer;
    }

    public float GetAbility1Cooldown()
    {
        return _ability_1_cooldown;
    }

    public float GetAbility2Cooldown()
    {
        return _ability_2_cooldown;
    }
}
