using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{
    public struct Primitive
    {
        int geometryIndex;
        int geometryInstanceBegin;
        int geometryInstanceCount;
        int materialIndex;
        int materialInstanceIndex;
        int transformIndex;

        public Primitive(
            int geometryIndex,
            int geometryInstanceBegin,
            int geometryInstanceCount,
            int materialIndex,
            int materialInstanceIndex,
            int transformIndex)
        {
            this.geometryIndex = geometryIndex;
            this.geometryInstanceBegin = geometryInstanceBegin;
            this.geometryInstanceCount = geometryInstanceCount;
            this.materialIndex = materialIndex;
            this.materialInstanceIndex = materialInstanceIndex;
            this.transformIndex = transformIndex;
        }

        public static int GetStride()
        {
            return sizeof(int) * 6;
        }

    }
}