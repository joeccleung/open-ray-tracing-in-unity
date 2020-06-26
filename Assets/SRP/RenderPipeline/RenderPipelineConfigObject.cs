using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    [CreateAssetMenu(menuName = "Render Pipeline Config")]
    public class RenderPipelineConfigObject : ScriptableObject {
        public Texture skybox;

        public Color upperAmbitent = Color.white;

        public Color lowerAmbitent = Color.white;

        public float globalRefractiveIndex = 1.0f;

        [UnityEngine.Range(1, 8)] public int maxRayGeneration = 1;

        public bool enableShadow = false;

        public Color fogColor = Color.white;

        [UnityEngine.Range(0, 0.05f)] public float fogFactor = 0;

        public int rayGenId = 0;

        private void Awake() {
            if (skybox == null) {
                skybox = new Texture2D(2, 2);
            }
        }
    }
}