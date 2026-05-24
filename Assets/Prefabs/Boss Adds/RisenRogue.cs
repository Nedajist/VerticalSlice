using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisenRogue : RisenEntity
{
    private bool _ability_switch = false; // decides whether or not rogue uses 1st or 2nd ability 


    private void FixedUpdate()
    {
        if (_spawned_in == false) return;
        _i_frames -= Time.fixedDeltaTime;
        _targeting_timer -= Time.fixedDeltaTime;
        _ability_timer -= Time.fixedDeltaTime;

        if (_targeting_timer <= 0 || _target_player == null)
        {
            TargetWeakestPlayer();
            _targeting_timer = _targeting_cooldown;
        }

        if (_ability_timer <= 0)
        {
            if (_ability_switch) transform.GetComponent<ClassAbility>().Ability1(_target_player.transform.position);
            else transform.GetComponent<ClassAbility>().Ability2(_target_player.transform.position);
            _ability_switch = !_ability_switch;
            _ability_timer = _ability_cooldown;
        }

        MoveToTarget(); // moves to _target_player's position 

    }
}
