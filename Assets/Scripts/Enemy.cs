using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected Ally _target_player;
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
}
