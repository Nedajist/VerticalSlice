using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    [SerializeField] protected float _speed;
    [SerializeField] protected float _health = 100;
    [SerializeField] protected float _max_health = 100;
    public virtual void ReceiveHealing(float amount)
    {
        _health += amount;
        _health = Mathf.Clamp(_health, 0, _max_health);
        Debug.Log("HEALED FOR " + amount);
    }
}
