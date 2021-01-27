using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example {
    /// This is a custom sphere data container
    /// This goes with CustomSphere script
    [System.Serializable]
    public struct CustomSphereData : OpenRT.IRTGeometryData {
        [SerializeField] public Vector3 center;
        [SerializeField] public float radius;

        public CustomSphereData(Vector3 center, float radius) {
            this.center = center;
            this.radius = radius;
        }

        public int GetCount()
        {
            return 1;
        }

        public List<float> GetGeometryInstanceData(int geoLocalToGlobalIndexOffset, int mappingLocalToGlobalIndexOffset) {
            return new List<float> {
                center.x,
                center.y,
                center.z,
                radius
            };
        }

        public int GetStride() {
            return sizeof(float) * 4;
        }


        public void SetGameObjectCenter(Vector3 goCenter) {
            center = goCenter;
        }
    }
}