using System;
using System.Collections.Generic;
using UnityEngine;
using GUID = System.String;

namespace OpenRT {
    public class CustomShaderDatabase {

        private static CustomShaderDatabase sharedInstance;

        public static CustomShaderDatabase Instance {
            get {
                if (sharedInstance == null) {
                    sharedInstance = new CustomShaderDatabase();
                }
                return sharedInstance;
            }
        }

        private CustomShaderDatabaseDataTable m_dataTable;

        public CustomShaderDatabase() {
            m_dataTable = new CustomShaderDatabaseDataTable();
        }

        public string[] closestHitShaderNameList {
            get {
                return m_dataTable.GetClosestShaderNameList;
            }
        }

        public SortedList<GUID, CustomShaderMeta> closetHitShaderMetaList {
            get {
                return m_dataTable.GetClosestShaderMetaList;
            }
        }

        public string[] intersectShaderNameList {
            get {
                return m_dataTable.GetIntersectShaderNameList;
            }
        }

        public SortedList<GUID, CustomShaderMeta> intersectShaderMetaList {
            get {
                return m_dataTable.GetIntersectShaderMetaList;
            }
        }

        public void Add(CustomShaderMeta meta) {
            if (m_dataTable.ContainsThisShader(meta.shaderType, meta.name)) {
                // TODO: Support Update
                Debug.LogWarning("TODO: Support shader update");
            } else {
                switch (meta.shaderType) {
                    case OpenRT.EShaderType.CloestHit:
                        m_dataTable.AddClosetHitShader(meta);
                        break;

                    case OpenRT.EShaderType.Intersect:
                        m_dataTable.AddIntersectShader(meta);
                        break;

                    default:
                        // TODO: Support adding shaders of type {meta.shaderType}
                        Debug.LogWarning($"TODO: Support adding shaders of type {meta.shaderType}");
                        break;
                }

            }
        }

        public string GUIDToShaderName(GUID guid, EShaderType shaderType) {
            switch (shaderType) {
                case EShaderType.CloestHit:
                    return m_dataTable.GUIDToClosestHitShaderName(guid);

                case EShaderType.Intersect:
                    return m_dataTable.GUIDToIntersectShaderName(guid);

                default:
                    return string.Empty;
            }
        }

        public int GUIDToShaderIndex(GUID guid, EShaderType shaderType)
        {
            switch (shaderType) {
                case EShaderType.CloestHit:
                    return m_dataTable.GUIDToClosestHitShaderIndex(guid);

                case EShaderType.Intersect:
                    return m_dataTable.GUIDToIntersectShaderIndex(guid);

                default:
                    return -1;
            }
        }

        public string ShaderNameToGUID(string shaderName, EShaderType shaderType) {
            switch (shaderType) {
                case EShaderType.CloestHit:
                    return m_dataTable.ClosestHitShaderNameToGUID(shaderName);

                case EShaderType.Intersect:
                    return m_dataTable.IntersectShaderNameToGUID(shaderName);

                default:
                    return string.Empty;
            }
        }
    }
}