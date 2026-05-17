using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum VelocityAdditiveType
{
    vortex,
    hook,
    avoidance
}

public class VelocityAdditive
{
    public Vector2 additive_total;
    public float additive_max_magnitude;

    public Vector2 GetTrueAdditive()
    {
        Vector2 true_additive = additive_total.normalized * additive_max_magnitude;
        return true_additive;
    }

}
