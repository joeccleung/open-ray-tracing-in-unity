using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OpenRT {
    using GUID = System.String;
    public class ClosetHitShaderCollectionGPUProgramGenerator : IShaderCollectionGPUProgramGenerator {

        public const string CUSTOMER_SHADER_COLLECTION_FILENAME = "CustomShaderCollection";

        public bool ExportShaderCollection(SortedList<GUID, CustomShaderMeta> shadersImportMetaList) {
            return WriteToCustomShaderCollection(GenerateShaderCollectionFileContent(shadersImportMetaList));
        }

        public string GenerateShaderCollectionFileContent(SortedList<GUID, CustomShaderMeta>  shadersImportMetaList) {
            StringBuilder sb = new StringBuilder();
            // Order is reverse
            sb.AppendLine("// =============================================");
            sb.AppendLine("// =        Closet Hit Shader Collection       =");
            sb.AppendLine("// = Auto-generated File. Do not edit manually =");
            sb.AppendLine($"// = Time: {System.DateTime.Now.ToLongDateString()} {System.DateTime.Now.ToLongTimeString()} =");
            sb.AppendLine("// =============================================");
            sb.AppendLine();
            // sb.AppendLine("#pragma editor_sync_compilation");

            foreach (var kvp in shadersImportMetaList) {
                var relPath = GPUMainProgramPathProvider.RelativeToGPUMain(kvp.Value.absPath);
                sb.AppendLine($"#include \"{relPath}\"");
            }

            sb.AppendLine("float3 Shade(inout Ray ray, RayHit hit, float3 ambientLightUpper)");
            sb.AppendLine("{");
            // TODO: Determine which kind of switch attribute works
            // https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-switch
            sb.AppendLine("switch(_Primitives[hit.primitiveId].materialIndex)");
            sb.AppendLine("{");

            int index = 0;
            foreach (var kvp in shadersImportMetaList) {
                sb.AppendLine($"case {index}:");
                sb.AppendLine($"   return {kvp.Value.name}(ray, hit, ambientLightUpper);");
                index++;
            }
            sb.AppendLine($"default:");
            sb.AppendLine($"  return float3(0, 1, 1);");

            sb.AppendLine("}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        public bool WriteToCustomShaderCollection(string content) {
            try {
                System.IO.File.WriteAllText(
                    GPUMainProgramPathProvider.CUSTOMER_SHADER_COLLECTION_DIR + CUSTOMER_SHADER_COLLECTION_FILENAME + ".compute",
                    content);
                return true;
            } catch (System.Exception ex) {
                Debug.LogException(ex);
                return false;
            }
        }
    }
}