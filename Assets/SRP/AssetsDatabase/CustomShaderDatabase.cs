using System;
using System.Collections.Generic;
using UnityEngine;

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

        public List<CustomShaderMeta> closetHitShaderMetaList {
            get {
                return m_dataTable.GetClosestShaderMetaList;
            }
        }

        public string[] intersectShaderNameList {
            get {
                return m_dataTable.GetIntersectShaderNameList;
            }
        }

        public List<CustomShaderMeta> intersectShaderMetaList {
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
    }
}