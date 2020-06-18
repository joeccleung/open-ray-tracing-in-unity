using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

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

            var selectedShaderIndex = EditorGUILayout.Popup("Intersect", shaderIndex, CustomShaderDatabase.Instance.intersectShaderNameList);

            var selectedShaderName = CustomShaderDatabase.Instance.intersectShaderNameList[selectedShaderIndex];
            shaderGUIDProp.stringValue = CustomShaderDatabase.Instance.ShaderNameToGUID(selectedShaderName, EShaderType.Intersect);

            serializedObject.ApplyModifiedProperties();
        }
    }

}