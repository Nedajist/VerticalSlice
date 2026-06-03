using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LivingEntity : MonoBehaviour
{
    [SerializeField] protected float _speed;
    [SerializeField] protected float _health = 100;
    [SerializeField] protected float _max_health = 100;
    [SerializeField] protected SpriteRenderer _sprite;
    [SerializeField] protected AudioSource _audio_player;
    [SerializeField] List<AudioClip> _damage_SFX_list;
    [SerializeField] List<AudioClip> _death_SFX_list;

    protected float _iframe_duration = 0.25f;
    protected float _i_frames = 0f;

    public Dictionary<VelocityAdditiveType, VelocityAdditive> velocity_additive_dict = new Dictionary<VelocityAdditiveType, VelocityAdditive>();

    protected Color _original_color;

    protected virtual void Start()
    {
        SetColor();
    }

    protected virtual void SetColor()
    {
        if (transform.GetComponent<SpriteRenderer>() != null)
        {
            _original_color = _sprite.color;
        }
    }

    protected void AddVelocityAdditives()
    {
        Rigidbody2D rb = transform.GetComponent<Rigidbody2D>();
        foreach (KeyValuePair<VelocityAdditiveType, VelocityAdditive> pair in velocity_additive_dict)
        {
            rb.velocity += pair.Value.GetTrueAdditive();
        }
    }

    public virtual void ReceiveHealing(float amount)
    {
        _health += amount;
        _health = Mathf.Clamp(_health, 0, _max_health);
        StartCoroutine(FlashColor(0.08f, 0.08f, Color.green));
        //Debug.Log("HEALED FOR " + amount);
    }

    public virtual void ReceiveDamage(float amount)
    {
        if (_i_frames > 0)
        {
            return;
        }
        _i_frames = _iframe_duration;

        _health -= amount;
        ShakeHealthBar();
        StartCoroutine(FlashColor(0.1f, 0.1f, Color.red));
        if (_health <= 0)
        {
            StartCoroutine(FadeAway(1f));
        }
        else PlayDamageSFX(); // and death SFX plays in FadeAway
    }

    public float GetHealth()
    {
        return (_health);
    }

    public float GetMaxHealth()
    {
        return (_max_health);
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public void SetSpeed(float new_speed)
    {
        _speed = new_speed;
    }

    protected virtual IEnumerator FlashColor(float ease_in_duration, float ease_out_duration, Color _new_color)
    {
        float _ease_in_timer = ease_in_duration;
        float _ease_out_timer = ease_out_duration;

        while (_ease_in_timer > 0)
        {
            _ease_in_timer -= Time.fixedDeltaTime;
            _sprite.color = Color.Lerp(_original_color, _new_color, 1 - (_ease_in_timer / ease_in_duration));
            yield return new WaitForFixedUpdate();
        }

        while (_ease_out_timer > 0)
        {
            _ease_out_timer -= Time.fixedDeltaTime;
            _sprite.color = Color.Lerp(_new_color, _original_color, 1 - (_ease_out_timer / ease_out_duration));
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    protected virtual void ShakeHealthBar()
    {
        if (transform.GetComponent<HealthBar>() == null) return;
        transform.GetComponent<HealthBar>().StartCoroutine(transform.GetComponent<HealthBar>().TempSizeChange(0.15f, 0.3f, 0.3f));
    }

    protected virtual IEnumerator FadeAway(float duration) // turns sprite red, moves alpha to 0, destroys gameobject
    {
        float timer = duration;
        transform.GetComponent<Rigidbody2D>().simulated = false; // disabled rigidbody
        PlayDeathSFX();
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            _sprite.color = Color.Lerp(_sprite.color, new Color(Color.red.r, Color.red.b, Color.red.g, timer / duration), timer / duration);
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }

    protected void PlayDamageSFX()
    {
        if (_damage_SFX_list.Count == 0) return;
        AudioClip selected_clip = _damage_SFX_list[Random.Range(0, _damage_SFX_list.Count)];
        _audio_player.clip = selected_clip;
        _audio_player.Play();
    }

    protected void PlayDeathSFX()
    {
        if (_death_SFX_list.Count == 0) return;
        AudioClip selected_clip = _death_SFX_list[Random.Range(0, _death_SFX_list.Count)];
        _audio_player.clip = selected_clip;
        _audio_player.Play();
    }

}
