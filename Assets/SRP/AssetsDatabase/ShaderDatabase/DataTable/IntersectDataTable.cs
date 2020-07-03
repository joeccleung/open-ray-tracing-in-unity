using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    using GUID = System.String;

    public class IntersectDataTable : BaseDataTable, ICustomShaderDatabaseDataTable {
        public override GUID AddShader(CustomShaderMeta shaderMeta, CustomShaderDatabaseFile database, IShaderDatabaseFileIO fileIOHandler) {
            string guid = base.AddShader(shaderMeta, database, fileIOHandler);
            database.intersect.Add(guid, shaderMeta);
            fileIOHandler.WriteDatabaseToFile(database);

            return guid;
        }
    }

}