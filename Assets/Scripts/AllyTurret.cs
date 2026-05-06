using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AllyTurret : Ally
{
    [SerializeField] private GameObject _turret_bolt;
    [SerializeField] private float _seconds_between_projectiles;
    [SerializeField] private float _detection_range;
    private float _projectile_timer;

    protected override void Start()
    {
        SetColor();
    }

    protected override void FixedUpdate()
    {
        _i_frames -= Time.fixedDeltaTime;
        _projectile_timer -= Time.fixedDeltaTime;
        if (_projectile_timer <= 0)
        {
            List<Enemy> target_list = new List<Enemy>();
            _projectile_timer = _seconds_between_projectiles;
            RaycastHit2D[] collider_list = Physics2D.CircleCastAll(transform.position, _detection_range, Vector2.zero);
            foreach (RaycastHit2D hit in collider_list)
            {
                if (hit.transform.GetComponent<Enemy>() != null)
                {
                    target_list.Add(hit.transform.GetComponent<Enemy>());
                }
            }

            if (target_list.Count == 0)
            {
                return;
            }
            Enemy nearest_target = target_list[0];
            foreach (Enemy target in target_list)
            {
                if (Vector3.Distance(transform.position, target.transform.position) < Vector3.Distance(transform.position, nearest_target.transform.position))
                {
                    nearest_target = target;
                }
            }

            Vector3 lineToTarget = nearest_target.transform.position - transform.position;

            RaycastHit2D[] target_in_line_list = Physics2D.RaycastAll(transform.position, lineToTarget);
            foreach (RaycastHit2D hit in target_in_line_list)
            {
                if (hit.transform.GetComponent<Enemy>() != null) // exits loop if enemy will be first one hit
                {
                    break;
                }
                if (hit.transform.GetComponent<Ally>() != null) // won't fire if ally is in path 
                {
                    if (hit.transform == transform) // ignores itself 
                    {
                        continue;
                    }
                    return;
                }
            }



            lineToTarget = lineToTarget.normalized * 1.5f;
            GameObject instantiated_projectile = Instantiate(_turret_bolt, transform.position + lineToTarget, Quaternion.identity);
            instantiated_projectile.GetComponent<Projectile>().TargetLivingEntity(nearest_target);
        }
    }
}
