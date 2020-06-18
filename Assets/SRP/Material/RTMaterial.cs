using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public class RTMaterial : MonoBehaviour {
        [HideInInspector, SerializeField] public int shaderIndex = 0; // Close hit shader

    }
}