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

    [SerializeField] private GameObject _mage_symbol;
    [SerializeField] private GameObject _healer_symbol;
    [SerializeField] private GameObject _tank_symbol;

    private float _current_health;
    private Vector3 _current_class_symbol_position = new Vector3(10, -45, 0);

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

    public void ShowSelectionScreen() // called when entering selecting phase
    {
        _character_select_menu.SetActive(true);
        _current_class_symbol_position += new Vector3(40, 0, 0);
    }

    public void HideSelectionScreen() // called when entering playing phase
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

    public void DisplayMageClassSymbol()
    {
        GameObject _instantiated_class_symbol = Instantiate(_mage_symbol, Vector3.zero, Quaternion.identity);
        _instantiated_class_symbol.transform.SetParent(_character_select_menu.transform);
        _instantiated_class_symbol.transform.localPosition = _current_class_symbol_position;
    }

    public void DisplayHealerClassSymbol()
    {
        GameObject _instantiated_class_symbol = Instantiate(_healer_symbol, Vector3.zero, Quaternion.identity);
        _instantiated_class_symbol.transform.SetParent(_character_select_menu.transform);
        _instantiated_class_symbol.transform.localPosition = _current_class_symbol_position;
    }
    

    public void DisplayTankClassSymbol()
    {
        GameObject _instantiated_class_symbol = Instantiate(_tank_symbol, Vector3.zero, Quaternion.identity);
        _instantiated_class_symbol.transform.SetParent(_character_select_menu.transform);
        _instantiated_class_symbol.transform.localPosition = _current_class_symbol_position;
    }



}

