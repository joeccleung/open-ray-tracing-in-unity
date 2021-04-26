using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace OpenRT
{
    public static class PipelineLightslToBuffer
    {
        public static void LightsToBuffer(List<ComputeBuffer> computeBuffersForLightProperties,
                                          in SceneParseResult sceneParseResult,
                                          ref ComputeShader mainShader)
        {
            foreach (var light in sceneParseResult.Lights)
            {
                LightToBuffer(
                    light: light,
                    mainShader: ref mainShader,
                    sceneParseResult: sceneParseResult);
            }

            foreach (var colors in sceneParseResult.m_lightColorList)
            {
                ComputeBuffer cb = new ComputeBuffer(colors.Value.Count, sizeof(float) * 4);
                cb.SetData(colors.Value);
                mainShader.SetBuffer(0, colors.Key, cb);
                computeBuffersForLightProperties.Add(cb);
                // Do NOT release the compute buffer before the actual draw commands is being sent
            }

            foreach (var ints in sceneParseResult.m_lightIntList)
            {
                ComputeBuffer cb = new ComputeBuffer(ints.Value.Count, sizeof(int));
                cb.SetData(ints.Value);
                mainShader.SetBuffer(0, ints.Key, cb);
                computeBuffersForLightProperties.Add(cb);
                // Do NOT release the compute buffer before the actual draw commands is being sent
            }

            foreach (var floats in sceneParseResult.m_lightFloatList)
            {
                ComputeBuffer cb = new ComputeBuffer(floats.Value.Count, sizeof(float));
                cb.SetData(floats.Value);
                mainShader.SetBuffer(0, floats.Key, cb);
                computeBuffersForLightProperties.Add(cb);
                // Do NOT release the compute buffer before the actual draw commands is being sent
            }

            foreach (var vector2s in sceneParseResult.m_lightVector2List)
            {
                ComputeBuffer cb = new ComputeBuffer(vector2s.Value.Count, sizeof(float) * 2);
                cb.SetData(vector2s.Value);
                mainShader.SetBuffer(0, vector2s.Key, cb);
                computeBuffersForLightProperties.Add(cb);
                // Do NOT release the compute buffer before the actual draw commands is being sent
            }

            foreach (var vector3s in sceneParseResult.m_lightVector3List)
            {
                ComputeBuffer cb = new ComputeBuffer(vector3s.Value.Count, sizeof(float) * 3);
                cb.SetData(vector3s.Value);
                mainShader.SetBuffer(0, vector3s.Key, cb);
                computeBuffersForLightProperties.Add(cb);
                // Do NOT release the compute buffer before the actual draw commands is being sent
            }

            foreach (var vector4s in sceneParseResult.m_lightVector4List)
            {
                ComputeBuffer cb = new ComputeBuffer(vector4s.Value.Count, sizeof(float) * 4);
                cb.SetData(vector4s.Value);
                mainShader.SetBuffer(0, vector4s.Key, cb);
                computeBuffersForLightProperties.Add(cb);
                // Do NOT release the compute buffer before the actual draw commands is being sent
            }
        }

        public static void LightToBuffer(in RTLight light,
                                         ref ComputeShader mainShader,
                                         SceneParseResult sceneParseResult)
        {
            FieldInfo[] fieldsInMat = GetFieldsInLight(light); // TODO: Later we may want to decide to include both public fields and private fields

            foreach (var field in fieldsInMat)
            {
                string fieldName = GetFieldName(lightName: GetMatName(light), field);

                var fieldValue = GetFieldValue(light, field);

                ProcessField(ref mainShader, fieldName, fieldValue, sceneParseResult);
            }
        }

        private static FieldInfo[] GetFieldsInLight(RTLight light)
        {
            return light.GetType().GetFields();
        }

        private static string GetFieldName(string lightName, FieldInfo field)
        {
            return $"{lightName}_{field.Name}";
        }

        private static object GetFieldValue(RTLight light, FieldInfo field)
        {
            return field.GetValue(light);
        }

        private static string GetMatName(RTLight material)
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
                    sceneParseResult.AddLightColor(name: fieldName, color: c);
                    break;

                case int i:
                    sceneParseResult.AddLightInt(name: fieldName, value: i);
                    break;

                case float f:
                    sceneParseResult.AddLightFloat(name: fieldName, value: f);
                    break;

                case Vector2 v2:
                    sceneParseResult.AddLightVector2(name: fieldName, value: v2);
                    break;

                case Vector3 v3:
                    sceneParseResult.AddLightVector3(name: fieldName, value: v3);
                    break;

                case Vector4 v4:
                    sceneParseResult.AddLightVector4(name: fieldName, value: v4);
                    break;

                case Texture2D texture2D:
                    sceneParseResult.AddLightTexture(name: fieldName, texture: texture2D);
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
