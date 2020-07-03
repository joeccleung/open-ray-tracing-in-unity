using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace OpenRT {
    [CustomEditor(typeof(RTGeometry), true)]
    public class RTGeometryInspector : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            var shaderGUIDProp = serializedObject.FindProperty("intersectShaderGUID");

            var shaderIndex = CustomShaderDatabase.Instance.GUIDToShaderIndex(shaderGUIDProp.stringValue, EShaderType.Intersect);

            if (shaderIndex == -1) {
                shaderIndex = 0;
            }

            var selectedShaderIndex = EditorGUILayout.Popup(
                "Intersect",
                shaderIndex,
                CustomShaderDatabase.Instance.ShaderNameList(EShaderType.Intersect));

            var selectedShaderName = CustomShaderDatabase.Instance.ShaderNameList(EShaderType.Intersect) [selectedShaderIndex];
            shaderGUIDProp.stringValue = CustomShaderDatabase.Instance.ShaderNameToGUID(selectedShaderName, EShaderType.Intersect);

            serializedObject.ApplyModifiedProperties();
        }
    }

}