using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingEntity
{
    [SerializeField] protected Ally _target_player;

    [SerializeField] public float contact_damage;
    [SerializeField] public Vector2 _velocity_additive;
    [SerializeField] public float _velocity_multiplicative = 1;



    protected void TargetNearestPlayer()
    {
        GameController.instance.RefreshAllyList();
        float nearest_distance = Vector3.Distance(transform.position, GameController.instance.ally_list[0].transform.position);
        _target_player = GameController.instance.ally_list[0];
        for (int i = 0; i < GameController.instance.ally_list.Count; i++)
        {
            Ally new_player = GameController.instance.ally_list[i];
            float new_distance = Vector3.Distance(new_player.transform.position, transform.position);
            if (new_distance < nearest_distance)
            {
                nearest_distance = new_distance;
                _target_player = new_player;
            }
        }
    }

    protected void TargetFarthestPlayer()
    {
        GameController.instance.RefreshAllyList();
        float farthest_distance = Vector3.Distance(transform.position, GameController.instance.ally_list[0].transform.position);
        _target_player = GameController.instance.ally_list[0];
        for (int i = 0; i < GameController.instance.ally_list.Count; i++)
        {
            Ally new_player = GameController.instance.ally_list[i];
            float new_distance = Vector3.Distance(new_player.transform.position, transform.position);
            if (new_distance > farthest_distance)
            {
                farthest_distance = new_distance;
                _target_player = new_player;
            }
        }

    }

    protected void TargetStrongestPlayer()
    {
        GameController.instance.RefreshAllyList();
        float highest_health = GameController.instance.ally_list[0].GetHealth();
        _target_player = GameController.instance.ally_list[0];
        for (int i = 0; i < GameController.instance.ally_list.Count; i++)
        {
            Ally new_player = GameController.instance.ally_list[i];
            float new_health = new_player.GetHealth();
            if (new_health > highest_health)
            {
                highest_health = new_health;
                _target_player = new_player;
            }
        }
    }

    protected void TargetWeakestPlayer()
    {
        GameController.instance.RefreshAllyList();
        float lowest_health = GameController.instance.ally_list[0].GetHealth();
        _target_player = GameController.instance.ally_list[0];
        for (int i = 0; i < GameController.instance.ally_list.Count; i++)
        {
            Ally new_player = GameController.instance.ally_list[i];
            float new_health = new_player.GetHealth();
            if (new_health < lowest_health)
            {
                lowest_health = new_health;
                _target_player = new_player;
            }
        }
    }
}
