using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenRT;

public class TranslucentMat : RTMaterial
{
    [SerializeField] public Color color = Color.white;
    [SerializeField] public float reflectivity = 0;
    [SerializeField] public float secondaryRayEffect = 0;
    [SerializeField] public float transparency = 0;
}
