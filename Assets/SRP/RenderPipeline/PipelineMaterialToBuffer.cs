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

            foreach (var materialColorList in sceneParseResult.m_materialsColorList)
            {
                ComputeBuffer cb = new ComputeBuffer(materialColorList.Value.Count, sizeof(float) * 4);
                cb.SetData(materialColorList.Value);
                mainShader.SetBuffer(0, materialColorList.Key, cb);
                computeBuffersForMaterialProperties.Add(cb);
                // Do NOT release the compute buffer before the actual draw commands is being sent
            }

            foreach (var materialIntList in sceneParseResult.m_materialsIntList)
            {
                ComputeBuffer cb = new ComputeBuffer(materialIntList.Value.Count, sizeof(int));
                cb.SetData(materialIntList.Value);
                mainShader.SetBuffer(0, materialIntList.Key, cb);
                computeBuffersForMaterialProperties.Add(cb);
                // Do NOT release the compute buffer before the actual draw commands is being sent
            }

            foreach (var materialFloatList in sceneParseResult.m_materialsFloatList)
            {
                ComputeBuffer cb = new ComputeBuffer(materialFloatList.Value.Count, sizeof(float));
                cb.SetData(materialFloatList.Value);
                mainShader.SetBuffer(0, materialFloatList.Key, cb);
                computeBuffersForMaterialProperties.Add(cb);
                // Do NOT release the compute buffer before the actual draw commands is being sent
            }

            foreach (var materialVector2List in sceneParseResult.m_materialsVector2List)
            {
                ComputeBuffer cb = new ComputeBuffer(materialVector2List.Value.Count, sizeof(float) * 2);
                cb.SetData(materialVector2List.Value);
                mainShader.SetBuffer(0, materialVector2List.Key, cb);
                computeBuffersForMaterialProperties.Add(cb);
                // Do NOT release the compute buffer before the actual draw commands is being sent
            }

            foreach (var materialVector3List in sceneParseResult.m_materialsVector3List)
            {
                ComputeBuffer cb = new ComputeBuffer(materialVector3List.Value.Count, sizeof(float) * 3);
                cb.SetData(materialVector3List.Value);
                mainShader.SetBuffer(0, materialVector3List.Key, cb);
                computeBuffersForMaterialProperties.Add(cb);
                // Do NOT release the compute buffer before the actual draw commands is being sent
            }

            foreach (var materialVector4List in sceneParseResult.m_materialsVector4List)
            {
                ComputeBuffer cb = new ComputeBuffer(materialVector4List.Value.Count, sizeof(float) * 4);
                cb.SetData(materialVector4List.Value);
                mainShader.SetBuffer(0, materialVector4List.Key, cb);
                computeBuffersForMaterialProperties.Add(cb);
                // Do NOT release the compute buffer before the actual draw commands is being sent
            }

            Texture2DArray texArr = new Texture2DArray(1024, 1024, sceneParseResult.m_textureCollection.Count, TextureFormat.RGBA32, 1, false);
            int texCounter = 0;
            foreach (var tex in sceneParseResult.m_textureCollection)
            {
                try
                {
                    Graphics.CopyTexture(tex, 0, texArr, texCounter);
                    texCounter++;
                }
                catch (Exception e)
                {
                    Debug.LogError($"[PipelineMaterialToBuffer] Exception when copying texture:{tex.name} Exp:{e}");
                    return;
                }

            }
            mainShader.SetTexture(mainShader.FindKernel("CSMain"), "_MatTexture", texArr);

            foreach (var materialTextureIndex in sceneParseResult.m_materialsTextureIndexList)
            {
                ComputeBuffer cb = new ComputeBuffer(materialTextureIndex.Value.Count, sizeof(int));
                cb.SetData(materialTextureIndex.Value);
                mainShader.SetBuffer(0, materialTextureIndex.Key, cb);
                computeBuffersForMaterialProperties.Add(cb);
                // Do NOT release the compute buffer before the actual draw commands is being sent
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