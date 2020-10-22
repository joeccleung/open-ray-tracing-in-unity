using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace OpenRT
{

    public class ComputeShaderImporter : AssetPostprocessor
    {

        public const string CUSTOMER_SHADER_COLLECTION_DIR = "Assets/SRP/ComputeShader/Shading/";
        public const string CUSTOMER_SHADER_COLLECTION_FILENAME = "CustomShaderCollection";
        public const string GPU_MAIN_PROCESS_FILENAME = "JoeShade";

        private static IShaderMetaReader closetHitReader = new ClosetHitShaderMetaReader();
        private static IShaderMetaReader intersectReader = new IntersectShaderMetaReader();

        private static ClosetHitShaderCollectionGPUProgramGenerator closetHitShaderCollectionGPUProgramGenerator = new ClosetHitShaderCollectionGPUProgramGenerator();
        private static IntersectShaderCollectionGPUProgramGenerator intersectShaderCollectionGPUProgramGenerator = new IntersectShaderCollectionGPUProgramGenerator();

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {

            foreach (string absPath in importedAssets)
            {
                HandleImportAssets(absPath);
            }

            foreach (string absPath in deletedAssets)
            {
                HandleDeleteAssets(absPath);
            }

            for (int i = 0; i < movedAssets.Length; i++)
            {
                HandleMoveAssets(movedAssets, movedFromAssetPaths, i);
            }

            ExportChangedShadersToGPUProgram(CustomShaderDatabase.Instance);
        }


        private static void HandleDeleteAssets(string absPath)
        {
            if (shouldProcess(absPath))
            {
                var meta = ReadShaderMeta(absPath: absPath);
                Debug.Log($"[ComputeShaderImporter] Delete assert meta = {meta.Value.absPath}");
                CustomShaderDatabase.Instance.Remove(meta.Value);
                Debug.LogWarning("[ComputeShaderImporter] Deleted Compute Shader: " + absPath);
            }
        }

        private static void HandleImportAssets(string absPath)
        {
            if (shouldProcess(absPath))
            {

                var meta = ReadShaderMeta(absPath: absPath);

                if (meta.HasValue)
                {
                    Debug.Log($"[ComputeShaderImporter] Add assert meta = {meta.Value.absPath}");

                    CustomShaderDatabase.Instance.Add(meta.Value);
                }

            }
        }

        private static void HandleMoveAssets(string[] movedAssets, string[] movedFromAssetPaths, int i)
        {
            if (shouldProcess(movedAssets[i]))
            {
                var meta = ReadShaderMeta(absPath: movedAssets[i]);
                var previous = ReadShaderMeta(absPath: movedFromAssetPaths[i]);
                CustomShaderDatabase.Instance.Move(meta.Value, previous.Value);
                Debug.LogWarning("[ComputeShaderImporter] Moved Compute Shader: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
            }
        }

        private static bool shouldProcess(string path)
        {
            string ext = Path.GetExtension(path);

            if (ext == ".compute")
            {
                if (Path.GetFileNameWithoutExtension(path) == CUSTOMER_SHADER_COLLECTION_FILENAME)
                {
                    return false;
                }

                if (Path.GetFileNameWithoutExtension(path) == GPU_MAIN_PROCESS_FILENAME)
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private static CustomShaderMeta? ReadShaderMeta(string absPath)
        {
            string fileContent = File.ReadAllText(absPath);

            string shaderName = null;
            if (closetHitReader.CanHandle(fileContent, out shaderName))
            {
                return new CustomShaderMeta(name: shaderName, absPath: absPath, shaderType: OpenRT.EShaderType.ClosestHit);
                // } else if (intersectReader.CanHandle(fileContent, out shaderName)) {
                //     return new CustomShaderMeta(name: shaderName, absPath: absPath, shaderType: OpenRT.EShaderType.Intersect);
            }
            else
            {
                return null;
            }
        }

        private static void ExportChangedShadersToGPUProgram(CustomShaderDatabase db)
        {
            Debug.Log("[ShaderImporter] ExportChangedShadersToGPUProgram");
            if (db.IsShaderTableDirty(EShaderType.ClosestHit))
            {
                closetHitShaderCollectionGPUProgramGenerator.ExportShaderCollection(db.ShaderSortedByName(EShaderType.ClosestHit),
                                                                                    db.ShaderMetaList(EShaderType.ClosestHit));
                db.SetShaderTableClean(EShaderType.ClosestHit);
            }
            if (db.IsShaderTableDirty(EShaderType.Intersect))
            {
                // TODO: Fix intersection shader collection
                // intersectShaderCollectionGPUProgramGenerator.ExportShaderCollection(CustomShaderDatabase.Instance.ShaderMetaList(EShaderType.Intersect));
                db.SetShaderTableClean(EShaderType.Intersect);
            }
        }
    }
}