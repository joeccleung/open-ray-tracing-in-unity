using System.Collections.Generic;

namespace OpenRT {
    public interface IShaderCollectionGPUProgramGenerator {

        /// <summary>
        /// Short-hand for GenerateShaderCollectionFileContent then WriteToCustomShaderCollection
        /// </summary>
        bool ExportShaderCollection(List<CustomShaderMeta> shadersImportMetaList);

        string GenerateShaderCollectionFileContent(List<CustomShaderMeta> shadersImportMetaList);

        bool WriteToCustomShaderCollection(string content);
    }
}