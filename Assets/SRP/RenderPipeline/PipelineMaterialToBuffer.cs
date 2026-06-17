using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace OpenRT
{
    static class PipelineMaterialToBuffer
    {
        public static void MaterialsToBuffer(List<ComputeBuffer> computeBuffersForMaterialProperties,
                                             in SceneParseResult sceneParseResult,
                                             ref ComputeShader mainShader)
        {
            foreach (var mat in sceneParseResult.Materials)
            {
                MaterialToBuffer(material: mat,
                                 mainShader: ref mainShader,
                                 sceneParseResult: sceneParseResult);
            }

            // If no materials in scene, ensure shader properties are still set with dummy buffers
            // to avoid "Property not set" errors. The shader declares these buffers based on
            // material types available in the project (DemoMat, TextureUnlitMat, etc.)
            bool hasMaterials = sceneParseResult.Materials.Count > 0;

            foreach (var materialColorList in sceneParseResult.m_materialsColorList)
            {
                int count = materialColorList.Value.Count;
                if (count > 0)
                {
                    ComputeBuffer cb = new ComputeBuffer(count, sizeof(float) * 4);
                    cb.SetData(materialColorList.Value);
                    mainShader.SetBuffer(0, materialColorList.Key, cb);
                    computeBuffersForMaterialProperties.Add(cb);
                    // Do NOT release the compute buffer before the actual draw commands is being sent
                }
            }

            // If no materials, set up dummy buffers for expected material properties
            // These are the properties from materials used in shaders (DemoMat, TextureUnlitMat, TranslucentMat, MarsMat, etc.)
            if (!hasMaterials)
            {
                // DemoMat properties (used in Phong.compute)
                if (!sceneParseResult.m_materialsColorList.ContainsKey("DemoMat_color"))
                {
                    ComputeBuffer cb = new ComputeBuffer(1, sizeof(float) * 4);
                    mainShader.SetBuffer(0, "DemoMat_color", cb);
                    computeBuffersForMaterialProperties.Add(cb);
                }
                // TranslucentMat properties (used in Translucent.compute)
                if (!sceneParseResult.m_materialsColorList.ContainsKey("TranslucentMat_color"))
                {
                    ComputeBuffer cb = new ComputeBuffer(1, sizeof(float) * 4);
                    mainShader.SetBuffer(0, "TranslucentMat_color", cb);
                    computeBuffersForMaterialProperties.Add(cb);
                }
                // MarsMat properties
                if (!sceneParseResult.m_materialsColorList.ContainsKey("MarsMat_color"))
                {
                    ComputeBuffer cb = new ComputeBuffer(1, sizeof(float) * 4);
                    mainShader.SetBuffer(0, "MarsMat_color", cb);
                    computeBuffersForMaterialProperties.Add(cb);
                }
                // TextureUnlitMat properties
                if (!sceneParseResult.m_materialsColorList.ContainsKey("TextureUnlitMat_color"))
                {
                    ComputeBuffer cb = new ComputeBuffer(1, sizeof(float) * 4);
                    mainShader.SetBuffer(0, "TextureUnlitMat_color", cb);
                    computeBuffersForMaterialProperties.Add(cb);
                }
            }

            foreach (var materialIntList in sceneParseResult.m_materialsIntList)
            {
                int count = materialIntList.Value.Count;
                if (count > 0)
                {
                    ComputeBuffer cb = new ComputeBuffer(count, sizeof(int));
                    cb.SetData(materialIntList.Value);
                    mainShader.SetBuffer(0, materialIntList.Key, cb);
                    computeBuffersForMaterialProperties.Add(cb);
                    // Do NOT release the compute buffer before the actual draw commands is being sent
                }
            }

            // If no materials, set up dummy buffers for expected material int properties
            if (!hasMaterials)
            {
                // DemoMat_main (used in Phong.compute)
                if (!sceneParseResult.m_materialsIntList.ContainsKey("DemoMat_main"))
                {
                    ComputeBuffer cb = new ComputeBuffer(1, sizeof(int));
                    mainShader.SetBuffer(0, "DemoMat_main", cb);
                    computeBuffersForMaterialProperties.Add(cb);
                }
                // TextureUnlitMat_main (used in Phong.compute)
                if (!sceneParseResult.m_materialsIntList.ContainsKey("TextureUnlitMat_main"))
                {
                    ComputeBuffer cb = new ComputeBuffer(1, sizeof(int));
                    mainShader.SetBuffer(0, "TextureUnlitMat_main", cb);
                    computeBuffersForMaterialProperties.Add(cb);
                }
                // MarsMat_main
                if (!sceneParseResult.m_materialsIntList.ContainsKey("MarsMat_main"))
                {
                    ComputeBuffer cb = new ComputeBuffer(1, sizeof(int));
                    mainShader.SetBuffer(0, "MarsMat_main", cb);
                    computeBuffersForMaterialProperties.Add(cb);
                }
            }

            foreach (var materialFloatList in sceneParseResult.m_materialsFloatList)
            {
                int count = materialFloatList.Value.Count;
                if (count > 0)
                {
                    ComputeBuffer cb = new ComputeBuffer(count, sizeof(float));
                    cb.SetData(materialFloatList.Value);
                    mainShader.SetBuffer(0, materialFloatList.Key, cb);
                    computeBuffersForMaterialProperties.Add(cb);
                    // Do NOT release the compute buffer before the actual draw commands is being sent
                }
            }

            // If no materials, set up dummy buffers for expected material float properties
            if (!hasMaterials)
            {
                // RefractiveMat properties
                if (!sceneParseResult.m_materialsFloatList.ContainsKey("RefractiveMat_refractiveIndex"))
                {
                    ComputeBuffer cb = new ComputeBuffer(1, sizeof(float));
                    mainShader.SetBuffer(0, "RefractiveMat_refractiveIndex", cb);
                    computeBuffersForMaterialProperties.Add(cb);
                }
                // SinMat properties
                if (!sceneParseResult.m_materialsFloatList.ContainsKey("SinMat_sin"))
                {
                    ComputeBuffer cb = new ComputeBuffer(1, sizeof(float));
                    mainShader.SetBuffer(0, "SinMat_sin", cb);
                    computeBuffersForMaterialProperties.Add(cb);
                }
                // TranslucentMat properties
                if (!sceneParseResult.m_materialsFloatList.ContainsKey("TranslucentMat_reflectivity"))
                {
                    ComputeBuffer cb = new ComputeBuffer(1, sizeof(float));
                    mainShader.SetBuffer(0, "TranslucentMat_reflectivity", cb);
                    computeBuffersForMaterialProperties.Add(cb);
                }
                if (!sceneParseResult.m_materialsFloatList.ContainsKey("TranslucentMat_secondaryRayEffect"))
                {
                    ComputeBuffer cb = new ComputeBuffer(1, sizeof(float));
                    mainShader.SetBuffer(0, "TranslucentMat_secondaryRayEffect", cb);
                    computeBuffersForMaterialProperties.Add(cb);
                }
                if (!sceneParseResult.m_materialsFloatList.ContainsKey("TranslucentMat_transparency"))
                {
                    ComputeBuffer cb = new ComputeBuffer(1, sizeof(float));
                    mainShader.SetBuffer(0, "TranslucentMat_transparency", cb);
                    computeBuffersForMaterialProperties.Add(cb);
                }
                // VolumetricMat properties
                if (!sceneParseResult.m_materialsFloatList.ContainsKey("VolumetricMat_luminanceLight"))
                {
                    ComputeBuffer cb = new ComputeBuffer(1, sizeof(float));
                    mainShader.SetBuffer(0, "VolumetricMat_luminanceLight", cb);
                    computeBuffersForMaterialProperties.Add(cb);
                }
                if (!sceneParseResult.m_materialsFloatList.ContainsKey("VolumetricMat_sigma"))
                {
                    ComputeBuffer cb = new ComputeBuffer(1, sizeof(float));
                    mainShader.SetBuffer(0, "VolumetricMat_sigma", cb);
                    computeBuffersForMaterialProperties.Add(cb);
                }
                if (!sceneParseResult.m_materialsFloatList.ContainsKey("VolumetricMat_stepSize"))
                {
                    ComputeBuffer cb = new ComputeBuffer(1, sizeof(float));
                    mainShader.SetBuffer(0, "VolumetricMat_stepSize", cb);
                    computeBuffersForMaterialProperties.Add(cb);
                }
            }

            foreach (var materialVector2List in sceneParseResult.m_materialsVector2List)
            {
                int count = materialVector2List.Value.Count;
                if (count > 0)
                {
                    ComputeBuffer cb = new ComputeBuffer(count, sizeof(float) * 2);
                    cb.SetData(materialVector2List.Value);
                    mainShader.SetBuffer(0, materialVector2List.Key, cb);
                    computeBuffersForMaterialProperties.Add(cb);
                    // Do NOT release the compute buffer before the actual draw commands is being sent
                }
            }

            foreach (var materialVector3List in sceneParseResult.m_materialsVector3List)
            {
                int count = materialVector3List.Value.Count;
                if (count > 0)
                {
                    ComputeBuffer cb = new ComputeBuffer(count, sizeof(float) * 3);
                    cb.SetData(materialVector3List.Value);
                    mainShader.SetBuffer(0, materialVector3List.Key, cb);
                    computeBuffersForMaterialProperties.Add(cb);
                    // Do NOT release the compute buffer before the actual draw commands is being sent
                }
            }

            foreach (var materialVector4List in sceneParseResult.m_materialsVector4List)
            {
                int count = materialVector4List.Value.Count;
                if (count > 0)
                {
                    ComputeBuffer cb = new ComputeBuffer(count, sizeof(float) * 4);
                    cb.SetData(materialVector4List.Value);
                    mainShader.SetBuffer(0, materialVector4List.Key, cb);
                    computeBuffersForMaterialProperties.Add(cb);
                    // Do NOT release the compute buffer before the actual draw commands is being sent
                }
            }

            // Ensure Texture2DArray has at least depth 1 to avoid out of range exception
            int textureCount = Mathf.Max(1, sceneParseResult.m_textureCollection.Count);
            Texture2DArray texArr = new Texture2DArray(1024, 1024, textureCount, TextureFormat.RGBA32, 1, false);
            int texCounter = 0;
            foreach (var tex in sceneParseResult.m_textureCollection)
            {
                try
                {
                    // Check if source texture matches destination format and size
                    // Graphics.CopyTexture requires matching formats and dimensions for full mipmap copies
                    bool needsConversion = tex.width != 1024 || tex.height != 1024 || 
                                          tex.format != TextureFormat.RGBA32;
                    
                    if (needsConversion)
                    {
                        // Log warning to help developers identify textures that need re-importing
                        // with correct settings (1024x1024, RGBA32) for optimal performance
#if UNITY_EDITOR
                        string assetPath = UnityEditor.AssetDatabase.GetAssetPath(tex);
                        string pathInfo = string.IsNullOrEmpty(assetPath) ? "(built-in or runtime texture)" : $"at path: {assetPath}";
#else
                        string pathInfo = "(asset path unavailable in build)";
#endif
                        Debug.LogWarning($"[PipelineMaterialToBuffer] Texture '{tex.name}' {pathInfo} does not match required format. " +
                                       $"Current: {tex.width}x{tex.height}, {tex.format}. " +
                                       $"Required: 1024x1024, RGBA32. " +
                                       $"Texture will be converted at runtime (impacts performance). " +
                                       $"Consider re-importing with correct settings.", tex);
                        
                        // Create a temporary RenderTexture with the target format and size
                        // Blit handles format conversion and resizing
                        RenderTexture tempRT = RenderTexture.GetTemporary(1024, 1024, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
                        tempRT.filterMode = FilterMode.Bilinear;
                        
                        // Blit source texture to temporary RT (handles format conversion and scaling)
                        Graphics.Blit(tex, tempRT);
                        
                        // Copy from temporary RT to the texture array slice
                        Graphics.CopyTexture(tempRT, 0, 0, texArr, texCounter, 0);
                        
                        // Release temporary RT
                        RenderTexture.ReleaseTemporary(tempRT);
                    }
                    else
                    {
                        // Direct copy if format and size match
                        Graphics.CopyTexture(tex, 0, texArr, texCounter);
                    }
                    texCounter++;
                }
                catch (Exception e)
                {
                    Debug.LogError($"[PipelineMaterialToBuffer] Exception when copying texture '{tex.name}'. " +
                                 $"Texture will be skipped. Error: {e}", tex);
                    // Do NOT return early - continue to set up remaining buffers
                    // The texture array slice will remain empty (black), but shader properties must be set
                    texCounter++;
                }

            }
            mainShader.SetTexture(mainShader.FindKernel("CSMain"), "_MatTexture", texArr);

            foreach (var materialTextureIndex in sceneParseResult.m_materialsTextureIndexList)
            {
                int count = materialTextureIndex.Value.Count;
                if (count > 0)
                {
                    ComputeBuffer cb = new ComputeBuffer(count, sizeof(int));
                    cb.SetData(materialTextureIndex.Value);
                    mainShader.SetBuffer(0, materialTextureIndex.Key, cb);
                    computeBuffersForMaterialProperties.Add(cb);
                    // Do NOT release the compute buffer before the actual draw commands is being sent
                }
            }
        }

        public static void MaterialToBuffer(in RTMaterial material,
                                            ref ComputeShader mainShader,
                                            SceneParseResult sceneParseResult)
        {
            FieldInfo[] fieldsInMat = GetFieldsInMaterial(material); // TODO: Later we may want to decide to include both public fields and private fields

            foreach (var field in fieldsInMat)
            {
                string fieldName = GetFieldName(matName: GetMatName(material), field);

                var fieldValue = GetFieldValue(material, field);

                ProcessField(ref mainShader, fieldName, fieldValue, sceneParseResult);
            }
        }

        private static FieldInfo[] GetFieldsInMaterial(RTMaterial material)
        {
            return material.GetType().GetFields();
        }

        private static string GetFieldName(string matName, FieldInfo field)
        {
            return $"{matName}_{field.Name}";
        }

        private static object GetFieldValue(RTMaterial material, FieldInfo field)
        {
            return field.GetValue(material);
        }

        private static string GetMatName(RTMaterial material)
        {
            return material.GetType().Name;
        }

        private static void AssignFieldToMainShader(string fieldName,
                                                    in object fieldValue,
                                                    SceneParseResult sceneParseResult)
        {
            switch (fieldValue)
            {
                case Color c:
                    sceneParseResult.AddMaterialColor(name: fieldName, color: c);
                    break;

                case int i:
                    sceneParseResult.AddMaterialInt(name: fieldName, value: i);
                    break;

                case float f:
                    sceneParseResult.AddMaterialFloat(name: fieldName, value: f);
                    break;

                case Vector2 v2:
                    sceneParseResult.AddMaterialVector2(name: fieldName, value: v2);
                    break;

                case Vector3 v3:
                    sceneParseResult.AddMaterialVector3(name: fieldName, value: v3);
                    break;

                case Vector4 v4:
                    sceneParseResult.AddMaterialVector4(name: fieldName, value: v4);
                    break;

                case Texture2D texture2D:
                    sceneParseResult.AddMaterialTexture(name: fieldName, texture: texture2D);
                    break;
            }
        }

        private static void ProcessField(ref ComputeShader mainShader,
                                         string fieldName,
                                         object fieldValue,
                                         SceneParseResult sceneParseResult)
        {
            if (fieldValue == null)
            {
                return;
            }

            AssignFieldToMainShader(fieldName, fieldValue, sceneParseResult);
        }
    }
}