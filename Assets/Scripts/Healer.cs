using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    [SerializeField] private GameObject _healer_stuff;
    public void HealerAbility1(Vector3 mouse_position)
    {
        Debug.Log("Healer ability 1!");
    }
    public void HealerAbility2(Vector3 mouse_position)
    {
        Debug.Log("Healer ability 2!");
    }
}
