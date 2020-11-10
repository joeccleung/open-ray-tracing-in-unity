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
    public class RTMeshBVH : RTGeometry
    {
        private RTMeshBVHBuilder builder = new RTMeshBVHBuilder();
        [SerializeField] Mesh m_mesh;

        public override RTBoundingBox GetBoundingBox()
        {
            //TODO: Optimization. Does not need to rebuild the bounding box if the mesh did not deform or transform
            return builder.Root.bounding;
        }

        public override int GetCount()
        {
            return 1;
        }

        public override List<float> GetGeometryInstanceData()
        {
            return GetGeometryInstanceData(trianglesVertexOrder: GetTrianglesVertexOrder(), vertices: GetVertices());
        }

        public List<float> GetGeometryInstanceData(in int[] trianglesVertexOrder, Vector3[] vertices)
        {
            List<List<float>> triangles = BuildBVHAndTriangleList(trianglesVertexOrder, vertices);

            RTMeshBVHBuilder.Flatten(triangles,
                                     out List<List<float>> flattenBVH,
                                     out List<List<float>> reorderedPrimitives,
                                     builder.Root);

            return SerializeRTMeshBVH(flattenBVH, reorderedPrimitives);
        }

        public List<List<float>> BuildBVHAndTriangleList(int[] trianglesVertexOrder, Vector3[] vertices)
        {
            int primitiveCounter = 0;
            List<List<float>> triangles = new List<List<float>>();
            builder.Clear();

            for (int i = 0; i < trianglesVertexOrder.Length; i += 3)
            {
                RTBoundingBox box = RTBoundingBox.RTBoundingBoxFromTriangle(primitiveCounter,
                                                                            transform.localToWorldMatrix.MultiplyPoint(vertices[trianglesVertexOrder[i]]),
                                                                            transform.localToWorldMatrix.MultiplyPoint(vertices[trianglesVertexOrder[i + 1]]),
                                                                            transform.localToWorldMatrix.MultiplyPoint(vertices[trianglesVertexOrder[i + 2]]));
                builder.AddBoundingBox(box);

                triangles.Add(GenerateTriangle(vertices[trianglesVertexOrder[i]],
                                               vertices[trianglesVertexOrder[i + 1]],
                                               vertices[trianglesVertexOrder[i + 2]]));

                primitiveCounter++;
            }

            builder.Construct();
            return triangles;
        }


        public BVHNode GetRoot()
        {
            return builder.Root;
        }

        public override int GetStride()
        {
            return sizeof(float) * 14;
        }

        public int[] GetTrianglesVertexOrder()
        {
            return m_mesh.GetTriangles(0);
        }

        public Vector3[] GetVertices()
        {
            return m_mesh.vertices;
        }


        private List<float> GenerateTriangle(Vector3 v0, Vector3 v1, Vector3 v2)
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
                    normal.x,
                    normal.y,
                    normal.z,
                    planeD,
                    area
            };
        }

        public override bool IsUnevenStride()
        {
            return true;
        }

        private static List<float> SerializeRTMeshBVH(List<List<float>> flattenBVH, List<List<float>> reorderedPrimitives)
        {
            var result = new List<float>(){
                flattenBVH.Count,
                reorderedPrimitives.Count
            };
            flattenBVH.ForEach(v =>
            {
                result.AddRange(v);
            });
            reorderedPrimitives.ForEach(v =>
            {
                result.AddRange(v);
            });

            return result;
        }
    }

}
