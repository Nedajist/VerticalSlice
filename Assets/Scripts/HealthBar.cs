using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _health_bar;
    [SerializeField] private Slider _lazy_bar;
    [SerializeField] private float _rate_of_change;
    [SerializeField] private Image _infection_sign;
    [SerializeField] private GameObject _bar_canvas;


    private LivingEntity _living_entity;
    private float _current_health;
    private float _max_health;
    private Vector3 _original_scale;

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
        _original_scale = _bar_canvas.transform.localScale;
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

    public IEnumerator TempSizeChange(float ease_in, float ease_out, float scaleIncrease)
    {
        float duration = ease_in;
        while (duration > 0)
        {
            duration -= Time.fixedDeltaTime;
            _bar_canvas.transform.localScale = Vector3.Lerp(_original_scale, new Vector3(_original_scale.x + scaleIncrease, _original_scale.y + scaleIncrease, 0), 1 - duration / ease_in);
            yield return new WaitForFixedUpdate();
        }

        duration = ease_out;
        Vector3 newScale = _bar_canvas.transform.localScale;
        while (duration > 0)
        {
            duration -= Time.fixedDeltaTime;
            _bar_canvas.transform.localScale = Vector3.Lerp(newScale, _original_scale, 1 - duration / ease_out);
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }


}
