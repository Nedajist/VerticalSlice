using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisenEntity : LivingEntity
{
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] protected float _ability_cooldown;
    [SerializeField] protected Vector3 _starting_position = Vector3.zero;
    [SerializeField] GameObject _target;

    public PlayerClass risen_class;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = _starting_position;
    }

    // Update is called once per frame
    void Update()
    {
        





    }
}
