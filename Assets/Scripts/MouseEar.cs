using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class MouseEar : LivingEntity
{
    protected override void Start()
    {
        return; // can't set color in start since start runs when mouse is disabled?
    }


    private void Awake()
    {
        base.Start();
    }


    private void FixedUpdate()
    {
        _i_frames -= Time.fixedDeltaTime;
    }

    public void ResetMouseEar()
    {
        StopAllCoroutines();
        _health = _max_health;
        _sprite.color = _original_color;
        gameObject.SetActive(true);
    }

    protected override IEnumerator FadeAway(float duration)
    {
        float timer = duration;
        // transform.GetComponent<Rigidbody2D>().simulated = false; // Mouse ear doesn't have a rigidbody, only collider 
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            _sprite.color = Color.Lerp(_sprite.color, new Color(Color.red.r, Color.red.b, Color.red.g, timer / duration), timer / duration);
            yield return new WaitForFixedUpdate();
        }
        gameObject.SetActive(false); // mouse ear can't be destroyed since it must respawn next fight 

    }

    public void BossDied()
    {
        StartCoroutine(FadeAway(5));
    }
}
