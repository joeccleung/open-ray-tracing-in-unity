using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OpenRT {
    public class ClosetHitShaderCollectionGPUProgramGenerator : IShaderCollectionGPUProgramGenerator {

        public const string CUSTOMER_SHADER_COLLECTION_FILENAME = "CustomShaderCollection";

        public bool ExportShaderCollection(List<CustomShaderMeta> shadersImportMetaList) {
            return WriteToCustomShaderCollection(GenerateShaderCollectionFileContent(shadersImportMetaList));
        }

        public string GenerateShaderCollectionFileContent(List<CustomShaderMeta> shadersImportMetaList) {
            StringBuilder sb = new StringBuilder();
            // Order is reverse
            sb.AppendLine("// =============================================");
            sb.AppendLine("// =        Closet Hit Shader Collection       =");
            sb.AppendLine("// = Auto-generated File. Do not edit manually =");
            sb.AppendLine($"// = Time: {System.DateTime.Now.ToLongDateString()} {System.DateTime.Now.ToLongTimeString()} =");
            sb.AppendLine("// =============================================");
            sb.AppendLine();
            // sb.AppendLine("#pragma editor_sync_compilation");

            shadersImportMetaList.ForEach((shader) => {
                var relPath = GPUMainProgramPathProvider.RelativeToGPUMain(shader.absPath);
                sb.AppendLine($"#include \"{relPath}\"");
            });

            sb.AppendLine("float3 Shade(inout Ray ray, RayHit hit, float3 ambientLightUpper)");
            sb.AppendLine("{");
            // TODO: Determine which kind of switch attribute works
            // https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-switch
            sb.AppendLine("switch(hit.matIndex)");
            sb.AppendLine("{");

            for (int s = 0; s < shadersImportMetaList.Count; s++) {
                sb.AppendLine($"case {s}:");
                sb.AppendLine($"   return {shadersImportMetaList[s].name}(ray, hit, ambientLightUpper);");
            }
            sb.AppendLine($"default:");
            sb.AppendLine($"  return float3(0, 0, 0);");

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