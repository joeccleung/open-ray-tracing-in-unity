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
            Vector3[] GetNormals();
            int[] GetTrianglesVertexOrder(int bitmap);
            Vector2[] GetUVs();
            Vector3[] GetVertices();
            Vector3 LocalToWorldDirection(Vector3 local);
            Vector3 LocalToWorldVertex(Vector3 local);
        }

        private IActuator m_actuator;
        private RTMeshBVHBuilder m_builder = new RTMeshBVHBuilder();
        private List<List<float>> m_flattenBVH = new List<List<float>>();
        private bool m_meshDirty = true;
        private List<List<int>> m_accelerationGeometryMappingCollection = new List<List<int>>();
        private List<float> m_triangles = new List<float>();

        public RTMeshBVHController(IActuator actuator)
        {
            m_actuator = actuator;
        }

        public void BuildBVHAndTriangleList(
            int geoLocalToGlobalIndexOffset,
            int mappingLocalToGlobalIndexOffset,
            int minNumberOfGeoPerBox)
        {
            _BuildFlatternBVHIfDirty(ref m_builder,
                                     geoLocalToGlobalIndexOffset,
                                     mappingLocalToGlobalIndexOffset,
                                     ref m_meshDirty,
                                     minNumberOfGeoPerBox,
                                     m_actuator.GetNormals(),
                                     m_actuator.GetTrianglesVertexOrder(0),
                                     m_actuator.GetUVs(),
                                     m_actuator.GetVertices(),
                                     ref m_flattenBVH,
                                     ref m_accelerationGeometryMappingCollection,
                                     ref m_triangles);
        }

        private void _BuildFlatternBVHIfDirty(ref RTMeshBVHBuilder builder,
                                              int geoLocalToGlobalIndexOffset,
                                              int mappingLocalToGlobalIndexOffset,
                                              ref bool meshDirty,
                                              int minNumberOfGeoPerBox,
                                              Vector3[] normals,
                                              in int[] trianglesVertexOrder,
                                              Vector2[] uvs,
                                              Vector3[] vertices,
                                              ref List<List<float>> flattenBVH,
                                              ref List<List<int>> accelerationGeometryMappingCollection,
                                              ref List<float> triangles)
        {
            if (meshDirty)
            {
                // meshDirty = false;   // Disable dirty flag

                _BuildBVHAndTriangleList(ref builder,
                                         minNumberOfGeoPerBox,
                                         normals,
                                         triangles,
                                         trianglesVertexOrder,
                                         uvs,
                                         vertices);

                RTMeshBVHBuilder.Flatten(ref flattenBVH,
                                         geoLocalToGlobalIndexOffset,
                                         mappingLocalToGlobalIndexOffset,
                                         ref accelerationGeometryMappingCollection,
                                         builder.Root);
            }
        }

        public List<float> GetAccelerationStructureGeometryData(int geoLocalToGlobalIndexOffset,
                                                                int mappingLocalToGlobalIndexOffset,
                                                                int minNumberOfGeoPerBox)
        {
            return GetAccelerationStructureGeometryData(builder: ref m_builder,
                                                        geoLocalToGlobalIndexOffset: geoLocalToGlobalIndexOffset,
                                                        mappingLocalToGlobalIndexOffset: mappingLocalToGlobalIndexOffset,
                                                        meshDirty: ref m_meshDirty,
                                                        minNumberOfGeoPerBox: minNumberOfGeoPerBox,
                                                        normals: m_actuator.GetNormals(),
                                                        flattenBVH: ref m_flattenBVH,
                                                        accelerationGeometryMappingCollection: ref m_accelerationGeometryMappingCollection,
                                                        triangles: ref m_triangles,
                                                        trianglesVertexOrder: m_actuator.GetTrianglesVertexOrder(0),
                                                        vertices: m_actuator.GetVertices());
        }

        public List<float> GetAccelerationStructureGeometryData(ref RTMeshBVHBuilder builder,
                                                                int geoLocalToGlobalIndexOffset,
                                                                int mappingLocalToGlobalIndexOffset,
                                                                ref bool meshDirty,
                                                                int minNumberOfGeoPerBox,
                                                                Vector3[] normals,
                                                                ref List<List<float>> flattenBVH,
                                                                ref List<List<int>> accelerationGeometryMappingCollection,
                                                                ref List<float> triangles,
                                                                in int[] trianglesVertexOrder,
                                                                Vector3[] vertices)
        {
            // _BuildFlatternBVHIfDirty(ref builder,
            //                          geoLocalToGlobalIndexOffset,
            //                          mappingLocalToGlobalIndexOffset,
            //                          ref meshDirty,
            //                          minNumberOfGeoPerBox,
            //                          normals,
            //                          trianglesVertexOrder,
            //                          vertices,
            //                          ref flattenBVH,
            //                          ref accelerationGeometryMappingCollection,
            //                          ref triangles);

            return triangles;
        }

        public List<int> GetAccelerationStructureGeometryMapping(int geoLocalToGlobalIndexOffset,
                                                                 int mappingLocalToGlobalIndexOffset,
                                                                 int minNumberOfGeoPerBox)
        {
            return GetAccelerationStructureGeometryMapping(builder: ref m_builder,
                                                           geoLocalToGlobalIndexOffset: geoLocalToGlobalIndexOffset,
                                                           mappingLocalToGlobalIndexOffset: mappingLocalToGlobalIndexOffset,
                                                           meshDirty: ref m_meshDirty,
                                                           minNumberOfGeoPerBox: minNumberOfGeoPerBox,
                                                           normals: m_actuator.GetNormals(),
                                                           flattenBVH: ref m_flattenBVH,
                                                           accelerationGeometryMappingCollection: ref m_accelerationGeometryMappingCollection,
                                                           triangles: ref m_triangles,
                                                           trianglesVertexOrder: m_actuator.GetTrianglesVertexOrder(0),
                                                           vertices: m_actuator.GetVertices());
        }

        public List<int> GetAccelerationStructureGeometryMapping(ref RTMeshBVHBuilder builder,
                                                                 int geoLocalToGlobalIndexOffset,
                                                                 int mappingLocalToGlobalIndexOffset,
                                                                 ref bool meshDirty,
                                                                 int minNumberOfGeoPerBox,
                                                                 Vector3[] normals,
                                                                 ref List<List<float>> flattenBVH,
                                                                 ref List<List<int>> accelerationGeometryMappingCollection,
                                                                 ref List<float> triangles,
                                                                 in int[] trianglesVertexOrder,
                                                                 Vector3[] vertices)
        {
            // _BuildFlatternBVHIfDirty(ref builder,
            //                          geoLocalToGlobalIndexOffset,
            //                          mappingLocalToGlobalIndexOffset,
            //                          ref meshDirty,
            //                          minNumberOfGeoPerBox,
            //                          normals,
            //                          trianglesVertexOrder,
            //                          vertices,
            //                          ref flattenBVH,
            //                          ref accelerationGeometryMappingCollection,
            //                          ref triangles);

            return SerializeObjectLevelGeoMap(accelerationGeometryMappingCollection);
        }

        public List<float> GetGeometryInstanceData(int geoLocalToGlobalIndexOffset, int mappingLocalToGlobalIndexOffset, int minNumberOfGeoPerBox)
        {
            return GetGeometryInstanceData(ref m_builder,
                                           ref m_meshDirty,
                                           minNumberOfGeoPerBox: minNumberOfGeoPerBox,
                                           normals: m_actuator.GetNormals(),
                                           flattenBVH: ref m_flattenBVH,
                                           accelerationGeometryMappingCollection: ref m_accelerationGeometryMappingCollection,
                                           geoLocalToGlobalIndexOffset: geoLocalToGlobalIndexOffset,
                                           mappingLocalToGlobalIndexOffset: mappingLocalToGlobalIndexOffset,
                                           triangles: ref m_triangles,
                                           trianglesVertexOrder: m_actuator.GetTrianglesVertexOrder(0),
                                           vertices: m_actuator.GetVertices());
        }

        public List<float> GetGeometryInstanceData(ref RTMeshBVHBuilder builder,
                                                   ref bool meshDirty,
                                                   int minNumberOfGeoPerBox,
                                                   Vector3[] normals,
                                                   ref List<List<float>> flattenBVH,
                                                   ref List<List<int>> accelerationGeometryMappingCollection,
                                                   int geoLocalToGlobalIndexOffset,
                                                   int mappingLocalToGlobalIndexOffset,
                                                   ref List<float> triangles,
                                                   in int[] trianglesVertexOrder,
                                                   Vector3[] vertices)
        {
            // _BuildFlatternBVHIfDirty(ref builder,
            //                          geoLocalToGlobalIndexOffset: geoLocalToGlobalIndexOffset,
            //                          mappingLocalToGlobalIndexOffset: mappingLocalToGlobalIndexOffset,
            //                          ref meshDirty,
            //                          minNumberOfGeoPerBox,
            //                          normals,
            //                          trianglesVertexOrder,
            //                          vertices,
            //                          ref flattenBVH,
            //                          ref accelerationGeometryMappingCollection,
            //                          ref triangles);

            return SerializeRTMeshBVH(flattenBVH);
        }

        public RTBoundingBox GetTopLevelBoundingBox(int assignedPrimitiveId)
        {
            // Top Level Bounding Box is the box that capsulate the entire Game Object / geometry, not individual geometries (triangles)
            return new RTBoundingBox(leftID: -1,
                                     rightID: -1,
                                     max: m_builder.Root.bounding.max,
                                     min: m_builder.Root.bounding.min,
                                     geoIndices: new HashSet<int> { assignedPrimitiveId });    // There is exactly 1 primitive in this box
        }

        public BVHNode GetRoot()
        {
            return m_builder.Root;
        }

        private float[] GenerateTriangle(Vector3 v0,
                                         Vector3 v1,
                                         Vector3 v2,
                                         Vector3 n0,
                                         Vector3 n1,
                                         Vector3 n2,
                                         Vector2 uv0,
                                         Vector2 uv1,
                                         Vector2 uv2)
        {
            Vector3 wv0 = m_actuator.LocalToWorldVertex(v0);
            Vector3 wv1 = m_actuator.LocalToWorldVertex(v1);
            Vector3 wv2 = m_actuator.LocalToWorldVertex(v2);

            Vector3 wn0 = Vector3.Normalize(m_actuator.LocalToWorldDirection(n0));
            Vector3 wn1 = Vector3.Normalize(m_actuator.LocalToWorldDirection(n1));
            Vector3 wn2 = Vector3.Normalize(m_actuator.LocalToWorldDirection(n2));

            // Vector3 _cross = Vector3.Cross(wv1 - wv0, wv2 - wv0);
            // Vector3 normal = Vector3.Normalize(_cross);
            // float planeD = -1 * Vector3.Dot(normal, wv0);
            // float area = Vector3.Dot(normal, _cross);

            return new float[] {
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

        public void BuildBVHAndTriangleList(int minNumberOfGeoPerBox,
                                            Vector3[] normals,
                                            int[] trianglesVertexOrder,
                                            Vector2[] uvs,
                                            Vector3[] vertices)
        {
            _BuildBVHAndTriangleList(ref m_builder,
                                     minNumberOfGeoPerBox,
                                     normals,
                                     m_triangles,
                                     trianglesVertexOrder,
                                     uvs,
                                     vertices);
        }

        private void _BuildBVHAndTriangleList(ref RTMeshBVHBuilder builder,
                                              int minNumberOfGeoPerBox,
                                              Vector3[] normals,
                                              List<float> triangles,
                                              int[] trianglesVertexOrder,
                                              Vector2[] uvs,
                                              Vector3[] vertices)
        {
            int primitiveCounter = 0;
            builder.Clear();
            triangles.Clear();

            for (int i = 0; i < trianglesVertexOrder.Length; i += 3)
            {
                RTBoundingBox box = RTBoundingBox.RTBoundingBoxFromTriangle(primitiveCounter,
                                                                            m_actuator.LocalToWorldVertex(vertices[trianglesVertexOrder[i]]),
                                                                            m_actuator.LocalToWorldVertex(vertices[trianglesVertexOrder[i + 1]]),
                                                                            m_actuator.LocalToWorldVertex(vertices[trianglesVertexOrder[i + 2]]));
                builder.AddBoundingBox(box);

                triangles.AddRange(GenerateTriangle(vertices[trianglesVertexOrder[i]],
                                                    vertices[trianglesVertexOrder[i + 1]],
                                                    vertices[trianglesVertexOrder[i + 2]],
                                                    normals[trianglesVertexOrder[i]],
                                                    normals[trianglesVertexOrder[i + 1]],
                                                    normals[trianglesVertexOrder[i + 2]],
                                                    uvs[trianglesVertexOrder[i]],
                                                    uvs[trianglesVertexOrder[i + 1]],
                                                    uvs[trianglesVertexOrder[i + 2]]
                                                    ));
                primitiveCounter++;
            }

            builder.Construct(minNumberOfGeoPerBox);
        }

        public void SetMesh()
        {
            m_meshDirty = true;
        }

        public static List<int> SerializeObjectLevelGeoMap(List<List<int>> geoMapCol)
        {
            var result = new List<int>();
            geoMapCol.ForEach(v =>
            {
                result.AddRange(v);
            });
            return result;
        }

        public static List<float> SerializeRTMeshBVH(List<List<float>> flattenBVH)
        {
            var bvhLen = flattenBVH.Count * RTBoundingBox.NUMBER_OF_FLOAT;
            var result = new List<float>(){
                bvhLen
            };
            flattenBVH.ForEach(v =>
            {
                result.AddRange(v);
            });

            return result;
        }
    }
}
