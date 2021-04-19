using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OpenRT
{
    using GUID = System.String;
    public class ClosetHitShaderCollectionGPUProgramGenerator : IShaderCollectionGPUProgramGenerator
    {

        public const string CUSTOMER_SHADER_COLLECTION_FILENAME = "CustomShaderCollection";

        public bool ExportShaderCollection(SortedList<string, GUID> sortedByName, SortedList<GUID, CustomShaderMeta> shadersImportMetaList)
        {
            return WriteToCustomShaderCollection(GenerateShaderCollectionFileContent(sortedByName, shadersImportMetaList));
        }

        public string GenerateShaderCollectionFileContent(SortedList<string, GUID> sortedByName, SortedList<GUID, CustomShaderMeta> shadersImportMetaList)
        {

            Debug.Log("[ClosestHit] GenerateShaderCollectionFileContent");

            StringBuilder sb = new StringBuilder();
            // Order is reverse
            sb.AppendLine("#ifndef CUSTOM_CLOSEST_HIT");
            sb.AppendLine("#define CUSTOM_CLOSEST_HIT");
            sb.AppendLine("// =============================================");
            sb.AppendLine("// =        Closet Hit Shader Collection       =");
            sb.AppendLine("// = Auto-generated File. Do not edit manually =");
            sb.AppendLine("// =============================================");
            sb.AppendLine();
            // sb.AppendLine("#pragma editor_sync_compilation");

            foreach (var kvp in shadersImportMetaList)
            {
                var relPath = GPUMainProgramPathProvider.RelativeToGPUMain(kvp.Value.absPath);
                sb.AppendLine($"#include \"{relPath}\"");
            }

            sb.AppendLine("void SecRays(Ray ray, RayHit hit, inout SecRaysAtHit secRays)");
            sb.AppendLine("{");
            // TODO: Determine which kind of switch attribute works
            // https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-switch
            sb.AppendLine("     switch(_Primitives[hit.primitiveId].materialIndex)");
            sb.AppendLine("     {");
            int secRaysIndex = 0;
            foreach (var shaderNameGUIDPair in sortedByName)
            {

                var guid = shaderNameGUIDPair.Value;

                sb.AppendLine($"        case {secRaysIndex}:");
                sb.AppendLine($"            {shadersImportMetaList[guid].name}_SecRays(ray, hit, secRays);");
                sb.AppendLine($"        break;");
                secRaysIndex++;
            }
            sb.AppendLine("     }");
            sb.AppendLine("}");

            sb.AppendLine("float3 ClosestHit(inout Ray ray, RayHit hit, float3 ambientLightUpper, float3 secondaryRayColor)");
            sb.AppendLine("{");
            // TODO: Determine which kind of switch attribute works
            // https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-switch
            sb.AppendLine("switch(_Primitives[hit.primitiveId].materialIndex)");
            sb.AppendLine("{");

            int index = 0;
            foreach (var shaderNameGUIDPair in sortedByName)
            {
                var guid = shaderNameGUIDPair.Value;

                sb.AppendLine($"case {index}:");
                sb.AppendLine($"   return {shadersImportMetaList[guid].name}(ray, hit, ambientLightUpper, secondaryRayColor);");
                index++;
            }
            sb.AppendLine($"default:");
            sb.AppendLine($"  return float3(0, 1, 1);");

            sb.AppendLine("}");
            sb.AppendLine("}");

            sb.AppendLine("float3 OnShadowRayHit(Ray ray, RayHit hit)");
            sb.AppendLine("{");
            // TODO: Determine which kind of switch attribute works
            // https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-switch
            sb.AppendLine("switch(_Primitives[hit.primitiveId].materialIndex)");
            sb.AppendLine("{");

            int onShadowRayHitIndex = 0;
            foreach (var shaderNameGUIDPair in sortedByName)
            {
                var guid = shaderNameGUIDPair.Value;

                sb.AppendLine($"        case {onShadowRayHitIndex}:");
                sb.AppendLine($"           return {shadersImportMetaList[guid].name}_OnShadowRayHit(ray, hit);");
                onShadowRayHitIndex++;
            }
            sb.AppendLine($"        default:");
            sb.AppendLine($"          return float3(1, 0, 1);");

            sb.AppendLine("}");
            sb.AppendLine("}");

            sb.AppendLine("#endif  // CUSTOM_CLOSEST_HIT");

            return sb.ToString();
        }

        public bool WriteToCustomShaderCollection(string content)
        {
            try
            {
                System.IO.File.WriteAllText(
                    GPUMainProgramPathProvider.CUSTOMER_SHADER_COLLECTION_DIR + CUSTOMER_SHADER_COLLECTION_FILENAME + ".compute",
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