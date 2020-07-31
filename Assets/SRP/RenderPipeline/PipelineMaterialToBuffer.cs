using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

namespace OpenRT {
    static class PipelineMaterialToBuffer {

        public static void MaterialsToBuffer(in List<RTMaterial> materials, ref ComputeShader mainShader) {
            foreach (var mat in materials) {
                MaterialToBuffer(material: mat, mainShader: ref mainShader);
            }
        }

        public static void MaterialToBuffer(in RTMaterial material, ref ComputeShader mainShader) {

            SceneTextureCollection sceneTexture = new SceneTextureCollection();

            FieldInfo[] fieldsInMat = GetFieldsInMaterial(material); // TODO: Later we may want to decide to include both public fields and private fields

            foreach (var field in fieldsInMat) {
                string fieldName = GetFieldName(matName: GetMatName(material), field);

                var fieldValue = GetFieldValue(material, field);

                ProcessField(ref mainShader, ref sceneTexture, fieldName, fieldValue);
            }

        }

        private static FieldInfo[] GetFieldsInMaterial(RTMaterial material) {
            return material.GetType().GetFields();
        }

        private static string GetMatName(RTMaterial material) {
            return material.GetType().Name;
        }

        private static void ProcessField(ref ComputeShader mainShader,
                                         ref SceneTextureCollection sceneTexture,
                                         string fieldName,
                                         object fieldValue) {
            if (fieldValue == null) {
                return;
            }

            if (fieldValue is Texture2D tex) {
                RegisterTexture(fieldName, tex, ref sceneTexture);
            } else {
                AssignFieldToMainShader(fieldName, fieldValue, ref mainShader);
            }
        }

        private static string GetFieldName(string matName, FieldInfo field) {
            return $"{matName}_{field.Name}";
        }

        private static object GetFieldValue(RTMaterial material, FieldInfo field) {
            return field.GetValue(material);
        }

        private static void AssignFieldToMainShader(string fieldName, in object fieldValue,
            ref ComputeShader mainShader) {
            switch (fieldValue) {
                case Color c:
                    mainShader.SetVector(name: fieldName, val: c);
                    break;

                case int i:
                    mainShader.SetInt(name: fieldName, val: i);
                    break;

                case float f:
                    mainShader.SetFloat(name: fieldName, val: f);
                    break;

                case Vector2 v2:
                    mainShader.SetVector(name: fieldName, val: v2);
                    break;

                case Vector3 v3:
                    mainShader.SetVector(name: fieldName, val: v3);
                    break;

                case Vector4 v4:
                    mainShader.SetVector(name: fieldName, val: v4);
                    break;
            }
        }

        private static void RegisterTexture(string fieldName, Texture2D texture, ref SceneTextureCollection texturesCol) {
            texturesCol.AddTexture(fieldName, texture);
        }
    }
}