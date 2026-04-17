using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : ClassAbility
{
    [SerializeField] private GameObject _tank_sword;
    public override void Ability1(Vector3 mouse_position)
    {
        Debug.Log("Tank ability 1!");
    }
    public override void Ability2(Vector3 mouse_position)
    {
        Debug.Log("Tank ability 2!");
    }
}
