﻿using System.Collections;
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

            // TODO: Optimize dynamic array generation
            sceneParseResult.ClearAllPrimitives();
            sceneParseResult.ClearAllGeometries();
            sceneParseResult.ClearAllMaterials();
            sceneParseResult.ClearTopLevelBVH();

            foreach (var root in roots)
            {
                RTRenderer[] renderers = root.GetComponentsInChildren<RTRenderer>();

                foreach (var renderer in renderers)
                {
                    if (renderer.gameObject.activeSelf)
                    {
                        var geometry = renderer.geometry;
                        List<float> geoInsData = renderer.geometry.GetGeometryInstanceData();
                        var closestShaderGUID = renderer.material.GetClosestHitGUID();
                        int closestShaderIndex = CustomShaderDatabase.Instance.GUIDToShaderIndex(closestShaderGUID, EShaderType.ClosestHit);
                        var intersectShaderGUID = renderer.geometry.GetIntersectShaderGUID();
                        int intersectShaderIndex = CustomShaderDatabase.Instance.GUIDToShaderIndex(intersectShaderGUID, EShaderType.Intersect);

                        RTMaterial material = renderer.material;

                        if (geoInsData == null || material == null)
                        {
                            continue;
                        }

                        if (!sceneParseResult.GeometryStride.ContainsKey(intersectShaderIndex))
                        {
                            sceneParseResult.GeometryStride.Add(intersectShaderIndex, geometry.IsUnevenStride() ? 0 : renderer.geometry.GetStride());
                        }

                        if (geometry.IsUnevenStride())
                        {
                            // Such as Low-Level BVH (RTMeshBVH)
                            sceneParseResult.AddVaryingSizeGeometry(
                                geometryData: geoInsData,
                                intersectIndex: intersectShaderIndex
                            );
                        }
                        else
                        {
                            // Standardized Geometry (Sphere, Triangle)
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

                        var boxOfThisObject = renderer.geometry.GetBoundingBox();
                        sceneParseResult.AddBoundingBox(new RTBoundingBox(
                            max: boxOfThisObject.max,
                            min: boxOfThisObject.min,
                            primitive: sceneParseResult.Primitives.Count - 1
                        ));

                        sceneParseResult.AddMaterial(material);
                    }
                }
            }
        }
    }
}