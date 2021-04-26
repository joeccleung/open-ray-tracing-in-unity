using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpenRT
{

    using ISIdx = System.Int32; // IntersectShaderIndex

    public class SceneParser
    {

        private static SceneParser _sharedInstance = new SceneParser();

        public SceneParseResult sceneParseResult;

        public static SceneParser Instance
        {
            get { return _sharedInstance; }
        }

        private SceneParser()
        {
            sceneParseResult = new SceneParseResult();
        }

        public List<RTLight> GetAllLights(GameObject[] roots)
        {
            List<RTLight> lights = new List<RTLight>();

            foreach (var root in roots)
            {
                lights.AddRange(root.GetComponentsInChildren<RTLight>());
            }

            return lights;
        }

        public List<RTRenderer> GetAllRenderers(GameObject[] roots)
        {
            List<RTRenderer> renderers = new List<RTRenderer>();

            foreach (var root in roots)
            {
                renderers.AddRange(root.GetComponentsInChildren<RTRenderer>());
            }

            return renderers;
        }

        public bool IsAllGeometriesDirty(List<RTRenderer> renderers)
        {
            bool isDirty = false;

            renderers.ForEach(r =>
            {
                if (r.geometry != null)
                {
                    isDirty |= r.geometry.IsDirty();
                }
            });

            return isDirty;
        }

        public bool IsAllLightsDirty(List<RTLight> lights)
        {
            bool isDirty = false;

            lights.ForEach(r =>
            {
                isDirty |= r.IsDirty();
            });

            return isDirty;
        }

        public SceneParseResult ParseScene(Scene scene)
        {
            GameObject[] roots = scene.GetRootGameObjects();

            ParseGeometry(roots,
                          ref sceneParseResult);

            ParseLight(
                roots,
                ref sceneParseResult);

            sceneParseResult.TopLevelBVH.Construct();

            return sceneParseResult;
        }

        private void ParseLight(
            GameObject[] roots,
            ref SceneParseResult sceneParseResult)
        {
            var lights = GetAllLights(roots);

            if (!IsAllLightsDirty(lights))
            {
                // All the lights are unchange, no need to rebuild
                return;
            }

            sceneParseResult.ClearAllLights();

            foreach (var light in lights)
            {
                if (light.gameObject.activeInHierarchy)
                {
                    int lightInstanceIndex = sceneParseResult.AddLight(light);
                }
            }
        }

        private void ParseGeometry(
            GameObject[] roots,
            ref SceneParseResult sceneParseResult)
        {
            var renderers = GetAllRenderers(roots);

            if (!IsAllGeometriesDirty(renderers) && sceneParseResult.Primitives.Count != 0)
            {
                // All the geometries are unchange, no need to rebuild
                return;
            }

            // TODO: Optimize dynamic array generation
            sceneParseResult.ClearAllPrimitives();
            sceneParseResult.ClearAllGeometries();
            sceneParseResult.ClearAllMaterials();
            sceneParseResult.ClearTopLevelBVH();
 
            foreach (var renderer in renderers)
            {
                if (renderer.gameObject.activeInHierarchy)
                {
                    RTMaterial material = renderer.material;
                    if (renderer.geometry == null || !renderer.geometry.IsGeometryValid() || material == null)
                    {
                        continue;
                    }

                    var closestShaderGUID = renderer.material.GetClosestHitGUID();
                    int closestShaderIndex = CustomShaderDatabase.Instance.GUIDToShaderIndex(closestShaderGUID, EShaderType.ClosestHit);
                    var intersectShaderGUID = renderer.geometry.GetIntersectShaderGUID();
                    int intersectShaderIndex = CustomShaderDatabase.Instance.GUIDToShaderIndex(intersectShaderGUID, EShaderType.Intersect);

                    if (!sceneParseResult.GeometryStride.ContainsKey(intersectShaderIndex))
                    {
                        sceneParseResult.GeometryStride.Add(intersectShaderIndex, renderer.geometry.IsAccelerationStructure() ? 0 : renderer.geometry.GetStride());
                    }

                    if (renderer.geometry.IsAccelerationStructure())
                    {
                        // Such as Low-Level BVH (RTMeshBVH)
                        int mapOffset = sceneParseResult.ObjectLevelAccGeoMapCursor(intersectShaderIndex);
                        int geoOffset = sceneParseResult.ObjectLevelAccGeoCursor(intersectShaderIndex);
                        ((IRTMeshBVH)(renderer.geometry)).BuildBVHAndTriangleList(geoLocalToGlobalIndexOffset: geoOffset,
                                                                                  mappingLocalToGlobalIndexOffset: mapOffset);

                        List<float> geoInsData = renderer.geometry.GetGeometryInstanceData(geoLocalToGlobalIndexOffset: geoOffset,
                                                                                           mappingLocalToGlobalIndexOffset: mapOffset);
                        sceneParseResult.AddAccelerationStructureGeometry(
                            accelerationStructureData: geoInsData,
                            accelGeometryMapping: renderer.geometry.GetAccelerationStructureGeometryMapping(geoLocalToGlobalIndexOffset: geoOffset,
                                                                                                            mappingLocalToGlobalIndexOffset: mapOffset),
                            accelGeometryData: renderer.geometry.GetAccelerationStructureGeometryData(geoLocalToGlobalIndexOffset: geoOffset,
                                                                                                      mappingLocalToGlobalIndexOffset: mapOffset),
                            intersectIndex: intersectShaderIndex
                        );
                    }
                    else
                    {
                        // Standardized Geometry (Sphere, Triangle)
                        List<float> geoInsData = renderer.geometry.GetGeometryInstanceData(geoLocalToGlobalIndexOffset: 0, mappingLocalToGlobalIndexOffset: 0);  // No offset
                        sceneParseResult.AddGeometryData(
                            geometryData: geoInsData,
                            intersectIndex: intersectShaderIndex
                        );
                    }

                    int startIndex = sceneParseResult.AddGeometryCount(
                        count: renderer.geometry.GetCount(),
                        intersectIndex: intersectShaderIndex
                    );

                    int materialInstanceIndex = sceneParseResult.AddMaterial(material);

                    sceneParseResult.AddWorldToPrimitive(renderer.gameObject.transform.worldToLocalMatrix);

                    sceneParseResult.AddPrimitive(new Primitive(
                        geometryIndex: intersectShaderIndex,
                        geometryInstanceBegin: startIndex,
                        geometryInstanceCount: renderer.geometry.GetCount(),
                        materialIndex: closestShaderIndex,
                        materialInstanceIndex: materialInstanceIndex,
                        transformIndex: sceneParseResult.WorldToPrimitive.Count - 1
                    ));

                    var boxOfThisObject = renderer.geometry.GetTopLevelBoundingBox(assginedPrimitiveId: sceneParseResult.Primitives.Count - 1);
                    sceneParseResult.AddBoundingBox(boxOfThisObject);
                }
            }
        }
    }
}