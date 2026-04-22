using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [SerializeField] GameObject _mage_stc;
    [SerializeField] GameObject _healer_stc;
    [SerializeField] GameObject _tank_stc;

    [SerializeField] GameObject _mage_add;
    [SerializeField] GameObject _healer_add;
    [SerializeField] GameObject _tank_add;

    [SerializeField] GameObject _infectious_projectile_bundle;


    public static GameController instance;
    public Boss boss;
    public List<Ally> ally_list = new List<Ally>();

    public List<CloneTemplate> past_incarnation_list = new List<CloneTemplate>(); // list containing the hp, health, class, etc of past incarnatinos of the player



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
        boss = GameObject.FindObjectOfType<Boss>();
        RefreshAllyList();

    }

    private void FixedUpdate()
    {
        
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
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile"); // TEMPORARY
        for (int i = 0; i < projectiles.Count(); i++)
        {
            Destroy(projectiles[i]);
        }
    }

    void ResetAOEs()
    {
        GameObject[] AOEs = GameObject.FindGameObjectsWithTag("AOE"); // TEMPORARY
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}