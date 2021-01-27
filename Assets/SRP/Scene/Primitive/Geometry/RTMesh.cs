using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{
    public class RTMesh : RTGeometry
    {
        public const int TRIANGLE_STRIDE = 20;

        [SerializeField] Mesh m_mesh;
        private int m_meshHashCode = 0;

        private List<float> tris = new List<float>();
        private int numberOfTriangles = 0;

        public override RTBoundingBox GetTopLevelBoundingBox(int assignedPrimitiveId)
        {
            //TODO: Optimization. Does not need to rebuild the bounding box if the mesh did not deform or transform
            boundingBox.geoIndices = new List<int> { assignedPrimitiveId };
            return boundingBox;
        }

        public override List<float> GetGeometryInstanceData(int geoLocalToGlobalIndexOffset, int mappingLocalToGlobalIndexOffset)
        {
            //TODO: Cache the mesh if it does not deform

            int[] trianglesVertexOrder = m_mesh.GetTriangles(0);
            numberOfTriangles = trianglesVertexOrder.Length / 3;
            Vector3[] vertices = m_mesh.vertices;
            Vector3[] normals = m_mesh.normals;

            boundingBox.min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            boundingBox.max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (var vertex in vertices)
            {
                AddVertices(ref boundingBox, vertex);
            }

            tris.Clear();

            for (int i = 0; i < trianglesVertexOrder.Length; i += 3)
            {
                List<float> triangle = GenerateTriangle(vertices[trianglesVertexOrder[i]],
                                                        vertices[trianglesVertexOrder[i + 1]],
                                                        vertices[trianglesVertexOrder[i + 2]],
                                                        normals[trianglesVertexOrder[i]],
                                                        normals[trianglesVertexOrder[i + 1]],
                                                        normals[trianglesVertexOrder[i + 2]]);
                tris.AddRange(triangle);
            }

            return tris;
        }

        public override int GetStride()
        {
            return sizeof(float) * TRIANGLE_STRIDE;
        }

        public override int GetCount()
        {
            return numberOfTriangles;
        }

        private void AddVertices(ref RTBoundingBox boundingBox, Vector3 vertex)
        {
            Vector3 worldVex = transform.localToWorldMatrix.MultiplyPoint(vertex);
            boundingBox.min = Vector3.Min(boundingBox.min, worldVex);
            boundingBox.max = Vector3.Max(boundingBox.max, worldVex);
        }

        private List<float> GenerateTriangle(Vector3 v0,
                                             Vector3 v1,
                                             Vector3 v2,
                                             Vector3 n0,
                                             Vector3 n1,
                                             Vector3 n2)
        {
            v0 = transform.localToWorldMatrix.MultiplyPoint(v0);
            v1 = transform.localToWorldMatrix.MultiplyPoint(v1);
            v2 = transform.localToWorldMatrix.MultiplyPoint(v2);

            Vector3 _cross = Vector3.Cross(v1 - v0, v2 - v0);
            Vector3 normal = Vector3.Normalize(_cross);
            float planeD = -1 * Vector3.Dot(normal, v0);
            float area = Vector3.Dot(normal, _cross);

            return new List<float>() {
                v0.x,
                v0.y,
                v0.z,
                v1.x,
                v1.y,
                v1.z,
                v2.x,
                v2.y,
                v2.z,
                n0.x,
                n0.y,
                n0.z,
                n1.x,
                n1.y,
                n1.z,
                n2.x,
                n2.y,
                n2.z,
                planeD,
                area
            };
        }

        public override Vector3[] GetNormals()
        {
            return m_mesh.normals;
        }

        public override bool IsDirty()
        {
            if (transform.hasChanged)
            {
                transform.hasChanged = false;
                return true;
            }

            int _curHashCode = m_mesh.GetHashCode();
            if (m_meshHashCode != _curHashCode)
            {
                m_meshHashCode = _curHashCode;
                return true;
            }

            return false;
        }
        
        public override bool IsGeometryValid()
        {
            return m_mesh != null;
        }
    }
}