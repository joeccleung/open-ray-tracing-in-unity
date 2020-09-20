using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OpenRT
{
    public class RTFrameworkMenuItem : MonoBehaviour
    {
        private static OpenRT.ClosetHitShaderCollectionGPUProgramGenerator closetHitShaderCollectionGPUProgramGenerator = new OpenRT.ClosetHitShaderCollectionGPUProgramGenerator();

        [MenuItem("OpenRT/Refresh Shader Collection")]
        static void RefreshShaderCollection()
        {
            closetHitShaderCollectionGPUProgramGenerator.ExportShaderCollection(CustomShaderDatabase.Instance.ShaderSortedByName(EShaderType.ClosestHit),
                                                                                CustomShaderDatabase.Instance.ShaderMetaList(EShaderType.ClosestHit));
        }
    }

}
