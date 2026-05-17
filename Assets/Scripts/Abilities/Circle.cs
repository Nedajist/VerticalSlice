using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    [SerializeField] protected float _lifespan;
    [SerializeField] protected SpriteRenderer _sprite_renderer;
    protected List<LivingEntity> _list_of_recipients = new List<LivingEntity>();

    protected float _max_lifespan;



    protected virtual void OnTriggerEnter2D(Collider2D collision) // living things added to list
    {
        if (collision.transform.GetComponent<LivingEntity>() != null)
        {
            _list_of_recipients.Add(collision.transform.GetComponent<LivingEntity>());
        }

    }

    protected virtual void OnTriggerExit2D(Collider2D collision) // living things removed from list 
    {
        if (collision.transform.GetComponent<LivingEntity>() != null)
        {
            _list_of_recipients.Remove(collision.transform.GetComponent<LivingEntity>());
        }
    }

}
