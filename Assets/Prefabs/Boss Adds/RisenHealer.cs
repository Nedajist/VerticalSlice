using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisenHealer : RisenEntity
{
    [SerializeField] float _max_cast_distance = 5f;
    private GameObject _ability_target;

    private void AbilityTargetHealthiestEnemy() // Casts heal on the enemy (to the player) gameobject with the most health. Usually the boss. 
    {
        GameObject[] enemy_list = GameObject.FindGameObjectsWithTag("Enemy");
        Enemy healthiest_enemy = null;
        foreach (GameObject enemy_object in enemy_list)
        {
            Enemy enemy = enemy_object.GetComponent<Enemy>();
            if (enemy == null) continue;
            if (healthiest_enemy == null) healthiest_enemy = enemy;
            else
            {
                if (enemy.GetHealth() > healthiest_enemy.GetHealth())
                {
                    healthiest_enemy = enemy;
                    continue;
                }
            }

        }
        _ability_target = healthiest_enemy.transform.gameObject;
    }

    private void FixedUpdate()
    {
        if (_spawned_in == false) return;
        _i_frames -= Time.fixedDeltaTime;
        _targeting_timer -= Time.fixedDeltaTime;
        _ability_timer -= Time.fixedDeltaTime;

        if (_targeting_timer <= 0 || _target_player == null)
        {
            TargetFarthestPlayer();
            AbilityTargetHealthiestEnemy();
            _targeting_timer = _targeting_cooldown;
        }

        MoveToTarget(); // moves to _target_player's position 

        if (_ability_timer <= 0)
        {
            if (_ability_target == null) return;

            if (Vector3.Distance(transform.position, _ability_target.transform.position) > _max_cast_distance) return;

            transform.GetComponent<ClassAbility>().Ability1(_ability_target.transform.position);
            _ability_timer = _ability_cooldown;
        }


    }
}
