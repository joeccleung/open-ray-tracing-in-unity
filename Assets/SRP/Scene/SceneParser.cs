using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpenRT {

    using ISIdx = System.Int32; // IntersectShaderIndex

    public class SceneParser {

        private static SceneParser _sharedInstance = new SceneParser();

        public SceneParseResult sceneParseResult;

        public static SceneParser Instance {
            get { return _sharedInstance; }
        }

        private SceneParser() {
            sceneParseResult = new SceneParseResult();
        }

        public SceneParseResult ParseScene(Scene scene) {
            GameObject[] roots = scene.GetRootGameObjects();

            ParseGeometry(roots,
                          ref sceneParseResult);

            ParseLight(ref sceneParseResult);

            sceneParseResult.TopLevelBVH.Construct();

            return sceneParseResult;
        }

        private void ParseLight(ref SceneParseResult sceneParseResult) {
            sceneParseResult.ClearAllLights();

            // Placeholder for scene parsing
            sceneParseResult.AddLight(new RTLightInfo(
                instance: 0,
                position: new Vector3(1, 1, 1),
                rotation : new Vector3(45, 45, 45),
                type : 0
            ));

            sceneParseResult.AddLight(new RTLightInfo(
                instance: 0,
                position: new Vector3(0, 0, -3),
                rotation : new Vector3(0, 0, 0),
                type : 1
            ));
        }

        private void ParseGeometry(
            GameObject[] roots,
            ref SceneParseResult sceneParseResult) {

            // TODO: Optimize dynamic array generation
            sceneParseResult.ClearAllPrimitives();
            sceneParseResult.ClearAllGeometries();
            sceneParseResult.ClearAllMaterials();
            sceneParseResult.ClearTopLevelBVH();

            foreach (var root in roots) {
                RTRenderer[] renderers = root.GetComponentsInChildren<RTRenderer>();

                foreach (var renderer in renderers) {
                    if (renderer.gameObject.activeSelf) {
                        List<float> geoInsData = renderer.geometry.GetGeometryInstanceData();
                        var intersectShaderGUID = renderer.geometry.GetIntersectShaderGUID();
                        int intersectShaderIndex = CustomShaderDatabase.Instance.GUIDToShaderIndex(intersectShaderGUID, EShaderType.Intersect);

                        if (!sceneParseResult.GeometryStride.ContainsKey(intersectShaderIndex)) {
                            sceneParseResult.GeometryStride.Add(intersectShaderIndex, renderer.geometry.GetStride());
                        }

                        RTMaterial material = renderer.material;

                        if (geoInsData == null || material == null) {
                            continue;
                        }

                        if (sceneParseResult.GeometryInstances.ContainsKey(intersectShaderIndex)) {
                            sceneParseResult.GeometryInstances[intersectShaderIndex].AddRange(geoInsData);
                        } else {
                            sceneParseResult.GeometryInstances[intersectShaderIndex] = geoInsData;
                        }

                        List<int> primitiveIdsOfThisBox = new List<int>();
                        for (int t = 0; t < renderer.geometry.GetCount(); t++) {

                            if (sceneParseResult.GeometryCount.ContainsKey(intersectShaderIndex)) {
                                sceneParseResult.GeometryCount[intersectShaderIndex] += 1;
                            } else {
                                sceneParseResult.GeometryCount.Add(intersectShaderIndex, 1);
                            }

                            int primitiveIndex = sceneParseResult.Primitives.Count;
                            primitiveIdsOfThisBox.Add(primitiveIndex);

                            sceneParseResult.AddPrimitive(new Primitive(
                                geometryIndex: intersectShaderIndex,
                                geometryInstanceIndex: sceneParseResult.GeometryCount[intersectShaderIndex] - 1,
                                materialIndex: material.shaderIndex,
                                transformIndex: 0
                            ));
                        }
                        var boxOfThisObject = renderer.geometry.GetBoundingBox();
                        sceneParseResult.AddBoundingBox(new RTBoundingBox(
                            max: boxOfThisObject.max,
                            min: boxOfThisObject.min,
                            primitiveIds: primitiveIdsOfThisBox
                        ));
                        sceneParseResult.AddMaterial(material);
                    }
                }
            }
        }
    }
}