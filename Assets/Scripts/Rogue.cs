using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class Rogue : ClassAbility
{
    [SerializeField] GameObject _pullHook;
    [SerializeField] GameObject _pushHook;

    public override void Ability1(Vector3 mouse_position)
    {
        PlayAbility1SFX();
        Vector3 line_to_mouse = Vector3.Normalize(mouse_position - transform.position);
        line_to_mouse *= 1.3f;
        line_to_mouse.z = 0;
        GameObject instantiated_hook = Instantiate(_pullHook, transform.position + line_to_mouse, Quaternion.identity);
        Hook hook = instantiated_hook.GetComponent<Hook>();
        hook.starting_mouseclick_position = mouse_position;
        hook.originator = transform.gameObject;

    }
    public override void Ability2(Vector3 mouse_position)
    {
        Vector3 line_to_mouse = Vector3.Normalize(mouse_position - transform.position);
        line_to_mouse *= 1.3f;
        line_to_mouse.z = 0;
        GameObject instantiated_hook = Instantiate(_pushHook, transform.position + line_to_mouse, Quaternion.identity);
        Hook hook = instantiated_hook.GetComponent<Hook>();
        hook.starting_mouseclick_position = mouse_position;
        hook.originator = transform.gameObject;
    }
}