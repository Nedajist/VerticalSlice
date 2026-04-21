using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingCircle : Circle
{
    [SerializeField] float _healing_per_second;

    private float _radius;
    private float _radius_multiplier = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        _max_lifespan = _lifespan;
        _radius = (transform.localScale.x / 2f) * _radius_multiplier; // to make sure entities touching the outer edge of the circle are counted as close enough to be healed 
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        for (int i = 0; i < _list_of_recipients.Count; i++)
        {
            if (_list_of_recipients[i] != null && Vector2.Distance((Vector2) transform.position, (Vector2)_list_of_recipients[i].transform.position) < _radius)
            {
                _list_of_recipients[i].ReceiveHealing(_healing_per_second * Time.deltaTime);
            }
            else
            {
                _list_of_recipients.RemoveAt(i);
                i--;
            }
        }

        _lifespan -= Time.deltaTime;
        
        _sprite_renderer.color = new Color(_sprite_renderer.color.r, _sprite_renderer.color.g, _sprite_renderer.color.b, _lifespan / _max_lifespan); // circle becomes more transparent each frame
        transform.localScale = new Vector3(1.5f * _lifespan / _max_lifespan, 1.5f * _lifespan / _max_lifespan, 0);
        _radius = (transform.localScale.x / 2f) * _radius_multiplier; 

        if (_lifespan <= 0)
        {
            Destroy(gameObject); 
        }

    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
    }




}
