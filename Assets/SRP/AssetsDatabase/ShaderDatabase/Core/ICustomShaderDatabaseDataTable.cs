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

        void Clean();

        bool Contains(string shaderName);

        int GUIDToShaderIndex(GUID guid);

        string GUIDToShaderName(GUID guid);

        bool IsDirty();

        GUID MoveShader(CustomShaderMeta shaderMeta, CustomShaderMeta previousShaderMeta, CustomShaderDatabaseFile database, IShaderDatabaseFileIO fileIOHandler);

        void Populate(Dictionary<string, CustomShaderMeta> data);

        GUID RemoveShader(CustomShaderMeta shaderMeta, CustomShaderDatabaseFile database, IShaderDatabaseFileIO fileIOHandler);

        SortedList<GUID, CustomShaderMeta> ShaderMetaList {
            get;
        }

        string[] ShaderNameList {
            get;
        }

        GUID ShaderNameToGUID(string shaderName);
    }

}