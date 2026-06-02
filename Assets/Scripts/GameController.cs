using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public enum GameState
{
    selecting,
    playing
}


public class GameController : MonoBehaviour
{
    [SerializeField] GameObject _mage_player;
    [SerializeField] GameObject _healer_player;
    [SerializeField] GameObject _tank_player;
    [SerializeField] GameObject _rogue_player;
    [SerializeField] GameObject _painter_player;


    [SerializeField] GameObject _mage_stc;
    [SerializeField] GameObject _healer_stc;
    [SerializeField] GameObject _tank_stc;
    [SerializeField] GameObject _rogue_stc;
    [SerializeField] GameObject _painter_stc;


    [SerializeField] GameObject _mage_add;
    [SerializeField] GameObject _healer_add;
    [SerializeField] GameObject _tank_add;
    [SerializeField] GameObject _rogue_add;
    [SerializeField] GameObject _painter_add;


    [SerializeField] GameObject _infectious_projectile_bundle;
    [SerializeField] GameObject _bar_canvas_UI;
    [SerializeField] GameObject _falling_rain;

    [SerializeField] public GameObject crosshair;
    [SerializeField] public AdditiveDistortion distortion_layer;
    [SerializeField] public GameObject boss_object;
    [SerializeField] private bool _boss_level;

    public static GameController instance;
    public Camera main_camera;
    public Boss boss;
    public UIController UI;
    public MusicController music_controller;
    public List<Ally> ally_list = new List<Ally>();

    public List<CloneTemplate> past_incarnation_list = new List<CloneTemplate>(); // list containing the hp, health, class, etc of past incarnatinos of the player
    public GameState current_gamestate;

    private Vector3 _player_starting_position = new Vector3(-1, -1, 0); // effectively this is (-1, -2.5, 0) as -1.5 is added to vector3.y immediately
    private GameObject _selected_player;
    private EnemySpawnBundle[] _list_of_spawners;

    public GameObject current_player;
    public int player_attempts = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        if (_boss_level) boss = boss_object.GetComponent<Boss>();
        else
        {
            FindSpawners();
        }
        UI = GameObject.FindObjectOfType<UIController>();
        main_camera = GameObject.FindObjectOfType<Camera>();
        music_controller = GameObject.FindObjectOfType<MusicController>();

        RefreshAllyList();
        TransitionGameState(GameState.selecting);
    }

    private void FixedUpdate()
    {
        Vector3 mouse_position = main_camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 true_position = new Vector3(mouse_position.x, mouse_position.y, 0);
        crosshair.transform.position = true_position;
    }

    public void AddNewClone(Vector3 starting_position, List<InputData> list_of_inputs, PlayerClass player_class, float speed, float max_health, float ability_1_cooldown, float ability_2_cooldown)
    {
        CloneTemplate _soul_data = ScriptableObject.CreateInstance<CloneTemplate>();
        _soul_data.starting_position = starting_position;


        List<InputData> new_input_list = new List<InputData>();
        for (int i = 0; i < list_of_inputs.Count; i++)
        {
            InputData new_input_data = ScriptableObject.CreateInstance<InputData>();
            new_input_data.SetValues(list_of_inputs[i]);
            new_input_list.Add(new_input_data);
        }

        _soul_data.list_of_inputs = new_input_list;

        _soul_data.player_class = player_class;
        _soul_data.speed = speed; // spd, hp, mhp all are currently redunant as the prefab provides those values 
        _soul_data.health = max_health;
        _soul_data.max_health = max_health;
        _soul_data.ability_1_cooldown = ability_1_cooldown;
        _soul_data.ability_2_cooldown = ability_2_cooldown;


        past_incarnation_list.Add(_soul_data);

    }

    public void SummonAllClones()
    {
        for (int i = 0; i <past_incarnation_list.Count; i++)
        {
            //Debug.Log(past_incarnation_list[i].list_of_inputs.Count());
        }

        for (int i = 0; i < past_incarnation_list.Count; i++)
        {
            
            GameObject instantiated_clone = transform.gameObject; // temporary assignment overruled in switch statement 
            CloneTemplate clone_data = past_incarnation_list[i];
            switch (past_incarnation_list[i].player_class)
            {
                case PlayerClass.Mage:
                    instantiated_clone = Instantiate(_mage_stc, Vector3.zero, Quaternion.identity);
                    break;
                case PlayerClass.Healer:
                    instantiated_clone = Instantiate(_healer_stc, Vector3.zero, Quaternion.identity);
                    break;
                case PlayerClass.Tank:
                    instantiated_clone = Instantiate(_tank_stc, Vector3.zero, Quaternion.identity);
                    break;
                case PlayerClass.Rogue:
                    instantiated_clone = Instantiate(_rogue_stc, Vector3.zero, Quaternion.identity);
                    break;
                case PlayerClass.Painter:
                    instantiated_clone = Instantiate(_painter_stc, Vector3.zero, Quaternion.identity);
                    break;
            }

            Ally clone = instantiated_clone.GetComponent<Ally>();
            clone.GiveLife(clone_data.starting_position, clone_data.list_of_inputs);
            clone.StartExecuting();

        }


    }
    public void RefreshAllyList()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        ally_list = new List<Ally>();
        for (int i = 0; i < playerObjects.Count(); i++)
        {
            ally_list.Add(playerObjects[i].GetComponent<Ally>());
        }
    }

    public void CullClones()
    {
        RefreshAllyList();

        for (int i = 0; i < ally_list.Count; i++)
        {
            if (i >= ally_list.Count)
            {
                break;
            }
            if (ally_list[i] == null)
            {
                ally_list.RemoveAt(i);
                i--;
            }
            else if (ally_list[i].is_clone == true)
            {
                Destroy(ally_list[i].transform.gameObject);
                ally_list.RemoveAt(i);
                i--;
            }
            else if (i>0 && ally_list[i].transform.GetInstanceID() == ally_list[i - 1].transform.GetInstanceID())
            {
                ally_list.RemoveAt(i);
                i--;
            }
        }
        ResetEnemies();
        ResetProjectiles();
        ResetAOEs();
        ResetObstacles();
    }

    void ResetEnemies() // also deletes all enemy projectiles 
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy"); // TEMPORARY
        for (int i = 0; i < enemyObjects.Count(); i++)
        {
            if (enemyObjects[i].GetComponent<Boss>() != null)
            {
                Boss boss = enemyObjects[i].GetComponent<Boss>();
                boss.ResetSelf();
            }
            else
            {
                Destroy(enemyObjects[i]);
            }
        }
    }

    void ResetProjectiles()
    {
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        for (int i = 0; i < projectiles.Count(); i++)
        {
            Destroy(projectiles[i]);
        }

        if (_boss_level)
        {
            Projectile[] bossChildProjectiles = boss.transform.GetComponentsInChildren<Projectile>(); // deletes hooks attached to boss
            foreach (Projectile projectile in bossChildProjectiles)
            {
                Destroy(projectile.transform.gameObject);
            }
        }

    }

    void ResetAOEs()
    {
        GameObject[] AOEs = GameObject.FindGameObjectsWithTag("AOE"); 
        for (int i = 0; i < AOEs.Count(); i++)
        {
            Destroy(AOEs[i]);
        }
    }

    void ResetObstacles()
    {
        GameObject[] AOEs = GameObject.FindGameObjectsWithTag("Obstacle");
        for (int i = 0; i < AOEs.Count(); i++)
        {
            Destroy(AOEs[i]);
        }
    }

    public void SummonRisen(Vector3 location, PlayerClass risenclass)
    {
        switch (risenclass)
        {
            case PlayerClass.Tank:
                GameObject _instantiated_tank = Instantiate(_tank_add, location, Quaternion.identity);
                break;
            case PlayerClass.Healer:
                GameObject _instantiated_healer = Instantiate(_healer_add, location, Quaternion.identity);
                break;
            case PlayerClass.Mage:
                GameObject _instantiated_mage = Instantiate(_mage_add, location, Quaternion.identity);
                break;
            case PlayerClass.Rogue:
                GameObject _instantiated_rogue = Instantiate(_rogue_add, location, Quaternion.identity);
                break;
            case PlayerClass.Painter:
                GameObject _instantiated_painter = Instantiate(_painter_add, location, Quaternion.identity);
                break;
        }
    }

    public void SummonInfectiousProjectileBundle(Vector3 location)
    {
        GameObject _instantiated_infectious_projectile_bundle = Instantiate(_infectious_projectile_bundle, location, Quaternion.identity);
    }

    public Vector2 RotateVector2(Vector2 inputVector2, float degrees)
    {
        float rotationRadians = degrees * Mathf.Deg2Rad;
        Vector2 newVector2 = Vector2.zero;
        newVector2.x = inputVector2.x * Mathf.Cos(rotationRadians) + inputVector2.y * Mathf.Sin(rotationRadians);
        newVector2.y = -inputVector2.x * Mathf.Sin(rotationRadians) + inputVector2.y * Mathf.Cos(rotationRadians);

        return (newVector2);
    }

    public void TransitionGameState(GameState new_state)
    {
        Debug.Log("SWITCHING STATE");
        switch (new_state)
        {
            case (GameState.selecting):
                UI.ShowSelectionScreen();

                if (_boss_level)
                {
                    _bar_canvas_UI.SetActive(false);
                    boss.transform.gameObject.SetActive(false);
                }
                else
                {
                    SetSpawnerStates(false);
                }

                _falling_rain.SetActive(false);
                CullClones();
                _player_starting_position -= new Vector3(0, 1.5f, 0);
                main_camera.GetComponent<MainCamera>().seconds_of_camera_shake = 0;
                music_controller.ResetSelf(); 
                if (_player_starting_position.y == -7)
                {
                    _player_starting_position = new Vector3(_player_starting_position.x + 1, -2.5f, 0);
                }

                break;
            case (GameState.playing):
                if (_boss_level)
                {
                    _bar_canvas_UI.SetActive(true);
                    boss.transform.gameObject.SetActive(true);
                    boss.ResetSelf();
                }
                else
                {
                    SetSpawnerStates(true);
                }

                _falling_rain.SetActive(true);
                SummonAllClones();
                current_player = Instantiate(_selected_player, transform.position, Quaternion.identity);
                current_player.GetComponent<Player>().starting_position = _player_starting_position;
                main_camera.transform.SetParent(current_player.transform);
                main_camera.transform.localPosition = new Vector3(0, 0, -1);
                distortion_layer.player = current_player;
                music_controller.PlayLevelMusic();
                player_attempts += 1;

                UI.HideSelectionScreen();

                break;
        }
        current_gamestate = new_state;

    }

    public void TankSelected()
    {
        _selected_player = _tank_player;
        TransitionGameState(GameState.playing);
    }

    public void HealerSelected()
    {
        _selected_player = _healer_player;
        TransitionGameState(GameState.playing);
    }

    public void MageSelected()
    {
        _selected_player = _mage_player;
        TransitionGameState(GameState.playing);
    }
    public void RogueSelected()
    {
        _selected_player = _rogue_player;
        TransitionGameState(GameState.playing);
    }
    public void PainterSelected()
    {
        _selected_player = _painter_player;
        TransitionGameState(GameState.playing);
    }

    private void SetSpawnerStates(bool enabled)
    {
        foreach (EnemySpawnBundle spawner in _list_of_spawners)
        {
            spawner.transform.gameObject.SetActive(enabled);
            if (enabled == false) spawner.ResetSelf();
        }
    }

    private void FindSpawners()
    {
        _list_of_spawners = GameObject.FindObjectsOfType<EnemySpawnBundle>();
        foreach (EnemySpawnBundle spawner in _list_of_spawners)
        {
            spawner.transform.gameObject.SetActive(false);
        }
    }
}