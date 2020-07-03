using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    using GUID = String;

    public class ClosestHitDataTable : BaseDataTable, ICustomShaderDatabaseDataTable {
        public override GUID AddShader(CustomShaderMeta shaderMeta, CustomShaderDatabaseFile database, IShaderDatabaseFileIO fileIOHandler) {
            string guid = base.AddShader(shaderMeta, database, fileIOHandler);
            database.closetHit.Add(guid, shaderMeta);
            fileIOHandler.WriteDatabaseToFile(database);

            return guid;
        }
    }
}