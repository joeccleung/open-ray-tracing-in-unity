using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public class RTMaterial : MonoBehaviour {
        [HideInInspector, SerializeField] public int shaderIndex = 0; // Close hit shader
        [HideInInspector, SerializeField] public int intersectShaderIndex = 0; // Intersect shader
    }
}