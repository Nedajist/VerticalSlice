using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableRectangle : Rectangle
{
    [SerializeField] BoxCollider2D _collider;
    [SerializeField] float _maxArea;


    public bool placed = false;
    public bool placeable = false;


    // Update is called once per frame


    public void Place()
    {
        if (placeable == false)
        {
            Destroy(gameObject);
            return;
        }

        _sprite.color = new Color(_original_color.r, _original_color.g, _original_color.b, 1);
        _original_color = _sprite.color;
        _collider.isTrigger = false;
        placed = true;
    }

    private void FixedUpdate()
    {
        _damage_timer -= Time.fixedDeltaTime;
        if (placed == false)
        {
            List<Collider2D> hits = new List<Collider2D>();
            ContactFilter2D filter = new ContactFilter2D();
            filter.NoFilter();

            Physics2D.OverlapCollider(_collider, filter, hits);

            if (Mathf.Abs(transform.localScale.x) * Mathf.Abs(transform.localScale.y) > _maxArea || hits.Count > 0)
            {
                _sprite.color = new Color(Color.red.r, Color.red.g, Color.red.b, _original_color.a);
                placeable = false;
            }
            else
            {
                _sprite.color = _original_color;
                placeable = true;
            }
        }
    }



}
