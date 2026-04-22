using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{

    [SerializeField] private Slider _health_bar;
    [SerializeField] private Slider _lazy_bar;
    [SerializeField] private float _rate_of_bar_change;
    [SerializeField] private GameObject _character_select_menu;

    private float _current_health;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.instance.current_gamestate == GameState.playing)
        {
            UpdateBossHealthBar();
        }
    }

    public void ShowSelectionScreen()
    {
        _character_select_menu.SetActive(true);
    }

    public void HideSelectionScreen()
    {
        _character_select_menu.SetActive(false);
    }

    private void UpdateBossHealthBar()
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

