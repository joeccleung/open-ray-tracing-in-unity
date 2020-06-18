using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public struct Primitive {
        int geometryIndex;
        int geometryInstanceIndex;
        int materialIndex;
        int transformIndex;

        public Primitive(int geometryIndex, int geometryInstanceIndex, int materialIndex, int transformIndex)
        {
            this.geometryIndex = geometryIndex;
            this.geometryInstanceIndex = geometryInstanceIndex;
            this.materialIndex = materialIndex;
            this.transformIndex = transformIndex;
        }

        public static int GetStride() {
            return sizeof(int) * 4;
        }

    }
}