using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{
    public class RTMeshBVHController
    {
        public interface IActuator
        {
            int[] GetTrianglesVertexOrder(int bitmap);
            Vector3[] GetVertices();
            Vector3 LocalToWorld(Vector3 local);
        }

        private IActuator m_actuator;
        private RTMeshBVHBuilder builder = new RTMeshBVHBuilder();

        public RTMeshBVHController(IActuator actuator)
        {
            m_actuator = actuator;
        }

        public List<float> GetGeometryInstanceData()
        {
            return GetGeometryInstanceData(trianglesVertexOrder: m_actuator.GetTrianglesVertexOrder(0),
                                           vertices: m_actuator.GetVertices());
        }

        public RTBoundingBox GetBoundingBox()
        {
            //TODO: Optimization. Does not need to rebuild the bounding box if the mesh did not deform or transform
            return builder.Root.bounding;
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

        public BVHNode GetRoot()
        {
            return builder.Root;
        }


        private List<float> GenerateTriangle(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            v0 = m_actuator.LocalToWorld(v0);
            v1 = m_actuator.LocalToWorld(v1);
            v2 = m_actuator.LocalToWorld(v2);

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

        public List<List<float>> BuildBVHAndTriangleList(int[] trianglesVertexOrder, Vector3[] vertices)
        {
            int primitiveCounter = 0;
            List<List<float>> triangles = new List<List<float>>();
            builder.Clear();

            for (int i = 0; i < trianglesVertexOrder.Length; i += 3)
            {
                RTBoundingBox box = RTBoundingBox.RTBoundingBoxFromTriangle(primitiveCounter,
                                                                            m_actuator.LocalToWorld(vertices[trianglesVertexOrder[i]]),
                                                                            m_actuator.LocalToWorld(vertices[trianglesVertexOrder[i + 1]]),
                                                                            m_actuator.LocalToWorld(vertices[trianglesVertexOrder[i + 2]]));
                builder.AddBoundingBox(box);

                triangles.Add(GenerateTriangle(vertices[trianglesVertexOrder[i]],
                                               vertices[trianglesVertexOrder[i + 1]],
                                               vertices[trianglesVertexOrder[i + 2]]));

                primitiveCounter++;
            }

            builder.Construct();
            return triangles;
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
