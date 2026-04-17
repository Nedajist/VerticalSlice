using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    [SerializeField] private GameObject _tank_sword;
    public void TankAbility1(Vector3 mouse_position)
    {
        Debug.Log("Tank ability 1!");
    }
    public void TankAbility2(Vector3 mouse_position)
    {
        Debug.Log("Tank ability 2!");
    }
}
