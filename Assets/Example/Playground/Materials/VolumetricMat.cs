using UnityEngine;
using OpenRT;

public class VolumetricMat : RTMaterial
{
    [SerializeField] public float luminanceLight = 1;
    [SerializeField] public float sigma = 1;
    [SerializeField] public float stepSize = 0.1f;
}
