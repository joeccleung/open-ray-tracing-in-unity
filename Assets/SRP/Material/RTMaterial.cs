using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public class RTMaterial : MonoBehaviour {
        [HideInInspector, SerializeField] private string closestHitGUID = string.Empty;

        public string GetClosestHitGUID() {
            return closestHitGUID;
        }
    }
}