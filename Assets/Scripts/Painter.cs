using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Painter : ClassAbility
{
    [SerializeField] private GameObject _wall_rectangle;
    [SerializeField] private GameObject _moving_rectangle;

    private PlaceableRectangle _current_rectangle;
    private GameObject _current_object;

    private Vector3 _starting_mouse_coordinates;
    
    private void FixedUpdate()
    {
        if (_current_rectangle != null)
        {
            _current_object = _current_rectangle.gameObject;
        }

        if (_current_rectangle == null) return;

        Vector3 lineToSelf = Vector3.Normalize(transform.position - _current_rectangle.transform.position);

        Vector3 lineAlmostToSelf = (transform.position - _current_rectangle.transform.position) + (_current_rectangle.transform.position - transform.position).normalized;

        Vector3 lineEnd = _current_rectangle.transform.position + lineAlmostToSelf;
        float angle = Mathf.Atan2(lineToSelf.y, lineToSelf.x) * Mathf.Rad2Deg;

        Quaternion rotationToSelf = Quaternion.AngleAxis(angle, Vector3.forward);

        

        _current_object.transform.rotation = rotationToSelf;
        _current_object.transform.localScale = new Vector3(Vector3.Distance(_starting_mouse_coordinates, lineEnd), 0.5f, 0);
        _current_object.transform.position = new Vector3((_starting_mouse_coordinates.x + lineEnd.x) / 2, (_starting_mouse_coordinates.y + lineEnd.y) / 2, 0);

    }

    public override void Ability1(Vector3 mouse_position) // Rectangle draw
    {
        PlayAbility1SFX();
        Vector3 line_to_mouse = Vector3.Normalize(mouse_position - transform.position);
        line_to_mouse *= 1.2f;
        line_to_mouse.z = 0;
        GameObject instantiated_moving_rectangle = Instantiate(_moving_rectangle, transform.position + line_to_mouse, Quaternion.identity);
        instantiated_moving_rectangle.GetComponent<MovingRectangle>().starting_mouseclick_position = mouse_position;

    }

    public override void Ability2(Vector3 mouse_position) // Arrow draw
    {
        PlayAbility2SFX();
        Vector3 spawn_position = new Vector3(mouse_position.x, mouse_position.y, 0);

        if (_current_rectangle != null) // player is currently placing rectangle
        {
            _current_rectangle.Place();
            _current_rectangle = null;
        }
        else // player isn't placing anything, so instantiates rectangle
        {
            GameObject instantiated_rectangle = Instantiate(_wall_rectangle, spawn_position, Quaternion.identity);
            _current_rectangle = instantiated_rectangle.GetComponent<PlaceableRectangle>();
            _starting_mouse_coordinates = mouse_position;
        }

    }

    private void OnDestroy()
    {
        if (_current_rectangle != null)
        {
            Destroy(_current_rectangle.gameObject);
        }
    }
}