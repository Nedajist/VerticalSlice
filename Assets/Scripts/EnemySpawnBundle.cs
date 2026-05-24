using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnBundle : MonoBehaviour
{
    [SerializeField] List<GameObject> _spawn_prefab_list;
    [SerializeField] float second_to_spawn; // the second at which the spawner begins to activate
    [SerializeField] float spawn_interval = 1; // after spawning, the delay between each spawn
    [SerializeField] float spawn_randomization_factor = 3;

    private float _time = 0;
    private float _spawn_interval_timer = 0;
    private int _spawn_index = 0;
    private bool _spawning = false;
    private List<Vector3> _spawn_position_list = new List<Vector3>();

    private void Start()
    {
        Debug.Log("STARTED"); 
        for (int i = 0; i < _spawn_prefab_list.Count; i++)
        {
            _spawn_position_list.Add(new Vector3(transform.position.x + Random.Range(-spawn_randomization_factor, spawn_randomization_factor), transform.position.y + Random.Range(-spawn_randomization_factor, spawn_randomization_factor))); // spawn positions are determined on the first run, so remain deterministic within sessions 
        }
    }


    private void FixedUpdate()
    {
        _time += Time.fixedDeltaTime;
        if (_time >= second_to_spawn)
        {
            _spawning = true;
        }

        if (_spawning)
        {
            _spawn_interval_timer -= Time.deltaTime;
            if (_spawn_index >= _spawn_prefab_list.Count)
            {
                _spawning = false;
                return;
            }

            if (_spawn_interval_timer <= 0)
            {
                Vector3 spawn_position = _spawn_position_list[_spawn_index];
                GameObject instantiated_spawn = Instantiate(_spawn_prefab_list[_spawn_index], spawn_position, Quaternion.identity);
                _spawn_index += 1;
                _spawn_interval_timer = spawn_interval;
            }
        }


    }

    public void ResetSelf()
    {
        _time = 0;
        _spawn_interval_timer = 0;
        _spawn_index = 0;
        _spawning = false;
    }

}
