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
        private Dictionary<string, int> m_materialsInstancesCount;
        public Dictionary<string, List<Color>> m_materialsColorList;
        public Dictionary<string, List<float>> m_materialsFloatList;
        public Dictionary<string, List<int>> m_materialsIntList;
        public Dictionary<string, List<Vector2>> m_materialsVector2List;
        public Dictionary<string, List<Vector3>> m_materialsVector3List;
        public Dictionary<string, List<Vector4>> m_materialsVector4List;
        public List<Texture2D> m_textureCollection;
        public Dictionary<string, List<int>> m_materialsTextureIndexList;

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
            m_materialsInstancesCount = new Dictionary<string, int>();
            m_materialsColorList = new Dictionary<string, List<Color>>();
            m_materialsFloatList = new Dictionary<string, List<float>>();
            m_materialsIntList = new Dictionary<string, List<int>>();
            m_materialsVector2List = new Dictionary<string, List<Vector2>>();
            m_materialsVector3List = new Dictionary<string, List<Vector3>>();
            m_materialsVector4List = new Dictionary<string, List<Vector4>>();
            m_textureCollection = new List<Texture2D>();
            m_materialsTextureIndexList = new Dictionary<string, List<ISIdx>>();
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

        /// <summary>
        /// Register the material from a geometry, return the material index of that geometry relative to the material group
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        public int AddMaterial(RTMaterial material)
        {
            var closestHitGUID = material.GetClosestHitGUID();

            m_materials.Add(material);
            if (m_materialsInstancesCount.ContainsKey(closestHitGUID))
            {
                // Material already exists
                m_materialsInstancesCount[closestHitGUID]++;
            }
            else
            {
                // First time
                m_materialsInstancesCount.Add(closestHitGUID, 1);
            }
            return m_materialsInstancesCount[closestHitGUID] - 1; // 0 index based
        }

        public void AddMaterialColor(string name, Color color)
        {
            if (m_materialsColorList.ContainsKey(name))
            {
                m_materialsColorList[name].Add(color);
            }
            else
            {
                m_materialsColorList.Add(name, new List<Color>() { color });
            }
        }

        public void AddMaterialFloat(string name, float value)
        {
            if (m_materialsFloatList.ContainsKey(name))
            {
                m_materialsFloatList[name].Add(value);
            }
            else
            {
                m_materialsFloatList.Add(name, new List<float>() { value });
            }
        }

        public void AddMaterialInt(string name, int value)
        {
            if (m_materialsIntList.ContainsKey(name))
            {
                m_materialsIntList[name].Add(value);
            }
            else
            {
                m_materialsIntList.Add(name, new List<int>() { value });
            }
        }

        public void AddMaterialVector2(string name, Vector2 value)
        {
            if (m_materialsVector2List.ContainsKey(name))
            {
                m_materialsVector2List[name].Add(value);
            }
            else
            {
                m_materialsVector2List.Add(name, new List<Vector2>() { value });
            }
        }

        public void AddMaterialVector3(string name, Vector3 value)
        {
            if (m_materialsVector3List.ContainsKey(name))
            {
                m_materialsVector3List[name].Add(value);
            }
            else
            {
                m_materialsVector3List.Add(name, new List<Vector3>() { value });
            }
        }

        public void AddMaterialVector4(string name, Vector4 value)
        {
            if (m_materialsVector4List.ContainsKey(name))
            {
                m_materialsVector4List[name].Add(value);
            }
            else
            {
                m_materialsVector4List.Add(name, new List<Vector4>() { value });
            }
        }

        public void AddMaterialTexture(string name, Texture2D texture)
        {
            int index = -1;
            for (int t = 0; t < m_textureCollection.Count; t++) {
                if (texture.GetInstanceID() == m_textureCollection[t].GetInstanceID()) {
                    index = t;
                    break;
                }
            }
            if (index == -1) {
                m_textureCollection.Add(texture);
                index = m_textureCollection.Count - 1;
            }

            if (m_materialsTextureIndexList.ContainsKey(name))
            {
                m_materialsTextureIndexList[name].Add(index);
            }
            else
            {
                m_materialsTextureIndexList.Add(name, new List<int>() { index });
            }
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

            m_materials.Clear();
            m_materialsInstancesCount.Clear();
        }

        public void ClearAllLights()
        {
            m_lightInfos.Clear();
        }

        public void ClearAllMaterials()
        {
            m_materialsColorList.Clear();
            m_materialsFloatList.Clear();
            m_materialsIntList.Clear();
            m_materialsVector2List.Clear();
            m_materialsVector3List.Clear();
            m_materialsVector4List.Clear();
            m_materialsTextureIndexList.Clear();
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