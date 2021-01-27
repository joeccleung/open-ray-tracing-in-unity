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
                if (r.geometry == null)
                {
                    return;
                }
                isDirty |= r.geometry.IsDirty();
            });

            return isDirty;
        }

        public SceneParseResult ParseScene(Scene scene)
        {
            GameObject[] roots = scene.GetRootGameObjects();

            ParseGeometry(roots,
                          ref sceneParseResult);

            ParseLight(ref sceneParseResult);

            sceneParseResult.TopLevelBVH.Construct();

            return sceneParseResult;
        }

        private void ParseLight(ref SceneParseResult sceneParseResult)
        {
            sceneParseResult.ClearAllLights();

            // Placeholder for scene parsing
            sceneParseResult.AddLight(new RTLightInfo(
                instance: 0,
                position: new Vector3(1, 1, 1),
                rotation: new Vector3(0, 0, -1),
                type: 0
            ));

            sceneParseResult.AddLight(new RTLightInfo(
                instance: 0,
                position: new Vector3(0, 0, 0),
                rotation: new Vector3(0, 0, 0),
                type: 1
            ));
        }

        private void ParseGeometry(
            GameObject[] roots,
            ref SceneParseResult sceneParseResult)
        {
            var renderers = GetAllRenderers(roots);

            if (!IsAllGeometriesDirty(renderers))
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
                if (renderer.gameObject.activeSelf)
                {
                    RTMaterial material = renderer.material;
                    if (!renderer.geometry.IsGeometryValid() || material == null)
                    {
                        continue;
                    }

                    int closestShaderIndex = 0;
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

                    sceneParseResult.AddWorldToPrimitive(renderer.gameObject.transform.worldToLocalMatrix);

                    sceneParseResult.AddPrimitive(new Primitive(
                        geometryIndex: intersectShaderIndex,
                        geometryInstanceBegin: startIndex,
                        geometryInstanceCount: renderer.geometry.GetCount(),
                        materialIndex: closestShaderIndex,
                        transformIndex: sceneParseResult.WorldToPrimitive.Count - 1
                    ));

                    var boxOfThisObject = renderer.geometry.GetTopLevelBoundingBox(assginedPrimitiveId: sceneParseResult.Primitives.Count - 1);
                    sceneParseResult.AddBoundingBox(boxOfThisObject);

                    sceneParseResult.AddMaterial(material);
                }
            }
        }
    }
}