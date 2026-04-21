using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisenHealer : RisenEntity
{
    private Boss _target_boss;

    private void Start()
    {
        _target_boss = GameObject.FindAnyObjectByType<Boss>();
    }

    private void FixedUpdate()
    {
        _targeting_timer -= Time.fixedDeltaTime;
        _ability_timer -= Time.fixedDeltaTime;

        if (_targeting_timer <= 0)
        {
            TargetFarthestPlayer();
            _targeting_timer = _targeting_cooldown;
        }

        if (_ability_timer <= 0)
        {
            if (_target_boss == null)
            {
                return;
            }

            transform.GetComponent<ClassAbility>().Ability1(_target_boss.transform.position);
            _ability_timer = _ability_cooldown;
        }

        MoveToTarget(); // moves to _target_player's position 

    }
}
