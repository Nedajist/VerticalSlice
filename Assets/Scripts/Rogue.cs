using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class Rogue : ClassAbility
{
    [SerializeField] GameObject _hook;
    [SerializeField] GameObject _rogue_AOE;

    public override void Ability1(Vector3 mouse_position)
    {
        Vector3 line_to_mouse = Vector3.Normalize(mouse_position - transform.position);
        line_to_mouse *= 1.3f;
        line_to_mouse.z = 0;
        GameObject instantiated_hook = Instantiate(_hook, transform.position + line_to_mouse, Quaternion.identity);
        Hook hook = instantiated_hook.GetComponent<Hook>();
        hook.starting_mouseclick_position = mouse_position;
        hook.originator = transform.gameObject;

    }
    public override void Ability2(Vector3 mouse_position)
    {
        GameObject instantiated_rogue_AOE = Instantiate(_rogue_AOE, transform.position, Quaternion.identity);
        instantiated_rogue_AOE.GetComponent<RogueAOECircle>().originator = transform.gameObject;
    }
}