using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AdditiveDistortion : MonoBehaviour
{
    [SerializeField] List<SpriteRenderer> _list_of_distortion_sprites;
    private List<Material> _list_of_distortion_materials = new List<Material>();

    [HideInInspector] public GameObject player;

    private List<Vector3> _list_of_original_positions = new List<Vector3>();

    void Start()
    {
        foreach (SpriteRenderer sprite in _list_of_distortion_sprites)
        {
            _list_of_distortion_materials.Add(sprite.material); // material list set here
        }
        for (int i = 0; i < 4; i ++) _list_of_original_positions.Add(_list_of_distortion_sprites[i].transform.position); // original positions stored 

        ResetSelf(); // Changes to materials PERSIST PERMANENTLY, reset every time program starts 

    }

    public void DistortColor()
    {
        if (Random.Range(0, 2) == 1) StartCoroutine(ColorShift(1, 0.1f)); // color shift
        else StartCoroutine(ColorShake(0.8f, 2f, 2f)); // color shake
    }

    public void DistortPosition()
    {
        switch (Random.Range(0, 5))
        {
            case 0:
                StartCoroutine(DiagonalMove(0.03f, -0.03f)); // upward diagonal diagonal
                break;
            case 1:
                StartCoroutine(DiagonalMove(-0.03f, 0.03f)); // downward diagonal
                break;
            case 2:
                StartCoroutine(FourSplit(0.04f, 0.04f)); // apart snaps together
                break;
            case 3:
                StartCoroutine(FourSplit(-0.04f, -0.04f)); // apart to together 
                break;
            case 4:
                StartCoroutine(Syncopate(0.8f, 1.5f, 1.5f)); // syncopate 
                break;
        }
    }

    private void Update()
    {
        if (player != null) transform.position = player.transform.position; // player is set by GameController on state transition 
        else transform.position = Vector3.zero;
    }





    public IEnumerator ColorShift(float starting_opacity, float shrink_rate) // starting_opacity should generally be between 0 - 1, it makes the rainbows appear
    {
        foreach (SpriteRenderer sprite in _list_of_distortion_sprites) sprite.enabled = true;
        float opacity = starting_opacity;

        while (opacity > 0)
        {
            opacity -= shrink_rate * Time.deltaTime;
            foreach (Material distortion_material in _list_of_distortion_materials) distortion_material.SetFloat("_color_opacity", opacity);
            yield return new WaitForFixedUpdate();
        }


        foreach (Material distortion_material in _list_of_distortion_materials) distortion_material.SetFloat("_color_opacity", 0);
        foreach (SpriteRenderer sprite in _list_of_distortion_sprites) sprite.enabled = false;
    }

    public IEnumerator ColorShake(float starting_opacity, float duration, float rate_of_change) // rapidly scrambles the hues of the rainbow
    {
        foreach (SpriteRenderer sprite in _list_of_distortion_sprites) sprite.enabled = true;

        float opacity = starting_opacity;
        foreach (Material distortion_material in _list_of_distortion_materials) distortion_material.SetFloat("_color_opacity", opacity);

        float interval_timer = 0;
        rate_of_change *= Random.Range(-1, 2);
        Vector2 new_offset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            interval_timer -= Time.deltaTime;
            new_offset += new Vector2(1, 1) * Time.deltaTime * rate_of_change;
            foreach (Material distortion_material in _list_of_distortion_materials) distortion_material.SetVector("_color_offset", new_offset);
            
            yield return new WaitForFixedUpdate();
        }


        foreach (Material distortion_material in _list_of_distortion_materials) distortion_material.SetFloat("_color_opacity", 0);
        foreach (Material distortion_material in _list_of_distortion_materials) distortion_material.SetVector("_color_offset", Vector2.zero);

        foreach (SpriteRenderer sprite in _list_of_distortion_sprites) sprite.enabled = false;
    }

    public IEnumerator DiagonalMove(float starting_position, float additive_growth_rate) // distortion layer moves in a diagonal direction, starting at starting_position and moving to 0 
    {
        float additive = starting_position;
        foreach (SpriteRenderer sprite in _list_of_distortion_sprites) sprite.enabled = true;

        foreach (Material distortion_material in _list_of_distortion_materials) distortion_material.SetVector("_positional_additive", new Vector2(starting_position, starting_position));

        while ( (starting_position < 0 && additive < 0) || (starting_position > 0 && additive > 0) ) 
        {
            additive += additive_growth_rate * Time.deltaTime;
            foreach (Material distortion_material in _list_of_distortion_materials) distortion_material.SetVector("_positional_additive", new Vector2(additive, additive));
            yield return new WaitForFixedUpdate();
        }

        foreach (Material distortion_material in _list_of_distortion_materials) distortion_material.SetVector("_positional_additive", new Vector2(0, 0));


        foreach (SpriteRenderer sprite in _list_of_distortion_sprites) sprite.enabled = false;

    }

    public IEnumerator FourSplit(float starting_magnitude, float growth_rate) // distortion layers part in 4 opposite diagonal directions. Then return to 0
    {
        Vector2[] additive_list = { new Vector2(starting_magnitude, -starting_magnitude), new Vector2(-starting_magnitude, -starting_magnitude), new Vector2(starting_magnitude, starting_magnitude), new Vector2(-starting_magnitude, starting_magnitude) };
        Vector2[] growth_rate_list = { new Vector2(-growth_rate, growth_rate), new Vector2(growth_rate, growth_rate), new Vector2(-growth_rate, -growth_rate), new Vector2(growth_rate, -growth_rate) };
        
        foreach (SpriteRenderer sprite in _list_of_distortion_sprites) sprite.enabled = true;

        while ( (additive_list[1].x < 0 && additive_list[1].y < 0 && starting_magnitude > 0) || (additive_list[1].x > 0 && additive_list[1].y > 0 && starting_magnitude < 0))
        {
            for (int i = 0; i < 4; i++)
            {
                _list_of_distortion_materials[i].SetVector("_positional_additive", additive_list[i]);
                additive_list[i] += growth_rate_list[i] * Time.deltaTime;
            }
            yield return new WaitForFixedUpdate();
        }


        foreach (Material distortion_material in _list_of_distortion_materials) distortion_material.SetVector("_positional_additive", new Vector2(0, 0));

        foreach (SpriteRenderer sprite in _list_of_distortion_sprites) sprite.enabled = false;

    }
    public IEnumerator Syncopate(float max_movement_magnitude, float movement_rate, float duration) // scatters the 4 quadrants in different directions, then instantly snaps them back
    {
        float timer = duration;
        Vector2[] direction_list = { new Vector2(Random.Range(-max_movement_magnitude, max_movement_magnitude), Random.Range(-max_movement_magnitude, max_movement_magnitude)).normalized,
                                    new Vector2(Random.Range(-max_movement_magnitude, max_movement_magnitude), Random.Range(-max_movement_magnitude, max_movement_magnitude)).normalized,
                                    new Vector2(Random.Range(-max_movement_magnitude, max_movement_magnitude), Random.Range(-max_movement_magnitude, max_movement_magnitude)).normalized,
                                    new Vector2(Random.Range(-max_movement_magnitude, max_movement_magnitude), Random.Range(-max_movement_magnitude, max_movement_magnitude)).normalized};


        foreach (SpriteRenderer sprite in _list_of_distortion_sprites) sprite.enabled = true;

        while (timer > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                _list_of_distortion_materials[i].SetVector("_positional_additive", (Vector2)_list_of_distortion_materials[i].GetVector("_positional_additive") + direction_list[i] * movement_rate * Time.deltaTime);
                _list_of_distortion_materials[i].SetFloat("_color_opacity", timer / duration); // slowly fades the co
                _list_of_distortion_sprites[i].transform.position += (Vector3) direction_list[i] * movement_rate * Time.deltaTime;
            }

            timer -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        foreach (Material distortion_material in _list_of_distortion_materials) distortion_material.SetVector("_positional_additive", new Vector2(0, 0));
        foreach (Material distortion_material in _list_of_distortion_materials) distortion_material.SetFloat("_color_opacity", 0);

        foreach (SpriteRenderer sprite in _list_of_distortion_sprites) sprite.enabled = false;
        for (int i = 0; i < 4; i++) _list_of_distortion_sprites[i].transform.position = _list_of_original_positions[i]; 

    }

    private void ResetSelf() // moves the quadrants back to their original positions. Removes any hue changes (so the quadrants look identical to the actual environment). Hides the quadrant sprites from view. 
    {
        foreach (SpriteRenderer sprite in _list_of_distortion_sprites) sprite.enabled = false;
        foreach (Material distortion_material in _list_of_distortion_materials) distortion_material.SetFloat("_color_opacity", 0);
        foreach (Material distortion_material in _list_of_distortion_materials) distortion_material.SetVector("_positional_additive", new Vector2(0, 0));
    }

}
