using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OpenRT
{
    public class RTFrameworkMenuItem : MonoBehaviour
    {
        private static ClosetHitShaderCollectionGPUProgramGenerator closetHitShaderCollectionGPUProgramGenerator = new ClosetHitShaderCollectionGPUProgramGenerator();
        private static IntersectShaderCollectionGPUProgramGenerator intersectShaderCollectionGPUProgramGenerator = new IntersectShaderCollectionGPUProgramGenerator();
        private static LightShaderCollectionGPUProgramGenerator lightShaderCollectionGPUProgramGenerator = new LightShaderCollectionGPUProgramGenerator();

        [MenuItem("OpenRT/Refresh Shader Collection")]
        static void RefreshShaderCollection()
        {
            // (1) - Reload the shader database
            // (2) - Rebuild the Closest Hit Shader Collection GPU Program
            // (3) - Rebuild the Intersect Hit Shader Collection GPU Program

            CustomShaderDatabase.Instance.LoadShaderDatabase();
            closetHitShaderCollectionGPUProgramGenerator.ExportShaderCollection(CustomShaderDatabase.Instance.ShaderSortedByName(EShaderType.ClosestHit),
                                                                                CustomShaderDatabase.Instance.ShaderMetaList(EShaderType.ClosestHit));

            intersectShaderCollectionGPUProgramGenerator.ExportShaderCollection(CustomShaderDatabase.Instance.ShaderSortedByName(EShaderType.Intersect),
                                                                                CustomShaderDatabase.Instance.ShaderMetaList(EShaderType.Intersect));

            lightShaderCollectionGPUProgramGenerator.ExportShaderCollection(CustomShaderDatabase.Instance.ShaderSortedByName(EShaderType.Light),
                                                                            CustomShaderDatabase.Instance.ShaderMetaList(EShaderType.Light));
        }
    }

}
