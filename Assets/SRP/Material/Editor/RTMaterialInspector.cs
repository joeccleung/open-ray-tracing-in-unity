using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OpenRT {
    [CustomEditor(typeof(RTMaterial), true)]
    public class RTMaterialInspector : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            var m_shaderIndexProp = serializedObject.FindProperty("shaderIndex");
            var m_intersectShaderIndexProp = serializedObject.FindProperty("intersectShaderIndex");

            m_shaderIndexProp.intValue = EditorGUILayout.Popup("Shader", m_shaderIndexProp.intValue, CustomShaderDatabase.Instance.closestHitShaderNameList);

            m_intersectShaderIndexProp.intValue = EditorGUILayout.Popup("Intersect", m_intersectShaderIndexProp.intValue, CustomShaderDatabase.Instance.intersectShaderNameList);

            serializedObject.ApplyModifiedProperties();
        }
    }
}