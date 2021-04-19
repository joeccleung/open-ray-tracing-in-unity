using System.IO;
using UnityEditor;
using UnityEngine;

namespace OpenRT
{

    public class ComputeShaderImporter : AssetPostprocessor
    {
        private static IShaderMetaReader closetHitReader = new ClosetHitShaderMetaReader();
        private static IShaderMetaReader intersectReader = new IntersectShaderMetaReader();
        private static IShaderMetaReader lightShaderReader = new LightShaderMetaReader();

        private static ClosetHitShaderCollectionGPUProgramGenerator closetHitShaderCollectionGPUProgramGenerator = new ClosetHitShaderCollectionGPUProgramGenerator();
        private static IntersectShaderCollectionGPUProgramGenerator intersectShaderCollectionGPUProgramGenerator = new IntersectShaderCollectionGPUProgramGenerator();
        private static LightShaderCollectionGPUProgramGenerator lightShaderCollectionGPUProgramGenerator = new LightShaderCollectionGPUProgramGenerator();

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
            if (ComputeShaderCollectionFileNames.ShouldProcess(absPath))
            {
                var meta = ReadShaderMeta(absPath: absPath);
                Debug.Log($"[ComputeShaderImporter] Delete assert meta = {meta.Value.absPath}");
                CustomShaderDatabase.Instance.Remove(meta.Value);
                Debug.LogWarning("[ComputeShaderImporter] Deleted Compute Shader: " + absPath);
            }
        }

        private static void HandleImportAssets(string absPath)
        {
            if (ComputeShaderCollectionFileNames.ShouldProcess(absPath))
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
            if (ComputeShaderCollectionFileNames.ShouldProcess(movedAssets[i]))
            {
                var meta = ReadShaderMeta(absPath: movedAssets[i]);
                var previous = ReadShaderMeta(absPath: movedFromAssetPaths[i]);
                CustomShaderDatabase.Instance.Move(meta.Value, previous.Value);
                Debug.LogWarning("[ComputeShaderImporter] Moved Compute Shader: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
            }
        }

        private static CustomShaderMeta? ReadShaderMeta(string absPath)
        {
            string fileContent = File.ReadAllText(absPath);

            string shaderName = null;
            if (closetHitReader.CanHandle(fileContent, out shaderName))
            {
                return new CustomShaderMeta(name: shaderName, absPath: absPath, shaderType: OpenRT.EShaderType.ClosestHit);
            }
            else if (intersectReader.CanHandle(fileContent, out shaderName))
            {
                return new CustomShaderMeta(name: shaderName, absPath: absPath, shaderType: OpenRT.EShaderType.Intersect);
            }
            else if (lightShaderReader.CanHandle(fileContent, out shaderName))
            {
                return new CustomShaderMeta(name: shaderName, absPath: absPath, shaderType: OpenRT.EShaderType.Light);
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
                intersectShaderCollectionGPUProgramGenerator.ExportShaderCollection(db.ShaderSortedByName(EShaderType.Intersect),
                                                                                    db.ShaderMetaList(EShaderType.Intersect));
                db.SetShaderTableClean(EShaderType.Intersect);
            }
            if (db.IsShaderTableDirty(EShaderType.Light))
            {
                lightShaderCollectionGPUProgramGenerator.ExportShaderCollection(db.ShaderSortedByName(EShaderType.Light),
                                                                                db.ShaderMetaList(EShaderType.Light));
                db.SetShaderTableClean(EShaderType.Light);
            }
        }
    }
}