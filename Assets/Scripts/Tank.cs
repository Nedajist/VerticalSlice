using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : ClassAbility
{
    [SerializeField] private GameObject _tank_sword;
    public override void Ability1(Vector3 mouse_position)
    {
        Vector3 line_to_mouse = Vector3.Normalize(mouse_position - transform.position);
        float angle = Vector3.Angle(line_to_mouse, transform.right);

        if (transform.position.y > mouse_position.y) // attack is below player. the Vector3.angle function gets the SMALLEST angle between 2 angles, not the clockwise angle needed by the sword. 
        {
            angle = 180 + (180 - angle);
        }

        Quaternion starting_rotation = Quaternion.Euler(0, 0, angle - _tank_sword.GetComponent<TankSword>().target_angles_traveled/2); // starts tank sword at an angle such that the mouse position is the halfway point of the arc 


        GameObject _instantiated_tank_sword = Instantiate(_tank_sword, transform.position, starting_rotation);
        _instantiated_tank_sword.GetComponent<TankSword>().center = transform.gameObject;
        
    }
    public override void Ability2(Vector3 mouse_position)
    {
        Debug.Log("Tank ability 2!");
    }
}
