using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{
    /// <summary>
    /// (1) - Map each triangles into RTBoundingBox
    /// (2) - Build BVH out from the list of RTBoundingBox
    /// (3) - Flatten BVH
    /// (4) - Serialize BVH with the triangles to list of floats
    /// 
    /// | Stride of the BVH | Stride of all triangles | < Bounding Boxes > | < Triangles > |
    /// 
    /// </summary>    
    public class RTMeshBVH : IRTMeshBVH
    {
        public const int TRIANGLE_STRIDE = 24; // 3 * float3 vertices + 3 * float3 normals + 3 * float2 UVs
        private RTMeshBVHController m_controller;
        private RTMeshBVHController controller
        {
            get
            {
                m_controller = m_controller ?? new RTMeshBVHController(this);
                return m_controller;
            }
        }
        [SerializeField] Mesh m_mesh;
        private int m_meshHashCode = 0;
        [SerializeField] private int m_minNumberOfGeoPerBox = 30;
        private int m_minNumberOfGeoPerBoxPrev = 0;
        // [SerializeField] private bool m_refreshMesh;

        public override void BuildBVHAndTriangleList(int geoLocalToGlobalIndexOffset,
                                                     int mappingLocalToGlobalIndexOffset)
        {
            controller.BuildBVHAndTriangleList(geoLocalToGlobalIndexOffset,
                                               mappingLocalToGlobalIndexOffset,
                                               m_minNumberOfGeoPerBox);
        }

        public override void BuildBVHAndTriangleList(Vector3[] normals,
                                                     int[] trianglesVertexOrder,
                                                     Vector2[] uvs,
                                                     Vector3[] vertices)
        {
            controller.BuildBVHAndTriangleList(m_minNumberOfGeoPerBox,
                                               normals,
                                               trianglesVertexOrder,
                                               uvs,
                                               vertices);
        }

        public override List<float> GetAccelerationStructureGeometryData(int geoLocalToGlobalIndexOffset, int mappingLocalToGlobalIndexOffset)
        {
            return controller.GetAccelerationStructureGeometryData(geoLocalToGlobalIndexOffset, mappingLocalToGlobalIndexOffset, m_minNumberOfGeoPerBox);
        }

        public override List<int> GetAccelerationStructureGeometryMapping(int geoLocalToGlobalIndexOffset, int mappingLocalToGlobalIndexOffset)
        {
            return controller.GetAccelerationStructureGeometryMapping(geoLocalToGlobalIndexOffset, mappingLocalToGlobalIndexOffset, m_minNumberOfGeoPerBox);
        }

        public override RTBoundingBox GetTopLevelBoundingBox(int assignedPrimitiveId)
        {
            return controller.GetTopLevelBoundingBox(assignedPrimitiveId);
        }

        public override int GetCount()
        {
            return 1;
        }

        public override List<float> GetGeometryInstanceData(int geoLocalToGlobalIndexOffset, int mappingLocalToGlobalIndexOffset)
        {
            return controller.GetGeometryInstanceData(geoLocalToGlobalIndexOffset, mappingLocalToGlobalIndexOffset, m_minNumberOfGeoPerBox);
        }

        public override Vector3[] GetNormals()
        {
            return m_mesh.normals;
        }

        public override BVHNode GetRoot()
        {
            return controller.GetRoot();
        }

        public override int GetStride()
        {
            return sizeof(float);
        }

        public override int[] GetTrianglesVertexOrder(int mipmap)
        {
            return m_mesh.GetTriangles(mipmap);
        }

        public override Vector2[] GetUVs()
        {
            return m_mesh.uv;
        }

        public override Vector3[] GetVertices()
        {
            return m_mesh.vertices;
        }

        public override bool IsAccelerationStructure()
        {
            return true;    // This tell the scene parser to extract the geometry in 3 parts
        }

        public override bool IsDirty()
        {
            bool isDirty = false;

            if (prevFrameIsEnable != gameObject.activeInHierarchy)
            {
                prevFrameIsEnable = gameObject.activeInHierarchy;
                isDirty = true;
            }

            if (transform.hasChanged)
            {
                transform.hasChanged = false;
                isDirty = true;
            }

            int _curHashCode = m_mesh.GetHashCode();
            if (m_meshHashCode != _curHashCode)
            {
                m_meshHashCode = _curHashCode;
                isDirty = true;
            }

            if (m_minNumberOfGeoPerBoxPrev != m_minNumberOfGeoPerBox)
            {
                m_minNumberOfGeoPerBoxPrev = m_minNumberOfGeoPerBox;
                isDirty = true;
            }

            return isDirty;
        }

        public override bool IsGeometryValid()
        {
            return true;
        }

        public override Vector3 LocalToWorldDirection(Vector3 local)
        {
            return transform.localToWorldMatrix.MultiplyVector(local);
        }

        public override Vector3 LocalToWorldVertex(Vector3 local)
        {
            return transform.localToWorldMatrix.MultiplyPoint(local);
        }

        public Mesh mesh
        {
            get
            {
                return m_mesh;
            }
            set
            {
                m_mesh = value;
                controller.SetMesh();
            }
        }

        public void OnDrawGizmos()
        {
            // if (m_refreshMesh)
            // {
            //     m_refreshMesh = false;
            //     m_controller.SetMesh();
            // }
        }

        public void MarkMeshAsDirty()
        {
            m_controller.SetMesh();
        }
    }
}
