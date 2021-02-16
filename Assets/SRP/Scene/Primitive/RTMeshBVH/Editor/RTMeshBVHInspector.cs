using UnityEditor;
using UnityEngine;

namespace OpenRT
{
    [CustomEditor(typeof(RTMeshBVH), true)]
    public class RTMeshBVHInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            RTMeshBVH myScript = (RTMeshBVH)target;
            if (GUILayout.Button("Mark dirty"))
            {
                myScript.MarkMeshAsDirty();
            }

            var shaderGUIDProp = serializedObject.FindProperty("intersectShaderGUID");

            var shaderIndex = CustomShaderDatabase.Instance.GUIDToShaderIndex(shaderGUIDProp.stringValue, EShaderType.Intersect);

            if (shaderIndex == -1)
            {
                shaderIndex = 0;
            }

            var selectedShaderIndex = EditorGUILayout.Popup(
                "Intersect",
                shaderIndex,
                CustomShaderDatabase.Instance.ShaderNameList(EShaderType.Intersect));

            var selectedShaderName = CustomShaderDatabase.Instance.ShaderNameList(EShaderType.Intersect)[selectedShaderIndex];
            shaderGUIDProp.stringValue = CustomShaderDatabase.Instance.ShaderNameToGUID(selectedShaderName, EShaderType.Intersect);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
