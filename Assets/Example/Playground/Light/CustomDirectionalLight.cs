using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An example of user defined directional light material
/// </summary>
public class CustomDirectionalLight : OpenRT.RTLight
{
    [SerializeField] public Color color = Color.white;
}