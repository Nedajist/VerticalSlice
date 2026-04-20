using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSword : MonoBehaviour
{

    [SerializeField] private float _angles_per_second = 10; // ensure that only 1 sword can exist per tank at a time. Make sure this is high enough. 
    [SerializeField] private float _radius;
    [SerializeField] public GameObject center;
    


    
    public float target_angles_traveled = 150; // length of the sweep arc
    private float _angles_traveled = 0;
    private float _damage = 30f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (center == null)
        {
            Destroy(gameObject);
            return;
        }

        if (_angles_traveled < target_angles_traveled)
        {
            _angles_traveled += Time.deltaTime * _angles_per_second;
            Quaternion _rotation_quaternion = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + Time.deltaTime * _angles_per_second);
            transform.rotation = _rotation_quaternion;


            float _circle_x = _radius * Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad); // circle made with center's position and radius 
            float _circle_y = _radius * Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad);

            transform.position = center.transform.position + new Vector3(_circle_x, _circle_y, 0);

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LivingEntity target_entity = collision.GetComponent<LivingEntity>();

        if (collision.GetComponent<Projectile>() != null)
        {
            Destroy(collision.gameObject);
            return;
        }

        else if (target_entity != null && target_entity.GetInstanceID() != center.GetComponent<LivingEntity>().GetInstanceID())
        {
            collision.GetComponent<LivingEntity>().ReceiveDamage(_damage);
        }


    }
}
