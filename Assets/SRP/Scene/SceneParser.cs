using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpenRT {
    public class SceneParser {

        private static SceneParser _sharedInstance = new SceneParser();

        private List<RTTriangle_t> m_triangleGeom;
        private List<RTMaterial> m_materials;

        public static SceneParser Instance {
            get { return _sharedInstance; }
        }

        private SceneParser() {
            m_triangleGeom = new List<RTTriangle_t>();
            m_materials = new List<RTMaterial>();
        }

        public List<RTMaterial> GetMaterials() {
            return m_materials;
        }

        public List<RTTriangle_t> GetTriangles() {
            return m_triangleGeom;
        }

        public void ParseScene(Scene scene) {
            GameObject[] roots = scene.GetRootGameObjects();

            ParseMesh(roots, ref m_triangleGeom, ref m_materials);
        }

        private void ParseMesh(GameObject[] roots, ref List<RTTriangle_t> tris, ref List<RTMaterial> mats) {

            // TODO: Optimize dynamic array generation
            tris.Clear();

            foreach (var root in roots) {
                RTMeshRenderer[] meshRenderers = root.GetComponentsInChildren<RTMeshRenderer>();

                foreach (var renderer in meshRenderers) {
                    if (renderer.gameObject.activeSelf) {
                        List<RTTriangle_t> allTrianglesInMesh;
                        RTMaterial material;
                        renderer.GetGeometry(tris: out allTrianglesInMesh, mat: out material);

                        if (allTrianglesInMesh == null) {
                            continue;
                        }

                        for (int t = 0; t < allTrianglesInMesh.Count; t++) {
                            allTrianglesInMesh[t].SetId(idInput: tris.Count);
                            tris.Add(allTrianglesInMesh[t]);
                        }
                        mats.Add(material);
                    }
                }
            }
        }
    }
}