using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public class LightShaderDatabaseDataTable {
        private SortedSet<string> m_shaderNameList;
        private SortedSet<CustomShaderMeta> m_shaderMetaList;
        private CustomShaderDatabaseFile m_dbFile;

        public LightShaderDatabaseDataTable() {
            PopulateDataTableFromFile();
        }

        private void PopulateDataTableFromFile() {
            m_dbFile = CustomShaderDatabaseFileIO.ReadDatabaseFromFile();

            m_shaderNameList = new SortedSet<string>();
            m_shaderMetaList = new SortedSet<CustomShaderMeta>(comparer: new CustomShaderMetaComparer());

            foreach (var kvp in m_dbFile.lights) {
                // Shader Name = kvp.Value.name
                // Shader GUID = kvp.Key
                m_shaderNameList.Add(kvp.Value.name);
                m_shaderMetaList.Add(kvp.Value);
            }
        }
    }
}