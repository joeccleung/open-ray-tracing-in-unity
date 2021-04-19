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
            DrawDefaultInspector();

            var m_shaderIndexProp = serializedObject.FindProperty("lightIndex");

            m_shaderIndexProp.intValue = EditorGUILayout.Popup("Light Shader", m_shaderIndexProp.intValue, CustomShaderDatabase.Instance.ShaderNameList(EShaderType.Light));

            if (m_shaderIndexProp.intValue == -1)
            {
                m_shaderIndexProp.intValue = 0;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}