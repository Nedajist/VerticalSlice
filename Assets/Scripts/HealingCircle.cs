using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingCircle : MonoBehaviour
{
    [SerializeField] SpriteRenderer _sprite_renderer;
    [SerializeField] float _healing_per_second;
    [SerializeField] float _lifespan;

    private float _max_lifespan;
    private float _radius;
    private float _radius_multiplier = 1.5f;
    private List<LivingEntity> _list_of_recipients = new List<LivingEntity>(); // ontriggerstay2d only tracks moving entities, this list is used to check if any entities who has touched the circle are touching it on every physics frames 

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
        if (_lifespan <= 0)
        {
            Destroy(gameObject); 
        }

    }

    private void OnTriggerEnter2D(Collider2D collision) // living things added to list
    {
        if (collision.transform.GetComponent<LivingEntity>() != null)
        {
            _list_of_recipients.Add(collision.transform.GetComponent<LivingEntity>());
        }

    }

    private void OnTriggerExit2D(Collider2D collision) // living things removed from list 
    {
        if (collision.transform.GetComponent<LivingEntity>() != null)
        { 
            _list_of_recipients.Remove(collision.transform.GetComponent<LivingEntity>());
        }
    }



}
