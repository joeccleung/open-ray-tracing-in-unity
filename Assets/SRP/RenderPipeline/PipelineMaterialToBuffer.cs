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
            string matName = material.GetType().Name;

            var fields = material.GetType().GetFields(); // TODO: Later we may want to decide to include both public fields and private fields

            foreach (var field in fields) {
                string fieldName = $"{matName}_{field.Name}";
                switch (field.GetValue(material)) {
                    case Color c:
                        mainShader.SetVector(name: fieldName, val: c);
                        break;

                    case int i:
                        mainShader.SetInt(name: fieldName, val: i);
                        break;

                    case float f:
                        mainShader.SetFloat(name: fieldName, val: f);
                        break;

                    case Texture2D t:
                        // TODO: Support Texture2D
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
        }
    }
}