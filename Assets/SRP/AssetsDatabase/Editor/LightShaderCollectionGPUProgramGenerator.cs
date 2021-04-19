using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace OpenRT
{
    using GUID = System.String;

    public class LightShaderCollectionGPUProgramGenerator : IShaderCollectionGPUProgramGenerator
    {

        public const string COLLECTION_ABS_DIR = "Assets/SRP/ComputeShader/Light/";
        public const string COLLECTION_FILENAME = "CustomLights";
        public const string COLLECTION_RELATIVE_TO_MAIN_PROCESS = "../Light/";
        public const string CUSTOM_SHADER_INDIVIDUAL_FILES_DIR = "Assets/SRP/ComputeShader/Custom/Light/";
        public const string CUSTOM_SHADER_INDIVIDUAL_FILES_RELATIVE_TO_MAIN_PROCESS = "../Custom/Light/";


        public bool ExportShaderCollection(SortedList<string, GUID> sortedByName, SortedList<GUID, CustomShaderMeta> shadersImportMetaList)
        {
            return WriteToCustomShaderCollection(GenerateShaderCollectionFileContent(sortedByName, shadersImportMetaList));
        }

        public string GenerateShaderCollectionFileContent(SortedList<string, GUID> sortedByName, SortedList<GUID, CustomShaderMeta> shadersImportMetaList)
        {

            Debug.Log("[Intersect] GenerateShaderCollectionFileContent");

            StringBuilder sb = new StringBuilder();
            // Order is reverse
            sb.AppendLine("#ifndef CUSTOM_LIGHTS");
            sb.AppendLine("#define CUSTOM_LIGHTS");
            sb.AppendLine("// =============================================");
            sb.AppendLine("// =           Light Shader Collection         =");
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
            sb.AppendLine("LightHit GetIlluminate(float3 hitPos, int medium, int primitiveID, int lightID)");
            sb.AppendLine("{");
            sb.AppendLine("   switch(_Lights[lightID].type)");
            sb.AppendLine("   {");
            int secRaysIndex = 0;
            foreach (var shaderNameGUIDPair in sortedByName)
            {
                var guid = shaderNameGUIDPair.Value;

                sb.AppendLine($"        case {secRaysIndex}:");
                sb.AppendLine($"            return GetIlluminate_{shadersImportMetaList[guid].name}(_Lights[lightID], hitPos, medium, primitiveID);");
                secRaysIndex++;
            }
            sb.AppendLine($"       default:");
            sb.AppendLine($"            return CreateLightHit(float3(0, 0, 0), float3(0, 0, 0));");
            sb.AppendLine("   }");
            sb.AppendLine("}");

            sb.AppendLine("#endif // CUSTOM_LIGHTS ");

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