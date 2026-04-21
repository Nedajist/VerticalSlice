using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisenTank : RisenEntity
{
    [SerializeField] float _scale_growth_additive_per_second;
    [SerializeField] GameObject _death_square;

    private void FixedUpdate()
    {
        _targeting_timer -= Time.fixedDeltaTime;

        float _scale_additive = _scale_growth_additive_per_second * Time.fixedDeltaTime;

        transform.localScale = new Vector3(transform.localScale.x + _scale_additive, transform.localScale.y + _scale_additive, 0);

        if (_targeting_timer <= 0)
        {
            TargetStrongestPlayer();
            _targeting_timer = _targeting_cooldown;
        }

        MoveToTarget(); // moves to _target_player's position 

    }

    public override void ReceiveDamage(float amount)
    {
        _health -= amount;
        if (_health <= 0)
        {
            GameObject _instantiated_death_square = Instantiate(_death_square, transform.position, Quaternion.identity);
            _instantiated_death_square.GetComponent<BossAOECircle>()._max_scale = transform.localScale.x;
            Destroy(gameObject);
        }

    }

}