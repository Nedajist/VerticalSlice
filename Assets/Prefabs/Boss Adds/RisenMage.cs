using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisenMage : RisenEntity
{
    private void FixedUpdate()
    {
        _targeting_timer -= Time.fixedDeltaTime;
        _ability_timer -= Time.fixedDeltaTime;

        if (_targeting_timer <= 0)
        {
            TargetWeakestPlayer();
            _targeting_timer = _targeting_cooldown;
        }

        if (_ability_timer <= 0)
        {
            transform.GetComponent<ClassAbility>().Ability1(_target_player.transform.position);
            _ability_timer = _ability_cooldown;
        }

        MoveToTarget(); // moves to _target_player's position 

    }
}
