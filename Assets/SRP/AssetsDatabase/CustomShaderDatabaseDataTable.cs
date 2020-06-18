using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenRT;
using UnityEngine;

namespace OpenRT {
    using GUID = System.String;
    using ShaderName = System.String;
    public class CustomShaderDatabaseDataTable {

        // TODO: Split DatabaseDataTable by Shader Type
        private SortedList<ShaderName, GUID> m_closetHitShaderList;
        private SortedList<GUID, CustomShaderMeta> m_closetHitShaderMetaList;
        private SortedList<ShaderName, GUID> m_intersectShaderNameToGUID;
        private SortedList<GUID, CustomShaderMeta> m_intersectShaderMetaList;
        private CustomShaderDatabaseFile m_dbFile;

        public CustomShaderDatabaseDataTable() {
            PopulateDataTableFromFile();
        }

        public void AddClosetHitShader(CustomShaderMeta shaderMeta) {
            var guid = Guid.NewGuid().ToString();
            m_closetHitShaderList.Add(shaderMeta.name, guid);
            m_closetHitShaderMetaList.Add(guid, shaderMeta);
            m_dbFile.closetHit.Add(guid, shaderMeta);

            CustomShaderDatabaseFileIO.WriteDatabaseToFile(m_dbFile);
        }

        public void AddIntersectShader(CustomShaderMeta shaderMeta) {
            string guid;
            do {
                guid = Guid.NewGuid().ToString();
            } while (m_intersectShaderMetaList.ContainsKey(guid));
            m_intersectShaderMetaList.Add(guid, shaderMeta);
            m_intersectShaderNameToGUID.Add(shaderMeta.name, guid);
            m_dbFile.intersect.Add(guid, shaderMeta);

            CustomShaderDatabaseFileIO.WriteDatabaseToFile(m_dbFile);
        }

        public bool ContainsThisShader(EShaderType shaderType, string shaderName) {

            switch (shaderType) {
                case EShaderType.CloestHit:
                    return m_closetHitShaderMetaList.Any((kvp) => {
                        return kvp.Value.name == shaderName;
                    });

                case EShaderType.Intersect:
                    return m_closetHitShaderMetaList.Any((kvp) => {
                        return kvp.Value.name == shaderName;
                    });

                default:
                    Debug.LogWarning($"Unsupported shader type {shaderType}");
                    return false;
            }

        }

        public string[] GetClosestShaderNameList {
            get {
                return m_closetHitShaderList.Keys.ToArray();
            }
        }

        public int GUIDToClosestHitShaderIndex(GUID guid) {
            var name = GUIDToClosestHitShaderName(guid);
            return m_closetHitShaderList.IndexOfKey(name);
        }

        public string GUIDToClosestHitShaderName(GUID guid) {
            return m_closetHitShaderMetaList[guid].name;
        }

        public GUID ClosestHitShaderNameToGUID(string shaderName) {
            return m_closetHitShaderList[shaderName];
        }

        public SortedList<GUID, CustomShaderMeta> GetClosestShaderMetaList {
            get {
                return m_closetHitShaderMetaList;
            }
        }

        public string[] GetIntersectShaderNameList {
            get {
                return m_intersectShaderNameToGUID.Keys.ToArray();
            }
        }

        public int GUIDToIntersectShaderIndex(GUID guid) {
            var name = GUIDToIntersectShaderName(guid);
            if (m_intersectShaderNameToGUID.ContainsKey(name)) {
                return m_intersectShaderNameToGUID.IndexOfKey(name);
            } else {
                return -1;
            }
        }

        public string GUIDToIntersectShaderName(GUID guid) {
            if (m_intersectShaderMetaList.ContainsKey(guid)) {
                return m_intersectShaderMetaList[guid].name;
            } else {
                return string.Empty;
            }
        }

        public GUID IntersectShaderNameToGUID(string shaderName) {
            return m_intersectShaderNameToGUID[shaderName];
        }

        public SortedList<GUID, CustomShaderMeta> GetIntersectShaderMetaList {
            get {
                return m_intersectShaderMetaList;
            }
        }

        private void PopulateDataTableFromFile() {
            m_dbFile = CustomShaderDatabaseFileIO.ReadDatabaseFromFile();

            m_closetHitShaderList = new SortedList<ShaderName, GUID>();
            m_closetHitShaderMetaList = new SortedList<GUID, CustomShaderMeta>(comparer: new CustomShaderMetaGUIDComparer());

            m_intersectShaderNameToGUID = new SortedList<ShaderName, GUID>();
            m_intersectShaderMetaList = new SortedList<GUID, CustomShaderMeta>(comparer: new CustomShaderMetaGUIDComparer());

            foreach (var kvp in m_dbFile.closetHit) {
                // Shader Name = kvp.Value.name
                // Shader GUID = kvp.Key
                m_closetHitShaderList.Add(kvp.Value.name, kvp.Key);
                m_closetHitShaderMetaList.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in m_dbFile.intersect) {
                // Shader Name = kvp.Value.name
                // Shader GUID = kvp.Key
                m_intersectShaderNameToGUID.Add(kvp.Value.name, kvp.Key);
                m_intersectShaderMetaList.Add(kvp.Key, kvp.Value);
            }
        }
    }
}