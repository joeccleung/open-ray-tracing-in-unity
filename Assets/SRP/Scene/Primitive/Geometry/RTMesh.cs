using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{
    public class RTMesh : RTGeometry
    {
        public const int TRIANGLE_STRIDE = 24;

        [SerializeField] Mesh m_mesh;
        private int m_meshHashCode = 0;

        private List<float> tris = new List<float>();
        private int numberOfTriangles = 0;

        public override RTBoundingBox GetTopLevelBoundingBox(int assignedPrimitiveId)
        {
            //TODO: Optimization. Does not need to rebuild the bounding box if the mesh did not deform or transform
            boundingBox.geoIndices = new HashSet<int> { assignedPrimitiveId };
            return boundingBox;
        }

        public override List<float> GetGeometryInstanceData(int geoLocalToGlobalIndexOffset, int mappingLocalToGlobalIndexOffset)
        {
            //TODO: Cache the mesh if it does not deform

            int[] trianglesVertexOrder = m_mesh.GetTriangles(0);
            numberOfTriangles = trianglesVertexOrder.Length / 3;
            Vector3[] vertices = m_mesh.vertices;
            Vector3[] normals = m_mesh.normals;
            Vector2[] uvs = m_mesh.uv;

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
                                                        normals[trianglesVertexOrder[i + 2]],
                                                        uvs[trianglesVertexOrder[i]],
                                                        uvs[trianglesVertexOrder[i + 1]],
                                                        uvs[trianglesVertexOrder[i + 2]]);
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
                                             Vector3 n2,
                                             Vector2 uv0,
                                             Vector2 uv1,
                                             Vector2 uv2)
        {
            Vector3 wv0 = transform.localToWorldMatrix.MultiplyPoint(v0);
            Vector3 wv1 = transform.localToWorldMatrix.MultiplyPoint(v1);
            Vector3 wv2 = transform.localToWorldMatrix.MultiplyPoint(v2);

            Vector3 wn0 = Vector3.Normalize(transform.localToWorldMatrix.MultiplyVector(n0));
            Vector3 wn1 = Vector3.Normalize(transform.localToWorldMatrix.MultiplyVector(n1));
            Vector3 wn2 = Vector3.Normalize(transform.localToWorldMatrix.MultiplyVector(n2));

            // Vector3 _cross = Vector3.Cross(wv1 - wv0, wv2 - wv0);
            // Vector3 normal = Vector3.Normalize(_cross);
            // float planeD = -1 * Vector3.Dot(normal, wv0);
            // float area = Vector3.Dot(normal, _cross);

            return new List<float>() {
                wv0.x,
                wv0.y,
                wv0.z,
                wv1.x,
                wv1.y,
                wv1.z,
                wv2.x,
                wv2.y,
                wv2.z,
                wn0.x,
                wn0.y,
                wn0.z,
                wn1.x,
                wn1.y,
                wn1.z,
                wn2.x,
                wn2.y,
                wn2.z,
                uv0.x,
                uv0.y,
                uv1.x,
                uv1.y,
                uv2.x,
                uv2.y
            };
        }

        public override Vector3[] GetNormals()
        {
            return m_mesh.normals;
        }

        public override bool IsDirty()
        {
            bool isDirty = false;

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

            return isDirty;
        }

        public override bool IsGeometryValid()
        {
            return m_mesh != null;
        }
    }
}