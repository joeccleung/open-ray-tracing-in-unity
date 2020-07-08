using System.Collections.Generic;

namespace OpenRT {
    using ISIdx = System.Int32; // IntersectShaderIndex
    public class SceneParseResult {
        private SortedList<ISIdx, List<float>> m_geometryInstances;
        private Dictionary<ISIdx, int> m_geometryCount;
        private Dictionary<ISIdx, int> m_geometryStrides;
        private List<RTLightInfo> m_lightInfos;
        private List<RTMaterial> m_materials;
        private List<Primitive> m_primitives;

        public SceneParseResult() {
            m_geometryCount = new Dictionary<ISIdx, int>();
            m_geometryInstances = new SortedList<ISIdx, List<float>>();
            m_geometryStrides = new Dictionary<ISIdx, int>();
            m_lightInfos = new List<RTLightInfo>();
            m_materials = new List<RTMaterial>();
            m_primitives = new List<Primitive>();
        }

        public Dictionary<ISIdx, int> GeometryCount {
            get {
                return m_geometryCount;
            }
        }
        public SortedList<ISIdx, List<float>> GeometryInstances {
            get {
                return m_geometryInstances;
            }
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

        public Dictionary<ISIdx, int> GeometryStride {
            get {
                return m_geometryStrides;
            }
        }
        public List<RTLightInfo> Lights {
            get {
                return m_lightInfos;
            }
        }

        public List<RTMaterial> Materials {
            get {
                return m_materials;
            }
        }

        public List<Primitive> Primitives {
            get {
                return m_primitives;
            }
        }

        public void AddLight(RTLightInfo lightInfo) {
            m_lightInfos.Add(lightInfo);
        }

        public void AddMaterial(RTMaterial material) {
            m_materials.Add(material);
        }

        public void AddPrimitive(Primitive primitive) {
            m_primitives.Add(primitive);
        }

        public void ClearAllGeometries() {
            m_geometryInstances.Clear();
            m_geometryCount.Clear();
            m_geometryStrides.Clear();
        }

        public void ClearAllLights() {
            m_lightInfos.Clear();
        }

        public void ClearAllMaterials() {
            m_materials.Clear();
        }

        public void ClearAllPrimitives() {
            m_primitives.Clear();
        }
    }
}