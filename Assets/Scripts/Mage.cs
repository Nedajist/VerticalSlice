using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : ClassAbility
{
    [SerializeField] private GameObject _mage_projectile;
    [SerializeField] private GameObject _vortex_circle;
    public override void Ability1(Vector3 mouse_position)
    {
        Vector3 line_to_mouse = Vector3.Normalize(mouse_position - transform.position);
        line_to_mouse *= 1.2f;
        line_to_mouse.z = 0;
        GameObject instantiated_mage_bolt = Instantiate(_mage_projectile, transform.position + line_to_mouse, Quaternion.identity);
        instantiated_mage_bolt.GetComponent<Projectile>().starting_mouseclick_position = mouse_position;
    }
    public override void Ability2(Vector3 mouse_position)
    {
        Vector3 spawn_position = new Vector3(mouse_position.x, mouse_position.y, 0);
        GameObject instantiated_vortex_circle = Instantiate(_vortex_circle, spawn_position, Quaternion.identity);
    }



}