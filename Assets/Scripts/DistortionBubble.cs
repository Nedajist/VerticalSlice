using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DistortionBubble : MonoBehaviour
{
    [SerializeField] float target_scale = 3;
    [SerializeField] float scale_random_factor = 2;
    [SerializeField] float growth_rate = 1;
    // Start is called before the first frame update
    void Start()
    {
        target_scale += Random.Range(-scale_random_factor, scale_random_factor);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x < target_scale)
        {
            transform.localScale += new Vector3(growth_rate * Time.deltaTime, growth_rate * Time.deltaTime, 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
