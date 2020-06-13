using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public class RTMesh : RTGeometry {
        // Start is called before the first frame update
        [SerializeField] Mesh m_mesh;

        public override RTGeometryType GetGeometryType() {
            return RTGeometryType.Mesh;
        }

        public void GetGeometry(in int materialIdx, out List<RTTriangle_t> tris) {
            int[] triangles = m_mesh.GetTriangles(0);
            Vector3[] vertices = m_mesh.vertices;
            tris = new List<RTTriangle_t>(triangles.Length / 3);

            for (int i = 0; i < triangles.Length; i += 3) {
                RTTriangle_t triangle = GenerateTriangle(vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]]);
                triangle.materialIndex = materialIdx;
                tris.Add(triangle);
            }
        }

        private RTTriangle_t GenerateTriangle(Vector3 v0, Vector3 v1, Vector3 v2) {
            v0 = transform.localToWorldMatrix.MultiplyPoint(v0);
            v1 = transform.localToWorldMatrix.MultiplyPoint(v1);
            v2 = transform.localToWorldMatrix.MultiplyPoint(v2);

            Vector3 _cross = Vector3.Cross(v1 - v0, v2 - v0);
            Vector3 _normal = Vector3.Normalize(_cross);
            float _planeD = -1 * Vector3.Dot(_normal, v0);
            float _area = Vector3.Dot(_normal, _cross);

            return new RTTriangle_t() {
                vert0 = v0,
                    vert1 = v1,
                    vert2 = v2,
                    area = _area,
                    normal = _normal,
                    planeD = _planeD,
                    isDoubleSide = 0,
            };
        }
    }
}