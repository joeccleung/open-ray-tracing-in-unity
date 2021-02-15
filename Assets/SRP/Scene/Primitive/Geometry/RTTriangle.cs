using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{

    /// <summary>
    /// This structure matches with the RTSphere_t structure defined in RTSphere.compute
    /// </summary>
    public struct RTTriangle_t : OpenRT.IRTGeometryData
    {
        public int id; // geometryIndex
        public Vector3 vert0;
        public Vector3 vert1;
        public Vector3 vert2;
        public Vector3 normal0;
        public Vector3 normal1;
        public Vector3 normal2;
        public float planeD;
        public float area;
        public int isDoubleSide;
        public int materialIndex;

        public List<float> GetGeometryInstanceData(int geoLocalToGlobalIndexOffset, int mappingLocalToGlobalIndexOffset)
        {
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
                normal0.x,
                normal0.y,
                normal0.z,
                normal1.x,
                normal1.y,
                normal1.z,
                normal2.x,
                normal2.y,
                normal2.z,
                planeD,
                area
            };
        }

        public int GetFloatCount()
        {
            return 3 * 6 + 2;   // 3 vertex + 3 normal + planeD (1f) + area (1f)
        }

        public int GetStride()
        {
            return sizeof(float) * GetFloatCount();
        }

        public int GetCount()
        {
            return 1;
        }

        public void SetId(int idInput)
        {
            id = idInput;
        }
    }

}