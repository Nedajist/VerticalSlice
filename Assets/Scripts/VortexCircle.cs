using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VortexCircle : Circle
{
    [SerializeField] private float _pull_strength = 4;

    private float _starting_scale;
    private float _safe_zone = 0.3f;
    private float _boss_resistance_factor = 0.5f;
    private float _projectile_resistance_factor = 2f;
    private void Start()
    {
        _starting_scale = transform.localScale.x;
        _max_lifespan = _lifespan;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < _list_of_recipients.Count; i++)
        {
            if (_list_of_recipients[i] != null) // loops through list of entities who are in the circle
            {
                LivingEntity recipient = _list_of_recipients[i];

                if (Vector3.Distance(recipient.transform.position, transform.position) > _safe_zone) // checks if they are within the eye of the hurricane. 
                {
                    Vector2 lineToSelf = transform.position - recipient.transform.position; // gets line FROM recipient TO this circle
                    lineToSelf = lineToSelf.normalized * _pull_strength;

                    if (recipient.GetComponent<Boss>() != null)
                    {
                        recipient.velocity_additive = lineToSelf * _boss_resistance_factor; // if recipient is the boss, strength of pull is reduced by 50%
                    }
                    else if (recipient.GetComponent<Projectile>() != null)
                    {
                        recipient.velocity_additive = lineToSelf * _projectile_resistance_factor; // if recipient is the projectile, strength of pull is strengthened
                    }
                    else
                    {
                        recipient.velocity_additive = lineToSelf;
                    }
                }
                else // if they are, frees them from forced movement. This reduces rapid movements. 
                {
                    recipient.velocity_additive = Vector2.zero;
                }
            }
        }

        _lifespan -= Time.deltaTime;

        _sprite_renderer.color = new Color(_sprite_renderer.color.r, _sprite_renderer.color.g, _sprite_renderer.color.b, _lifespan / _max_lifespan); // circle becomes more transparent each frame

        if (_lifespan <= 0)
        {
            Destroy(gameObject);
        }

    }



    protected override void OnTriggerEnter2D(Collider2D collision) // living things added to list
    {
        if (collision.transform.GetComponent<LivingEntity>() != null)
        {
            _list_of_recipients.Add(collision.transform.GetComponent<LivingEntity>());

            if (collision.transform.GetComponent<Projectile>() != null)
            {
                collision.transform.GetComponent<Projectile>().FreezeVelocity();
            }

        }

    }

    protected override void OnTriggerExit2D(Collider2D collision) // living things removed from list 
    {
        if (collision.transform.GetComponent<LivingEntity>() != null)
        {
            LivingEntity entity = collision.transform.GetComponent<LivingEntity>();
            entity.velocity_additive = Vector2.zero;

            _list_of_recipients.Remove(collision.transform.GetComponent<LivingEntity>());
        }


    }


}


