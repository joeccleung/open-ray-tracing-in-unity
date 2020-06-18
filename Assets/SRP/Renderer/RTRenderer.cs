using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpenRT {
    public class RTRenderer : MonoBehaviour {
        [SerializeField] public RTGeometry geometry;
        [SerializeField] public RTMaterial material;
    }
}