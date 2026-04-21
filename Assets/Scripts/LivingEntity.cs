using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    [SerializeField] protected float _speed;
    [SerializeField] protected float _health = 100;
    [SerializeField] protected float _max_health = 100;

    public bool infected = false;
    public virtual void ReceiveHealing(float amount)
    {
        _health += amount;
        _health = Mathf.Clamp(_health, 0, _max_health);
        Debug.Log("HEALED FOR " + amount);
    }

    public virtual void ReceiveDamage(float amount)
    {
        _health -= amount;
        if (_health<= 0)
        {
            Destroy(gameObject);
        }
    }

    public float GetHealth()
    {
        return (_health);
    }

    public float GetMaxHealth()
    {
        return (_max_health);
    }

}
