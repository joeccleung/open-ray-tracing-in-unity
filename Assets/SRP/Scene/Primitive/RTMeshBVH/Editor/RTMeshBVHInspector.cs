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
        }
    }
}
