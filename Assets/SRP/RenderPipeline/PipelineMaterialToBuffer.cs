using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

namespace OpenRT {
    static class PipelineMaterialToBuffer {

        public static void MaterialsToBuffer(in List<RTMaterial> materials, ref CommandBuffer buffer) {
            foreach (var mat in materials) {
                MaterialToBuffer(material: mat, buffer: ref buffer);
            }
        }

        public static void MaterialToBuffer(in RTMaterial material, ref CommandBuffer buffer) {
            string matName = material.GetType().Name;

            var fields = material.GetType().GetFields(); // TODO: Later we may want to decide to include both public fields and private fields

            foreach (var field in fields) {
                string fieldName = $"{matName}_{field.Name}";
                switch (field.GetValue(material)) {
                    case Color c:
                        buffer.SetGlobalColor(name: fieldName, value: c);
                        break;

                    case int i:
                        buffer.SetGlobalInt(name: fieldName, value: i);
                        break;

                    case float f:
                        buffer.SetGlobalFloat(name: fieldName, value: f);
                        break;

                    case Texture2D t:
                        // TODO: Support Texture2D
                        break;

                    case Vector2 v2:
                        buffer.SetGlobalVector(name: fieldName, value: v2);
                        break;

                    case Vector3 v3:
                        buffer.SetGlobalVector(name: fieldName, value: v3);
                        break;

                    case Vector4 v4:
                        buffer.SetGlobalVector(name: fieldName, value: v4);
                        break;
                }
            }
        }
    }
}