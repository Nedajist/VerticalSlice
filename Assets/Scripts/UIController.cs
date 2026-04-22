using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{

    [SerializeField] private Slider _health_bar;
    [SerializeField] private Slider _lazy_bar;
    [SerializeField] private float _rate_of_bar_change;

    private float _current_health;
    private float _max_health;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        _current_health = GameController.instance.boss.GetHealth();
        _health_bar.value = _current_health;


        if (Mathf.Abs(_lazy_bar.value - _health_bar.value) < 1)
        {
            return;
        }

        if (_lazy_bar.value > _health_bar.value)
        {
            _lazy_bar.value -= _rate_of_bar_change * Time.deltaTime;
        }
        if (_lazy_bar.value < _health_bar.value)
        {
            _lazy_bar.value = _health_bar.value;
        }

    }
}

