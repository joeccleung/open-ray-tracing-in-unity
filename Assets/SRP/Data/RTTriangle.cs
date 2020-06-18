using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {

    /// <summary>
    /// This structure matches with the RTSphere_t structure defined in RTSphere.compute
    /// </summary>
    public struct RTTriangle_t : OpenRT.IRTGeometryData {
        public int id; // geometryIndex
        public Vector3 vert0;
        public Vector3 vert1;
        public Vector3 vert2;
        public Vector3 normal;
        public float planeD;
        public float area;
        public int isDoubleSide;
        public int materialIndex;

        public List<float> GetGeometryInstanceData() {
            return new List<float> {
                vert0.x,
                vert0.y,
                vert0.z,
                vert1.x,
                vert1.y,
                vert1.z,
                vert2.x,
                vert2.y,
                vert2.z,
                normal.x,
                normal.y,
                normal.z,
                planeD,
                area
            };
        }

        public int GetStride() {
            return Vector3Extension.SizeOf() * 4 + sizeof(float) * 2;
        }

        public int GetCount() {
            return 1;
        }

        public void SetId(int idInput) {
            id = idInput;
        }
    }

}