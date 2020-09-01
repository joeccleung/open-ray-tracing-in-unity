using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OpenRT {
    [CustomEditor(typeof(RTMaterial), true)]
    public class RTMaterialInspector : Editor {
        public override void OnInspectorGUI() {
            
            serializedObject.Update();

            DrawDefaultInspector();

            var shaderGUIDProp = serializedObject.FindProperty("closestHitGUID");

            var shaderIndex = CustomShaderDatabase.Instance.GUIDToShaderIndex(shaderGUIDProp.stringValue, EShaderType.ClosestHit);

            EditorGUILayout.HelpBox($"Shader Index:{shaderIndex} GUID:{shaderGUIDProp.stringValue}", MessageType.None);

            if (shaderIndex == -1) {
                shaderIndex = 0;
            }

            var selectedShaderIndex = EditorGUILayout.Popup(
                "Closest Hit",
                shaderIndex,
                CustomShaderDatabase.Instance.ShaderNameList(EShaderType.ClosestHit));

            var selectedShaderName = CustomShaderDatabase.Instance.ShaderNameList(EShaderType.ClosestHit) [selectedShaderIndex];
            shaderGUIDProp.stringValue = CustomShaderDatabase.Instance.ShaderNameToGUID(selectedShaderName, EShaderType.ClosestHit);

            serializedObject.ApplyModifiedProperties();
        }
    }
}