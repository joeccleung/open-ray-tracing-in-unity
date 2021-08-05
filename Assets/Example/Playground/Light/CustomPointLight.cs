using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An example of user defined directional light material
/// </summary>
public class CustomPointLight : OpenRT.RTLight
{
    [SerializeField] public Color color = Color.white;
    [SerializeField] public float innerRange = 1;
    [SerializeField] public float range = 1;
}
