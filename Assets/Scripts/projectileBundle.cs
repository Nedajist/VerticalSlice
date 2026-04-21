using UnityEngine;

public class projectileBundle : MonoBehaviour
{
    [SerializeField] GameObject _projectile;
    [SerializeField] float _distance_from_center;
    [SerializeField] int _projectile_count;
    [SerializeField] bool _clockwise;
    [SerializeField] float _seconds_between_projectile_spawn = 0;
    [SerializeField] bool _fired_by_boss;
    [SerializeField] bool _infectious;
    private GameObject _instantiated_projectile;
    private float _projectile_timer = 0;
    private int _summoned_projectile_count = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_seconds_between_projectile_spawn == 0)
        {
            for (int i = 0; i < _projectile_count; i++)
            {
                float angle = (360 / _projectile_count) * i;
                float _spawn_x = transform.position.x + _distance_from_center * Mathf.Cos(angle * Mathf.Deg2Rad);
                float _spawn_y = transform.position.y + _distance_from_center * Mathf.Sin(angle * Mathf.Deg2Rad);
                _projectile.transform.GetComponent<Projectile>().starting_angle = angle;
                _projectile.transform.GetComponent<Projectile>().clockwise = _clockwise;
                if (_fired_by_boss == true)
                {
                    _projectile.gameObject.layer = LayerMask.NameToLayer("Boss Projectile");
                }
                if (_infectious == true)
                {
                    _projectile.transform.GetComponent<Projectile>().infectious_target_index = i;
                }
                _instantiated_projectile = Instantiate(_projectile, new Vector3(_spawn_x, _spawn_y, 0), Quaternion.identity);

            }
            Destroy(gameObject);
        }

    }

    private void FixedUpdate()
    {
        _projectile_timer -= Time.fixedDeltaTime;
        if (_projectile_timer <= 0)
        {
            _projectile_timer = _seconds_between_projectile_spawn;
            float angle = (360 / _projectile_count) * _summoned_projectile_count;
            float _spawn_x = transform.position.x + _distance_from_center * Mathf.Cos(angle * Mathf.Deg2Rad);
            float _spawn_y = transform.position.y + _distance_from_center * Mathf.Sin(angle * Mathf.Deg2Rad);
            _projectile.transform.GetComponent<Projectile>().starting_angle = angle;
            _projectile.transform.GetComponent<Projectile>().clockwise = _clockwise;

            if (_fired_by_boss == true)
            {
                _projectile.gameObject.layer = LayerMask.NameToLayer("Boss Projectile");
            }

            if (_infectious == true)
            {
                _projectile.transform.GetComponent<Projectile>().infectious_target_index = _summoned_projectile_count;
            }

            _instantiated_projectile = Instantiate(_projectile, new Vector3(_spawn_x, _spawn_y, 0), Quaternion.identity);
            _summoned_projectile_count += 1;

            if (_summoned_projectile_count == _projectile_count)
            {
                Destroy(gameObject);
            }

        }

    }


}
