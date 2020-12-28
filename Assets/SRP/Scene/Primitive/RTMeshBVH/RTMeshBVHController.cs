using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{
    public class RTMeshBVHController
    {
        public const int FLOAT_PER_TRIANGLE = 20;

        public interface IActuator
        {
            Vector3[] GetNormals();
            int[] GetTrianglesVertexOrder(int bitmap);
            Vector3[] GetVertices();
            Vector3 LocalToWorld(Vector3 local);
        }

        private IActuator m_actuator;
        private RTMeshBVHBuilder builder = new RTMeshBVHBuilder();
        private bool meshDirty = true;
        private List<List<float>> triangles = new List<List<float>>();

        public RTMeshBVHController(IActuator actuator)
        {
            m_actuator = actuator;
        }

        public List<float> GetGeometryInstanceData()
        {
            return GetGeometryInstanceData(normals: m_actuator.GetNormals(),
                                           trianglesVertexOrder: m_actuator.GetTrianglesVertexOrder(0),
                                           vertices: m_actuator.GetVertices());
        }

        public RTBoundingBox GetBoundingBox()
        {
            //TODO: Optimization. Does not need to rebuild the bounding box if the mesh did not deform or transform
            return builder.Root.bounding;
        }

        public List<float> GetGeometryInstanceData(Vector3[] normals, in int[] trianglesVertexOrder, Vector3[] vertices)
        {
            List<List<float>> triangles = BuildBVHAndTriangleList(normals, trianglesVertexOrder, vertices);

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


        private List<float> GenerateTriangle(Vector3 v0,
                                             Vector3 v1,
                                             Vector3 v2,
                                             Vector3 n0,
                                             Vector3 n1,
                                             Vector3 n2)
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

        public List<List<float>> BuildBVHAndTriangleList(Vector3[] normals, int[] trianglesVertexOrder, Vector3[] vertices)
        {
            _BuildBVHAndTriangleList(builder, ref meshDirty, normals, triangles, trianglesVertexOrder, vertices);
            return triangles;
        }

        public void _BuildBVHAndTriangleList(RTMeshBVHBuilder builder,
                                             ref bool meshDirty,
                                             Vector3[] normals,
                                             List<List<float>> triangles,
                                             int[] trianglesVertexOrder,
                                             Vector3[] vertices)
        {
            if (meshDirty)
            {
                meshDirty = false;

                int primitiveCounter = 0;
                triangles.Clear();
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
                                                   vertices[trianglesVertexOrder[i + 2]],
                                                   normals[trianglesVertexOrder[i]],
                                                   normals[trianglesVertexOrder[i + 1]],
                                                   normals[trianglesVertexOrder[i + 2]]));

                    primitiveCounter++;
                }

                builder.Construct();
            }

        }

        public static List<float> SerializeRTMeshBVH(List<List<float>> flattenBVH, List<List<float>> reorderedPrimitives)
        {
            var bvhLen = flattenBVH.Count * RTBoundingBox.NUMBER_OF_FLOAT;
            var triLen = reorderedPrimitives.Count * FLOAT_PER_TRIANGLE;
            var result = new List<float>(){
                bvhLen,
                triLen
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
