using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider _health_bar;
    [SerializeField] private Slider _lazy_bar;
    [SerializeField] private float _rate_of_change;
    [SerializeField] private Image _infection_sign;


    private LivingEntity _living_entity;
    private float _current_health;
    private float _max_health;



    // Start is called before the first frame update
    void Start()
    {
        _living_entity = transform.GetComponent<LivingEntity>();
        _max_health = _living_entity.GetMaxHealth();
        _current_health = _living_entity.GetHealth();

        _health_bar.maxValue = _max_health;
        _health_bar.value = _current_health;

        _lazy_bar.maxValue = _max_health;
        _lazy_bar.value = _current_health;
    }

    // Update is called once per frame
    void Update()
    {
        _current_health = _living_entity.GetHealth();
        _health_bar.value = _current_health;


        if (Mathf.Abs(_lazy_bar.value - _health_bar.value) < 1)
        {
            return;
        }

        if (_lazy_bar.value > _health_bar.value)
        {
            _lazy_bar.value -= _rate_of_change * Time.deltaTime;
        }
        if (_lazy_bar.value < _health_bar.value)
        {
            _lazy_bar.value = _health_bar.value;
        }
    }
}
