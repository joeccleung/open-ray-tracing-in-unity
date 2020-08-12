using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    using GUID = String;

    public class ClosestHitDataTable : BaseDataTable, ICustomShaderDatabaseDataTable {
        public override GUID AddShader(CustomShaderMeta shaderMeta,
            CustomShaderDatabaseFile database,
            IShaderDatabaseFileIO fileIOHandler) {
            string guid = base.AddShader(shaderMeta, database, fileIOHandler);
            database.closetHit.Add(guid, shaderMeta);
            fileIOHandler.WriteDatabaseToFile(database);

            return guid;
        }

        public override GUID MoveShader(CustomShaderMeta shaderMeta,
            CustomShaderMeta previousShaderMeta,
            CustomShaderDatabaseFile database,
            IShaderDatabaseFileIO fileIOHandler) {
            string guid = base.MoveShader(shaderMeta, previousShaderMeta, database, fileIOHandler);
            database.closetHit.Remove(guid);
            database.closetHit.Add(guid, shaderMeta);
            fileIOHandler.WriteDatabaseToFile(database);

            return guid;
        }

        public override GUID RemoveShader(CustomShaderMeta shaderMeta,
            CustomShaderDatabaseFile database,
            IShaderDatabaseFileIO fileIOHandler) {
            string guid = base.RemoveShader(shaderMeta, database, fileIOHandler);
            database.closetHit.Remove(guid);
            fileIOHandler.WriteDatabaseToFile(database);

            return guid;
        }
    }
}