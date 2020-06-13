using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public abstract class RTGeometry : MonoBehaviour {
        public abstract RTGeometryType GetGeometryType();
    }
}