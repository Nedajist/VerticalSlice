using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rectangle : LivingEntity
{
    [SerializeField] protected float _damage;
    [SerializeField] protected bool _thorns = true;
    [SerializeField] float _cooldown_before_damage = 0.3f; // also counts as i frames
    protected float _damage_timer;

    protected override void Start()
    {
        SetColor();
        _max_health = _health;
        _iframe_duration = 0.5f;
    }

    public override void ReceiveDamage(float damage)
    {

        //damage sfx
        _health -= damage;
        if (_health <= 0)
        {
            StartCoroutine(FadeAway(1f));
        }


        if (damage > 0)
        {
            _original_color = new Color(_original_color.r, _original_color.g, _original_color.b, _health / _max_health);
            _sprite.color = _original_color;
            StartCoroutine(FlashColor(0.1f, 0.1f, Color.red));
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CollisionDamageCheck(collision);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionDamageCheck(collision);
    }

    private void CollisionDamageCheck(Collision2D collision)
    {
        if (_damage_timer > 0) return;

        _damage_timer = _cooldown_before_damage;
        if (collision.transform.GetComponent<Projectile>() != null)
        {
            ReceiveDamage(collision.transform.GetComponent<Projectile>().damage);
        }

        if (collision.transform.GetComponent<Ally>() != null)
        {
            if (_thorns) collision.transform.GetComponent<Ally>().ReceiveDamage(_damage);
            ReceiveDamage(_damage);
        }

        if (collision.transform.GetComponent<Enemy>() != null)
        {
            if (_thorns) collision.transform.GetComponent<Enemy>().ReceiveDamage(_damage);
            ReceiveDamage(collision.transform.GetComponent<Enemy>().contact_damage);
        }


    }

}
