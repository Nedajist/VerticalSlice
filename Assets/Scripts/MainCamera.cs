using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    
    public float seconds_of_camera_shake = 0;
    private Vector3 additiveSum;

    void Update()
    {
        transform.rotation = Quaternion.identity;
        if (seconds_of_camera_shake > 0)
        {
            additiveSum += RandomNormalVector3();
            transform.localEulerAngles += additiveSum;
            seconds_of_camera_shake -= Time.deltaTime;
        }
        else
        {
            additiveSum = Vector3.zero;
        }

    }

    private Vector3 RandomNormalVector3()
    {
        return (new Vector3(0, 0, Random.Range(-1f, 1f)));
    }

}
