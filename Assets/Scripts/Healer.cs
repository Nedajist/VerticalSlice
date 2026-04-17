using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : ClassAbility
{
    [SerializeField] private GameObject _healer_stuff;
    public override void Ability1(Vector3 mouse_position)
    {
        Debug.Log("Healer ability 1!");
    }
    public override void Ability2(Vector3 mouse_position)
    {
        Debug.Log("Healer ability 2!");
    }
}
