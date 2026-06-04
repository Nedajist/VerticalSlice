using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{

    [SerializeField] private Slider _health_bar;
    [SerializeField] private Slider _lazy_bar;
    [SerializeField] private Slider _ability_1_bar;
    [SerializeField] private Slider _ability_2_bar;
    [SerializeField] private Image _ability_1_image;
    [SerializeField] private Image _ability_2_image;
    [SerializeField] public GameObject ability_1_container;
    [SerializeField] public GameObject ability_2_container;

    [SerializeField] private float _rate_of_bar_change;
    [SerializeField] private GameObject _character_select_menu;

    [SerializeField] private GameObject _mage_symbol;
    [SerializeField] private GameObject _healer_symbol;
    [SerializeField] private GameObject _tank_symbol;
    [SerializeField] private GameObject _rogue_symbol;
    [SerializeField] private GameObject _painter_symbol;

    [SerializeField] private AudioSource _UI_audio_manager;
    [SerializeField] private AudioClip _return_SFX;
    [SerializeField] private AudioClip _tank_selected_SFX;
    [SerializeField] private AudioClip _mage_selected_SFX;
    [SerializeField] private AudioClip _healer_selected_SFX;
    [SerializeField] private AudioClip _rogue_selected_SFX;
    [SerializeField] private AudioClip _painter_selected_SFX;


    private float _past_character_count = 0;
    private Vector3 _current_class_symbol_position = new Vector3(10, -45, 0);

    private Player _active_player;
    private Color abilityBarColor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.instance.current_gamestate == GameState.playing)
        {
            UpdateAbilityHealthBars();
        }
    }

    public void ShowSelectionScreen() // called when entering selecting phase
    {
        _character_select_menu.SetActive(true);
        _current_class_symbol_position += new Vector3(40, 0, 0);
        _ability_1_bar.transform.gameObject.SetActive(false);
        _ability_2_bar.transform.gameObject.SetActive(false);
        if (_past_character_count > 1 && _past_character_count % 10 == 0)
        {
            _current_class_symbol_position += new Vector3(0, -40, 0);
            _current_class_symbol_position = new Vector3(50, _current_class_symbol_position.y, 0);
        }
        _past_character_count += 1;
    }

    public void HideSelectionScreen() // called when entering playing phase
    {
        _character_select_menu.SetActive(false);
        _ability_1_bar.transform.gameObject.SetActive(true);
        _ability_2_bar.transform.gameObject.SetActive(true);
        _active_player = GameController.instance.current_player.GetComponent<Player>();
        _ability_1_bar.maxValue = _active_player.GetAbility1Cooldown(); // ability bar max values set
        _ability_2_bar.maxValue = _active_player.GetAbility2Cooldown();

    }


    private void UpdateAbilityHealthBars()
    {
        _ability_1_bar.value = _ability_1_bar.maxValue - _active_player.GetAbility1Timer();
        _ability_2_bar.value = _ability_2_bar.maxValue - _active_player.GetAbility2Timer();

        _ability_1_image.color = Color.Lerp(Color.black, abilityBarColor, _ability_1_bar.value / _ability_1_bar.maxValue);
        _ability_2_image.color = Color.Lerp(Color.black, abilityBarColor, _ability_2_bar.value / _ability_2_bar.maxValue);

    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(PlayReturnSFXThenReturn(0.5f));
    }

    public void DisplayMageClassSymbol() // displays the mage symbol in previous answers 
    {
        GameObject _instantiated_class_symbol = Instantiate(_mage_symbol, Vector3.zero, Quaternion.identity);
        _instantiated_class_symbol.transform.SetParent(_character_select_menu.transform);
        _instantiated_class_symbol.transform.localPosition = _current_class_symbol_position;
        abilityBarColor = new Color(0.055f, 0.702f, 0.949f, 1f); // mana blue
        _UI_audio_manager.clip = _mage_selected_SFX;
        _UI_audio_manager.Play();
    }
    public void DisplayHealerClassSymbol()
    {
        GameObject _instantiated_class_symbol = Instantiate(_healer_symbol, Vector3.zero, Quaternion.identity);
        _instantiated_class_symbol.transform.SetParent(_character_select_menu.transform);
        _instantiated_class_symbol.transform.localPosition = _current_class_symbol_position;
        abilityBarColor = new Color(0.039f, 1f, 0.463f, 1f); // healer green
        _UI_audio_manager.clip = _healer_selected_SFX;
        _UI_audio_manager.Play();
    }


    public void DisplayTankClassSymbol()
    {
        GameObject _instantiated_class_symbol = Instantiate(_tank_symbol, Vector3.zero, Quaternion.identity);
        _instantiated_class_symbol.transform.SetParent(_character_select_menu.transform);
        _instantiated_class_symbol.transform.localPosition = _current_class_symbol_position;
        abilityBarColor = new Color(0.7f, 0.231f, 0.231f, 1f); // reddish brown
        _UI_audio_manager.clip = _tank_selected_SFX;
        _UI_audio_manager.Play();
    }

    public void DisplayRogueClassSymbol()
    {
        GameObject _instantiated_class_symbol = Instantiate(_rogue_symbol, Vector3.zero, Quaternion.identity);
        _instantiated_class_symbol.transform.SetParent(_character_select_menu.transform);
        _instantiated_class_symbol.transform.localPosition = _current_class_symbol_position;
        abilityBarColor = new Color(1f, 0.612f, 0.039f, 1f); // reddish brown
        _UI_audio_manager.clip = _rogue_selected_SFX;
        _UI_audio_manager.Play();
    }

    public void DisplayPainterClassSymbol()
    {
        GameObject _instantiated_class_symbol = Instantiate(_painter_symbol, Vector3.zero, Quaternion.identity);
        _instantiated_class_symbol.transform.SetParent(_character_select_menu.transform);
        _instantiated_class_symbol.transform.localPosition = _current_class_symbol_position;
        abilityBarColor = new Color(0.247f, 0.145f, 0.722f, 1f); // reddish brown
        _UI_audio_manager.clip = _painter_selected_SFX;
        _UI_audio_manager.Play();
    }

    public IEnumerator PlayReturnSFXThenReturn(float wait_duration)
    {
        _UI_audio_manager.clip = _return_SFX;
        _UI_audio_manager.Play();
        yield return new WaitForSeconds(wait_duration);
        SceneManager.LoadScene(0);
    }

}

