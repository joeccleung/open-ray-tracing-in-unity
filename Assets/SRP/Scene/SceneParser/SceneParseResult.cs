using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{
    using ISIdx = System.Int32; // IntersectShaderIndex
    public class SceneParseResult
    {
        private SortedList<ISIdx, List<float>> m_geometryData;
        private Dictionary<ISIdx, int> m_geometryCount;
        private Dictionary<ISIdx, bool> m_geometryTypeIsDirty;
        private Dictionary<ISIdx, int> m_geometryStrides;
        private List<RTLightInfo> m_lightInfos;
        private List<RTMaterial> m_materials;
        private List<Primitive> m_primitives;
        private List<Matrix4x4> m_worldToPrimitives;
        private TopLevelBVH topLevelBVH;
        private SortedList<ISIdx, List<float>> m_objectLevelAccelerationStructureData;
        private SortedList<ISIdx, List<float>> m_objectLevelAccelerationStructureGeometryData;
        private SortedList<ISIdx, int> m_objectLevelAccelerationStructureGeometryDataCursor;
        private SortedList<ISIdx, List<int>> m_objectLevelAccelerationStructureGeometryMapping;
        private SortedList<ISIdx, int> m_objectLevelAccelerationStructureGeometryMappingCursor;


        public SceneParseResult()
        {
            m_geometryCount = new Dictionary<ISIdx, int>();
            m_geometryData = new SortedList<ISIdx, List<float>>();
            m_geometryStrides = new Dictionary<ISIdx, int>();
            m_geometryTypeIsDirty = new Dictionary<ISIdx, bool>();
            m_lightInfos = new List<RTLightInfo>();
            m_materials = new List<RTMaterial>();
            m_primitives = new List<Primitive>();
            m_worldToPrimitives = new List<Matrix4x4>();
            topLevelBVH = new TopLevelBVH();
            m_objectLevelAccelerationStructureData = new SortedList<ISIdx, List<float>>();
            m_objectLevelAccelerationStructureGeometryData = new SortedList<ISIdx, List<float>>();
            m_objectLevelAccelerationStructureGeometryDataCursor = new SortedList<ISIdx, int>();
            m_objectLevelAccelerationStructureGeometryMapping = new SortedList<ISIdx, List<int>>();
            m_objectLevelAccelerationStructureGeometryMappingCursor = new SortedList<ISIdx, int>();
        }

        public void AddBoundingBox(RTBoundingBox box)
        {
            topLevelBVH.AddBoundingBox(box);
        }

        public int AddGeometryCount(int count, int intersectIndex)
        {
            int startIndex = 0;
            if (m_geometryCount.ContainsKey(intersectIndex))
            {
                startIndex = m_geometryCount[intersectIndex];
                m_geometryCount[intersectIndex] += count;
            }
            else
            {
                m_geometryCount.Add(intersectIndex, count);
            }
            return startIndex;
        }

        public void AddGeometryData(List<float> geometryData, int intersectIndex)
        {

            if (m_geometryData.ContainsKey(intersectIndex))
            {
                m_geometryData[intersectIndex].AddRange(geometryData);
            }
            else
            {
                m_geometryData[intersectIndex] = geometryData;
            }
        }

        public void AddLight(RTLightInfo lightInfo)
        {
            m_lightInfos.Add(lightInfo);
        }

        public void AddMaterial(RTMaterial material)
        {
            m_materials.Add(material);
        }

        public void AddPrimitive(Primitive primitive)
        {
            m_primitives.Add(primitive);
        }

        public void AddWorldToPrimitive(Matrix4x4 worldToPrimitive)
        {
            m_worldToPrimitives.Add(worldToPrimitive);
        }

        public void AddAccelerationStructureGeometry(List<float> accelerationStructureData,
                                                     List<float> accelGeometryData,
                                                     List<int> accelGeometryMapping,
                                                     int intersectIndex)
        {
            if (m_objectLevelAccelerationStructureData.ContainsKey(intersectIndex))
            {
                m_objectLevelAccelerationStructureData[intersectIndex].AddRange(accelerationStructureData);
            }
            else
            {
                m_objectLevelAccelerationStructureData[intersectIndex] = accelerationStructureData;
            }

            if (m_objectLevelAccelerationStructureGeometryData.ContainsKey(intersectIndex))
            {
                m_objectLevelAccelerationStructureGeometryData[intersectIndex].AddRange(accelGeometryData);
                m_objectLevelAccelerationStructureGeometryDataCursor[intersectIndex] = m_objectLevelAccelerationStructureGeometryData[intersectIndex].Count;
            }
            else
            {
                m_objectLevelAccelerationStructureGeometryData[intersectIndex] = accelGeometryData;
                m_objectLevelAccelerationStructureGeometryDataCursor[intersectIndex] = m_objectLevelAccelerationStructureGeometryData[intersectIndex].Count;
            }

            if (m_objectLevelAccelerationStructureGeometryMapping.ContainsKey(intersectIndex))
            {
                m_objectLevelAccelerationStructureGeometryMapping[intersectIndex].AddRange(accelGeometryMapping);
                m_objectLevelAccelerationStructureGeometryMappingCursor[intersectIndex] = m_objectLevelAccelerationStructureGeometryMapping[intersectIndex].Count;
            }
            else
            {
                m_objectLevelAccelerationStructureGeometryMapping[intersectIndex] = accelGeometryMapping;
                m_objectLevelAccelerationStructureGeometryMappingCursor[intersectIndex] = m_objectLevelAccelerationStructureGeometryMapping[intersectIndex].Count;
            }
        }

        public void ClearTopLevelBVH()
        {
            topLevelBVH.Clear();
        }

        public void ClearAllGeometries()
        {
            m_geometryData.Clear();
            m_geometryCount.Clear();
            m_geometryStrides.Clear();
            m_objectLevelAccelerationStructureData.Clear();
            m_objectLevelAccelerationStructureGeometryData.Clear();
            m_objectLevelAccelerationStructureGeometryDataCursor.Clear();
            m_objectLevelAccelerationStructureGeometryMapping.Clear();
            m_objectLevelAccelerationStructureGeometryMappingCursor.Clear();
        }

        public void ClearAllLights()
        {
            m_lightInfos.Clear();
        }

        public void ClearAllMaterials()
        {
            m_materials.Clear();
        }

        public void ClearAllPrimitives()
        {
            m_primitives.Clear();
            m_worldToPrimitives.Clear();
        }

        public Dictionary<ISIdx, int> GeometryCount
        {
            get
            {
                return m_geometryCount;
            }
        }

        public SortedList<ISIdx, List<float>> GeometryInstances
        {
            get
            {
                return m_geometryData;
            }
        }

        public Dictionary<ISIdx, int> GeometryStride
        {
            get
            {
                return m_geometryStrides;
            }
        }

        public int GetGeometryInstancesCount(int geometryIndex)
        {
            if (m_geometryCount.ContainsKey(geometryIndex))
            {
                return m_geometryCount[geometryIndex];
            }
            else
            {
                return 0;
            }
        }

        public int GetGeometryInstancesStride(int geometryIndex)
        {
            if (m_geometryStrides.ContainsKey(geometryIndex))
            {
                return m_geometryStrides[geometryIndex];
            }
            else
            {
                return 1;
            }
        }

        public List<RTLightInfo> Lights
        {
            get
            {
                return m_lightInfos;
            }
        }

        public List<RTMaterial> Materials
        {
            get
            {
                return m_materials;
            }
        }

        public int ObjectLevelAccGeoCursor(int intersectShaderIdx)
        {
            if (m_objectLevelAccelerationStructureGeometryDataCursor.ContainsKey(intersectShaderIdx))
            {
                return m_objectLevelAccelerationStructureGeometryDataCursor[intersectShaderIdx];
            }
            else
            {
                return 0;
            }
        }

        public int ObjectLevelAccGeoMapCursor(int intersectShaderIdx)
        {
            if (m_objectLevelAccelerationStructureGeometryMappingCursor.ContainsKey(intersectShaderIdx))
            {
                return m_objectLevelAccelerationStructureGeometryMappingCursor[intersectShaderIdx];
            }
            else
            {
                return 0;
            }
        }

        public List<Primitive> Primitives
        {
            get
            {
                return m_primitives;
            }
        }

        public List<Matrix4x4> WorldToPrimitive
        {
            get
            {
                return m_worldToPrimitives;
            }
        }

        public TopLevelBVH TopLevelBVH
        {
            get
            {
                return topLevelBVH;
            }
        }

        public SortedList<ISIdx, List<float>> ObjectLevelAccelerationGeometries
        {
            get
            {
                return m_objectLevelAccelerationStructureGeometryData;
            }
        }

        public SortedList<ISIdx, List<int>> ObjectLevelAccelerationGeometryMapping
        {
            get
            {
                return m_objectLevelAccelerationStructureGeometryMapping;
            }
        }

        public SortedList<ISIdx, List<float>> ObjectLevelAccelerationStructures
        {
            get
            {
                return m_objectLevelAccelerationStructureData;
            }
        }

    }
}