using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpenRT {

    using ISIdx = System.Int32; // IntersectShaderIndex

    public class SceneParser {

        private static SceneParser _sharedInstance = new SceneParser();

        private List<Primitive> m_primitives;
        private SortedList<ISIdx, List<float>> m_geometryInstances;
        private Dictionary<ISIdx, int> m_geometryCount;
        private Dictionary<ISIdx, int> m_geometryStrides;
        private List<RTMaterial> m_materials;

        public static SceneParser Instance {
            get { return _sharedInstance; }
        }

        private SceneParser() {
            m_primitives = new List<Primitive>();
            m_geometryInstances = new SortedList<ISIdx, List<float>>();
            m_geometryCount = new Dictionary<ISIdx, int>();
            m_geometryStrides = new Dictionary<ISIdx, int>();
            m_materials = new List<RTMaterial>();
        }

        public List<Primitive> GetPrimitives() {
            return m_primitives;
        }

        public IEnumerator<KeyValuePair<ISIdx, List<float>>> GetGeometryInstanceIterator() {
            return m_geometryInstances.GetEnumerator();
        }

        public List<float> GetGeometryInstancesByType(int geometryIndex) {
            return m_geometryInstances[geometryIndex];
        }

        public int GetGeometryInstancesCount(int geometryIndex) {
            if (m_geometryCount.ContainsKey(geometryIndex)) {
                return m_geometryCount[geometryIndex];
            } else {
                return 0;
            }
        }

        public int GetGeometryInstancesStride(int geometryIndex) {
            if (m_geometryStrides.ContainsKey(geometryIndex)) {
                return m_geometryStrides[geometryIndex];
            } else {
                return 1;
            }
        }

        public int NumberOfPrimitive() {
            return m_primitives.Count;
        }

        public List<RTMaterial> GetMaterials() {
            return m_materials;
        }

        public void ParseScene(Scene scene) {
            GameObject[] roots = scene.GetRootGameObjects();

            ParseGeometry(roots,
                ref m_primitives,
                ref m_geometryInstances,
                ref m_geometryCount,
                ref m_geometryStrides,
                ref m_materials);
        }

        private void ParseGeometry(GameObject[] roots,
            ref List<Primitive> primitives,
            ref SortedList<ISIdx, List<float>> geoIns,
            ref Dictionary<ISIdx, int> geoCount,
            ref Dictionary<ISIdx, int> geoStride,
            ref List<RTMaterial> mats) {

            // TODO: Optimize dynamic array generation
            primitives.Clear();
            geoIns.Clear();
            geoCount.Clear();
            geoStride.Clear();
            mats.Clear();

            foreach (var root in roots) {
                RTRenderer[] renderers = root.GetComponentsInChildren<RTRenderer>();

                foreach (var renderer in renderers) {
                    if (renderer.gameObject.activeSelf) {
                        List<float> geoInsData = renderer.geometry.GetGeometryInstanceData();
                        var intersectShaderGUID = renderer.geometry.GetIntersectShaderGUID();
                        int intersectShaderIndex = CustomShaderDatabase.Instance.GUIDToShaderIndex(intersectShaderGUID, EShaderType.Intersect);

                        if (!geoStride.ContainsKey(intersectShaderIndex)) {
                            geoStride.Add(intersectShaderIndex, renderer.geometry.GetStride());
                        }

                        RTMaterial material = renderer.material;

                        if (geoInsData == null || material == null) {
                            continue;
                        }

                        if (geoIns.ContainsKey(intersectShaderIndex)) {
                            geoIns[intersectShaderIndex].AddRange(geoInsData);
                        } else {
                            geoIns[intersectShaderIndex] = geoInsData;
                        }

                        for (int t = 0; t < renderer.geometry.GetCount(); t++) {

                            if (geoCount.ContainsKey(intersectShaderIndex)) {
                                geoCount[intersectShaderIndex] += 1;
                            } else {
                                geoCount.Add(intersectShaderIndex, 1);
                            }

                            primitives.Add(new Primitive(
                                geometryIndex: intersectShaderIndex,
                                geometryInstanceIndex: geoCount[intersectShaderIndex] - 1,
                                materialIndex: material.shaderIndex,
                                transformIndex: 0
                            ));
                        }
                        mats.Add(material);
                    }
                }
            }
        }
    }
}