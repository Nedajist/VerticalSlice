using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentRectangle : Rectangle
{
    [SerializeField] Rigidbody2D _rb; // tank sword won't collide with this if it has a collider instead of a rigidbody2d 
    [SerializeField] Color _true_color; // to accomodate for flash_color defaulting to original color 
    protected override void Start()
    {
        _sprite.color = _true_color;
        base.Start();
    }

    private void FixedUpdate()
    {
        _damage_timer -= Time.fixedDeltaTime;
    }

    public override void ReceiveDamage(float damage)
    {
        _health -= damage;
        if (damage > 0)
        {
            _original_color = new Color(_original_color.r, _original_color.g, _original_color.b, _health / _max_health);
            _sprite.color = _original_color;
            StartCoroutine(FlashOpen(0.35f, 0.35f, 0.8f));
        }

        if (_health <= 0)
        {
            _sprite.enabled = false;
            _rb.simulated = false;
        }

    }

    private IEnumerator FlashOpen(float ease_in_duration, float ease_out_duration, float holding_period)
    {
        float _ease_in_timer = ease_in_duration;
        float _ease_out_timer = ease_out_duration;
        float _holding_timer = holding_period;
        Color _new_color = new Color( (_original_color.r + 1) / 2, (_original_color.g + 1 ) / 2, (_original_color.b + 1 ) / 2, _original_color.a / 2);
        

        while (_ease_in_timer > 0)
        {
            _ease_in_timer -= Time.fixedDeltaTime;
            _sprite.color = Color.Lerp(_original_color, _new_color, 1 - (_ease_in_timer / ease_in_duration));
            yield return new WaitForFixedUpdate();
        }

        _rb.simulated = false; // characters can walk through after sprite fades to full transparency

        Collider2D hit = new Collider2D();
        
        while (_holding_timer > 0 || hit != null) // wall won't re-form until timer expires AND nothing is colliding it 
        {
            _holding_timer -= Time.fixedDeltaTime;
            hit = Physics2D.OverlapBox(transform.position, new Vector2(transform.lossyScale.x, transform.lossyScale.y), transform.eulerAngles.z);
            yield return new WaitForFixedUpdate();
        }



        _rb.simulated = true; // characters can no longer walk through when slide begins to fade back in 

        while (_ease_out_timer > 0)
        {
            _ease_out_timer -= Time.fixedDeltaTime;
            _sprite.color = Color.Lerp(_new_color, _original_color, 1 - (_ease_out_timer / ease_out_duration));
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }


    public void ResetSelf()
    {
        StopAllCoroutines();
        _sprite.color = _true_color;
        _sprite.enabled = true;
        _rb.simulated = true;
        _health = _max_health;
    }
}
