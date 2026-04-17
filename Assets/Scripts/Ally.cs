using System.Collections.Generic;
using UnityEngine;
public enum PlayerClass
{
    Mage,
    Tank,
    Healer
}
public class Ally : MonoBehaviour // the clone
{
    [SerializeField] protected Rigidbody2D _rb;
    [SerializeField] protected float _speed;
    [SerializeField] protected float _health = 100;
    [SerializeField] protected float _max_health = 100;
    [SerializeField] protected PlayerClass _class;
    [SerializeField] protected int _selected_ability = 1; // 1 or 2 
    [SerializeField] protected float _ability_1_cooldown;
    [SerializeField] protected float _ability_2_cooldown;

    [SerializeField] protected Vector3 _starting_position = Vector3.zero;
    [SerializeField] protected float _iframe_duration = 0.25f;

    [SerializeField] private GameObject _mage_projectile;

    public Vector2 _velocity_additive;
    public bool is_clone = true;
    protected List<InputData> _list_of_inputs = new List<InputData>();

    protected float _ability_1_timer = 0;
    protected float _ability_2_timer = 0;

    protected int _physics_frames;
    protected float _i_frames;
    protected bool _executing = false;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        transform.position = _starting_position;
        _physics_frames = 0;
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {
        _i_frames -= Time.deltaTime;
        _ability_1_timer -= Time.deltaTime;
        _ability_2_timer -= Time.deltaTime;
        _physics_frames += 1;
        if (_executing) // calls _execute() once every physics frame, so 0-1 inputs are executed each second. 
        {
            _execute();
        }

    }

    protected void _execute() // executes all inputs that start on this frame or on a previous frame but have not finished executing. Each individual input is executed only once per frame. This method is only called once per frame.
    {
        bool moved_this_frame = false;

        for (int i = 0; i < _list_of_inputs.Count; i++) // while there are still inputs happening THIS FRAME to execute
        {
            InputData current_input = _list_of_inputs[i]; // gets current input scriptable object


            if (current_input.inputFrame != _physics_frames && current_input.startedExecution == false) // if the selected input does not happen this frame / is not a previous input which stretches on to this frame, ends the loop
            {
                if (moved_this_frame == false)
                {
                    FreezeVelocity(); // resets velocity if not executing movement input this turn to preserve determinism
                }
                return;
            }

            current_input.startedExecution = true; // tells the scriptableobject that it has started execution

            switch (current_input.inputType) // element i of input list matches current frame, input will be executed
            {
                case "Movement":
                    moved_this_frame = true; // ensures velocity isn't frozen
                    HandleMovementInput(current_input.movementData);
                    break;
                case "AbilitySelect":
                    HandleAbilityInput(current_input.abilityData);
                    break;
                case "LeftMouseClick":
                    HandleMouseInput(current_input.mousePosition);
                    break;
                case "Die":
                    Destroy(gameObject);
                    return;
            }

            current_input.heldFrames -= 1; // subtracts 1 from its lifespan 
            if (current_input.heldFrames <= 0)
            {
                _list_of_inputs.RemoveAt(0); // removes executed input if its lifespan has reached 0 
            }
        }

        if (_list_of_inputs.Count == 0)
        {
            _executing = false;
            FreezeVelocity();
            Debug.Log("FINISHED EXECUTING ON FRAME " + _physics_frames);
            Destroy(gameObject); // die doesn't seem to be working
        }


    }

    protected void HandleMovementInput(Vector2 direction) // moves this player by direction
    {
        _rb.velocity = (direction * _speed) + _velocity_additive;
    }

    protected void HandleAbilityInput(float number) // selects ability
    {
        if (number == -1)
        {
            _selected_ability = 1;
        }
        else if (number == 1)
        {
            _selected_ability = 2;
        }
    }

    protected void HandleMouseInput(Vector3 mouse_position)
    {
        if (_selected_ability == 1 && _ability_1_timer <= 0)
        {
            _ability_1_timer = _ability_1_cooldown;
            ActivateAbility1(mouse_position);
        }

        else if (_selected_ability == 2 && _ability_2_timer <= 0)
        {
            _ability_2_timer = _ability_2_cooldown;
            ActivateAbility2(mouse_position);
        }

    }

    protected virtual void ActivateAbility1(Vector3 mouse_position)
    {
        switch (_class)
        {
            case PlayerClass.Mage:
                MageAbility1(mouse_position);
                break;
        }
    }

    protected virtual void ActivateAbility2(Vector3 mouse_position)
    {
        return;
    }

    protected void MageAbility1(Vector3 mouse_position)
    {
        Vector3 line_to_mouse = Vector3.Normalize(mouse_position - transform.position);
        line_to_mouse *= 1.5f;
        line_to_mouse.z = 0;
        GameObject instantiated_mage_bolt = Instantiate(_mage_projectile, transform.position + line_to_mouse, Quaternion.identity);
        instantiated_mage_bolt.GetComponent<Projectile>().starting_mouseclick_position = mouse_position;
    }

    protected void FreezeVelocity()
    {
        _rb.velocity = Vector2.zero + _velocity_additive;
    }

    public void StartExecuting()
    {
        //Debug.Log("STARTED ExEcution with " + _list_of_inputs.Count + " inputs!");
        _executing = true;
        transform.position = _starting_position;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Projectile") || collision.transform.CompareTag("Enemy"))
        {
            if (collision.transform.CompareTag("Projectile"))
            {
                ReceiveDamage(collision.transform.GetComponent<Projectile>().damage);
            }

        }
    }

    public virtual void ReceiveDamage(float amount)
    {
        if (_i_frames > 0)
        {
            return;
        }
        _i_frames = _iframe_duration;
        _health -= amount;
        if (_health <= 0) // die
        {
            for (int i = 0; i < GameController.instance.ally_list.Count; i++)
            {
                if (GameController.instance.ally_list[i].GetInstanceID() == transform.GetInstanceID())
                {
                    Debug.Log("REMOVED SELF FROM ALLY LIST");
                    GameController.instance.ally_list.RemoveAt(i);
                    break;
                }
            }
            Destroy(gameObject);
        }
    }

    public virtual void ReceiveHealing(float amount)
    {
        _health += amount;
    }



    public void GiveLife(Vector3 starting_position, List<InputData> list_of_inputs)
    {
        _starting_position = starting_position;
        _physics_frames = 0;

        for (int i = 0; i < list_of_inputs.Count; i++)
        {
            InputData new_input_data = ScriptableObject.CreateInstance<InputData>();
            new_input_data.SetValues(list_of_inputs[i]);
            _list_of_inputs.Add(new_input_data);
        }


    }


}
