using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public abstract class RTGeometry : MonoBehaviour, IRTGeometryData {
        [HideInInspector, SerializeField] private string intersectShaderGUID = string.Empty;

        protected RTBoundingBox boundingBox = RTBoundingBox.Empty;

        public abstract RTBoundingBox GetBoundingBox();
        public abstract int GetCount();
        public abstract List<float> GetGeometryInstanceData();
        public string GetIntersectShaderGUID() {
            return intersectShaderGUID;
        }
        public abstract int GetStride();
    }
}