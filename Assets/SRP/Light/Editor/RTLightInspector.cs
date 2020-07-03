using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OpenRT {
    [CustomEditor(typeof(RTLight), true)]
    public class RTLightInspector : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            var m_shaderIndexProp = serializedObject.FindProperty("shaderIndex");

            m_shaderIndexProp.intValue = EditorGUILayout.Popup("Shader", m_shaderIndexProp.intValue, CustomShaderDatabase.Instance.ShaderNameList(EShaderType.CloestHit));

            serializedObject.ApplyModifiedProperties();
        }
    }
}