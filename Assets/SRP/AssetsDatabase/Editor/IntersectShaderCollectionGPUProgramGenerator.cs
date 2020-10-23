using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace OpenRT
{
    using GUID = System.String;

    public class IntersectShaderCollectionGPUProgramGenerator : IShaderCollectionGPUProgramGenerator
    {

        public const string COLLECTION_ABS_DIR = "Assets/SRP/ComputeShader/Intersect/";
        public const string COLLECTION_FILENAME = "IntersectShaderCollection";
        public const string COLLECTION_RELATIVE_TO_MAIN_PROCESS = "../Intersect/";
        public const string CUSTOM_SHADER_INDIVIDUAL_FILES_DIR = "Assets/SRP/ComputeShader/Custom/Geometry/";
        public const string CUSTOM_SHADER_INDIVIDUAL_FILES_RELATIVE_TO_MAIN_PROCESS = "../Custom/Geometry/";


        public bool ExportShaderCollection(SortedList<string, GUID> sortedByName, SortedList<GUID, CustomShaderMeta> shadersImportMetaList)
        {
            return WriteToCustomShaderCollection(GenerateShaderCollectionFileContent(sortedByName, shadersImportMetaList));
        }

        public string GenerateShaderCollectionFileContent(SortedList<string, GUID> sortedByName, SortedList<GUID, CustomShaderMeta> shadersImportMetaList)
        {

            Debug.Log("[Intersect] GenerateShaderCollectionFileContent");

            StringBuilder sb = new StringBuilder();
            // Order is reverse
            sb.AppendLine("// =============================================");
            sb.AppendLine("// =         Intersect Shader Collection       =");
            sb.AppendLine("// = Auto-generated File. Do not edit manually =");
            sb.AppendLine("// =============================================");
            sb.AppendLine();
            // sb.AppendLine("#pragma editor_sync_compilation");

            foreach (var kvp in shadersImportMetaList)
            {
                var relPath = kvp.Value.absPath.Replace(CUSTOM_SHADER_INDIVIDUAL_FILES_DIR, CUSTOM_SHADER_INDIVIDUAL_FILES_RELATIVE_TO_MAIN_PROCESS);
                sb.AppendLine($"#include \"{relPath}\"");
            }

            sb.AppendLine();
            sb.AppendLine("void IntersectByGeometry(Ray ray, inout RayHit bestHit, int primitiveId)");
            sb.AppendLine("{");
            sb.AppendLine("   Primitive pri = _Primitives[primitiveId];");
            sb.AppendLine();
            sb.AppendLine("   switch(pri.geometryIndex)");
            sb.AppendLine("   {");
            int secRaysIndex = 0;
            foreach (var shaderNameGUIDPair in sortedByName)
            {
                var guid = shaderNameGUIDPair.Value;

                sb.AppendLine($"        case {secRaysIndex}:");
                sb.AppendLine($"            {shadersImportMetaList[guid].name}Intersect(ray, bestHit, pri, primitiveId);");
                sb.AppendLine($"        break;");
                secRaysIndex++;
            }
            sb.AppendLine("   }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        public bool WriteToCustomShaderCollection(string content)
        {

            if (!Directory.Exists(COLLECTION_ABS_DIR))
            {
                Directory.CreateDirectory(COLLECTION_ABS_DIR);
            }

            try
            {
                System.IO.File.WriteAllText(
                    COLLECTION_ABS_DIR + COLLECTION_FILENAME + ".compute",
                    content);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }
    }
}