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
    public class RTMeshBVH : RTGeometry, RTMeshBVHController.IActuator
    {
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

        public List<List<float>> BuildBVHAndTriangleList(Vector3[] normals, int[] trianglesVertexOrder, Vector3[] vertices)
        {
            return controller.BuildBVHAndTriangleList(normals, trianglesVertexOrder, vertices);
        }

        public override RTBoundingBox GetBoundingBox()
        {
            return controller.GetBoundingBox();
        }

        public override int GetCount()
        {
            return 1;
        }

        public override List<float> GetGeometryInstanceData()
        {
            return controller.GetGeometryInstanceData();
        }

        public override Vector3[] GetNormals()
        {
            return m_mesh.normals;
        }

        public BVHNode GetRoot()
        {
            return controller.GetRoot();
        }

        public override int GetStride()
        {
            return sizeof(float);
        }

        public int[] GetTrianglesVertexOrder(int mipmap)
        {
            return m_mesh.GetTriangles(mipmap);
        }

        public Vector3[] GetVertices()
        {
            return m_mesh.vertices;
        }


        public override bool IsUnevenStride()
        {
            return true;
        }

        public Vector3 LocalToWorld(Vector3 local)
        {
            return transform.localToWorldMatrix.MultiplyPoint(local);
        }
    }

}
