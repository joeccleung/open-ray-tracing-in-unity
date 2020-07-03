using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {

    using GUID = System.String;
    using ShaderName = System.String;

    /// <summary>
    /// Each Data Table contains a single shader type (e.g. Intersect, ClosestHit)
    /// </summary>
    public interface ICustomShaderDatabaseDataTable {
        GUID AddShader(CustomShaderMeta shaderMeta, CustomShaderDatabaseFile database, IShaderDatabaseFileIO fileIOHandler);

        bool Contains(string shaderName);

        int GUIDToShaderIndex(GUID guid);

        string GUIDToShaderName(GUID guid);

        void Populate(Dictionary<string, CustomShaderMeta> data);

        SortedList<GUID, CustomShaderMeta> ShaderMetaList {
            get;
        }

        string[] ShaderNameList {
            get;
        }

        GUID ShaderNameToGUID(string shaderName);
    }

}