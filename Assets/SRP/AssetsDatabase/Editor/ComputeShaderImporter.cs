using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace OpenRT {

    public class ComputeShaderImporter : AssetPostprocessor {

        public const string CUSTOMER_SHADER_COLLECTION_DIR = "Assets/SRP/ComputeShader/Shading/";
        public const string CUSTOMER_SHADER_COLLECTION_FILENAME = "CustomShaderCollection";
        public const string GPU_MAIN_PROCESS_FILENAME = "JoeShade";

        private static OpenRT.IShaderMetaReader closetHitReader = new OpenRT.ClosetHitShaderMetaReader();
        private static OpenRT.IShaderMetaReader intersectReader = new OpenRT.IntersectShaderMetaReader();

        private static OpenRT.ClosetHitShaderCollectionGPUProgramGenerator closetHitShaderCollectionGPUProgramGenerator = new OpenRT.ClosetHitShaderCollectionGPUProgramGenerator();
        private static OpenRT.IntersectShaderCollectionGPUProgramGenerator intersectShaderCollectionGPUProgramGenerator = new OpenRT.IntersectShaderCollectionGPUProgramGenerator();

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {

            foreach (string absPath in importedAssets) {
                if (shouldProcess(absPath)) {

                    var meta = ReadShaderMeta(absPath: absPath);

                    if (meta != null) {
                        CustomShaderDatabase.Instance.Add(meta.Value);
                    }

                }
            }

            foreach (string str in deletedAssets) {
                if (shouldProcess(str)) {
                    //TODO: [ComputeShaderImporter] Deleted Compute Shader
                    Debug.LogWarning("TODO: [ComputeShaderImporter] Deleted Compute Shader: " + str);
                }
            }

            for (int i = 0; i < movedAssets.Length; i++) {
                if (shouldProcess(movedAssets[i])) {
                    //TODO: [ComputeShaderImporter] Moved Compute Shader
                    Debug.LogWarning("[ComputeShaderImporter] Moved Compute Shader: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
                }
            }

            ExportAllShadersToGPUProgram();
        }

        private static bool shouldProcess(string path) {
            string ext = Path.GetExtension(path);

            if (ext == ".compute") {
                if (Path.GetFileNameWithoutExtension(path) == CUSTOMER_SHADER_COLLECTION_FILENAME) {
                    return false;
                }

                if (Path.GetFileNameWithoutExtension(path) == GPU_MAIN_PROCESS_FILENAME) {
                    return false;
                }
                return true;
            } else {
                return false;
            }
        }

        private static CustomShaderMeta? ReadShaderMeta(string absPath) {
            string fileContent = File.ReadAllText(absPath);

            string shaderName = null;
            if (closetHitReader.CanHandle(fileContent, out shaderName)) {
                return new CustomShaderMeta(name: shaderName, absPath: absPath, shaderType: OpenRT.EShaderType.CloestHit);
            } else if (intersectReader.CanHandle(fileContent, out shaderName)) {
                return new CustomShaderMeta(name: shaderName, absPath: absPath, shaderType: OpenRT.EShaderType.Intersect);
            } else {
                return null;
            }
        }

        private static void ExportAllShadersToGPUProgram() {
            closetHitShaderCollectionGPUProgramGenerator.ExportShaderCollection(CustomShaderDatabase.Instance.ShaderMetaList(EShaderType.CloestHit));
            intersectShaderCollectionGPUProgramGenerator.ExportShaderCollection(CustomShaderDatabase.Instance.ShaderMetaList(EShaderType.Intersect));
        }
    }
}