using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : ClassAbility
{
    [SerializeField] private GameObject _healer_circle;
    [SerializeField] private GameObject _turret;

    public override void Ability1(Vector3 mouse_position)
    {
        PlayAbility1SFX();
        Vector3 spawn_position = new Vector3(mouse_position.x, mouse_position.y, 0);
        GameObject instantiated_healer_circle = Instantiate(_healer_circle, spawn_position, Quaternion.identity);
    }
    public override void Ability2(Vector3 mouse_position)
    {
        PlayAbility2SFX();
        Vector3 spawn_position = new Vector3(mouse_position.x, mouse_position.y, 0);
        GameObject instantiated_turret = Instantiate(_turret, spawn_position, Quaternion.identity);
    }
}
