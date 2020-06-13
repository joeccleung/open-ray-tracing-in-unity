using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenRT;
using UnityEngine;

public class CustomShaderDatabaseDataTable {

    private SortedSet<string> m_closetHitShaderList;
    private SortedSet<CustomShaderMeta> m_closetHitShaderMetaList;
    private SortedSet<string> m_intersectShaderList;
    private SortedSet<CustomShaderMeta> m_intersectShaderMetaList;
    private CustomShaderDatabaseFile m_dbFile;

    public CustomShaderDatabaseDataTable() {
        PopulateDataTableFromFile();
    }

    public void AddClosetHitShader(CustomShaderMeta shaderMeta) {
        var guid = Guid.NewGuid().ToString();
        m_closetHitShaderList.Add(shaderMeta.name);
        m_closetHitShaderMetaList.Add(shaderMeta);
        m_dbFile.closetHit.Add(guid, shaderMeta);

        CustomShaderDatabaseFileIO.WriteDatabaseToFile(m_dbFile);
    }

    public void AddIntersectShader(CustomShaderMeta shaderMeta) {
        var guid = Guid.NewGuid().ToString();
        m_intersectShaderList.Add(shaderMeta.name);
        m_intersectShaderMetaList.Add(shaderMeta);
        m_dbFile.intersect.Add(guid, shaderMeta);

        CustomShaderDatabaseFileIO.WriteDatabaseToFile(m_dbFile);
    }

    public bool ContainsThisShader(EShaderType shaderType, string shaderName) {

        switch (shaderType) {
            case EShaderType.CloestHit:
                return m_closetHitShaderMetaList.Any((item) => {
                    return item.name == shaderName;
                });

            case EShaderType.Intersect:
                return m_closetHitShaderMetaList.Any((item) => {
                    return item.name == shaderName;
                });

            default:
                Debug.LogWarning($"Unsupported shader type {shaderType}");
                return false;
        }

    }

    public string[] GetClosestShaderNameList {
        get {
            return m_closetHitShaderList.ToArray();
        }
    }

    public List<CustomShaderMeta> GetClosestShaderMetaList {
        get {
            return m_closetHitShaderMetaList.ToList();
        }
    }

    public string[] GetIntersectShaderNameList {
        get {
            return m_intersectShaderList.ToArray();
        }
    }

    public List<CustomShaderMeta> GetIntersectShaderMetaList {
        get {
            return m_intersectShaderMetaList.ToList();
        }
    }

    private void PopulateDataTableFromFile() {
        m_dbFile = CustomShaderDatabaseFileIO.ReadDatabaseFromFile();

        m_closetHitShaderList = new SortedSet<string>();
        m_closetHitShaderMetaList = new SortedSet<CustomShaderMeta>(comparer: new CustomShaderMetaComparer());

        m_intersectShaderList = new SortedSet<string>();
        m_intersectShaderMetaList = new SortedSet<CustomShaderMeta>(comparer: new CustomShaderMetaComparer());

        foreach (var kvp in m_dbFile.closetHit) {
            // Shader Name = kvp.Value.name
            // Shader GUID = kvp.Key
            m_closetHitShaderList.Add(kvp.Value.name);
            m_closetHitShaderMetaList.Add(kvp.Value);
        }

        foreach (var kvp in m_dbFile.intersect) {
            // Shader Name = kvp.Value.name
            // Shader GUID = kvp.Key
            m_intersectShaderList.Add(kvp.Value.name);
            m_intersectShaderMetaList.Add(kvp.Value);
        }
    }
}