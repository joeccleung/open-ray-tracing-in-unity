using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public interface IShaderDatabaseFileIO {
        CustomShaderDatabaseFile ReadDatabaseFromFile();

        void WriteDatabaseToFile(CustomShaderDatabaseFile file);
    }
}