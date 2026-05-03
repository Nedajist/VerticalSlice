using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueAOECircle : Circle
{
    [SerializeField] float _damage;
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] public float _max_scale = 2.5f;
    [SerializeField] private float _speed;

    private GameObject _target;
    public GameObject originator;

    // Start is called before the first frame update
    void Start()
    {
        _max_lifespan = _lifespan;
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        _lifespan -= Time.deltaTime;
        _sprite_renderer.color = new Color(_sprite_renderer.color.r, _sprite_renderer.color.g, _sprite_renderer.color.b, 1 - _lifespan / _max_lifespan); // circle becomes less transparent each frame
        transform.localScale = new Vector3(_max_scale * (1 - _lifespan / _max_lifespan), _max_scale * (1 - _lifespan / _max_lifespan), 0); // scale grows

        if (_lifespan <= 0)
        {
            for (int i = 0; i < _list_of_recipients.Count; i++)
            {
                if (_list_of_recipients[i] != null)
                {
                    Debug.Log(_list_of_recipients[i].name);
                    _list_of_recipients[i].ReceiveDamage(_damage);

                }
            }

            Destroy(gameObject);
        }

        if (_target != null)
        {
            _rb.velocity = _target.transform.position - transform.position;
            _rb.velocity = _rb.velocity.normalized * _speed;
        }

    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.GetComponent<LivingEntity>() != null)
        {
            _list_of_recipients.Add(collision.transform.GetComponent<LivingEntity>());
            if (collision.transform != originator.transform && _target == null)
            {
                _target = collision.transform.gameObject;
            }

        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.GetComponent<LivingEntity>() != null)
        {
            _list_of_recipients.Remove(collision.transform.GetComponent<LivingEntity>());
        }
    }


}
