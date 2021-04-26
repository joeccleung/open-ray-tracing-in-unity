using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OpenRT
{
    [CustomEditor(typeof(RTLight), true)]
    public class RTLightInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            var shaderGUIDProp = serializedObject.FindProperty("shaderGUID");

            var shaderIndex = CustomShaderDatabase.Instance.GUIDToShaderIndex(shaderGUIDProp.stringValue, EShaderType.Light);

            if (shaderIndex == -1)
            {
                EditorGUILayout.HelpBox($"Unable to find light shader {shaderGUIDProp.stringValue}", MessageType.Warning);
                shaderIndex = 0;
            }
            else
            {
                EditorGUILayout.HelpBox($"Shader Index:{shaderIndex} GUID:{shaderGUIDProp.stringValue}", MessageType.None);
            }

            var selectedShaderIndex = EditorGUILayout.Popup(
                "Light Shader",
                shaderIndex,
                CustomShaderDatabase.Instance.ShaderNameList(EShaderType.Light));

            var selectedShaderName = CustomShaderDatabase.Instance.ShaderNameList(EShaderType.Light) [selectedShaderIndex];
            shaderGUIDProp.stringValue = CustomShaderDatabase.Instance.ShaderNameToGUID(selectedShaderName, EShaderType.Light);

            serializedObject.ApplyModifiedProperties();
        }
    }
}