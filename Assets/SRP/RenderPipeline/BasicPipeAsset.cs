using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace OpenRT {
    [ExecuteInEditMode]
    public class BasicPipeAsset : RenderPipelineAsset {
        public List<RenderPipelineConfigObject> m_config;
        public Color clearColor = Color.green;
        public ComputeShader mainShader;

        public bool useSoftShadow = true;

#if UNITY_EDITOR
        // Call to create a simple pipeline
        [UnityEditor.MenuItem("SRP-Demo/01 - Create Basic Asset Pipeline")]
        static void CreateBasicAssetPipeline() {
            var instance = ScriptableObject.CreateInstance<BasicPipeAsset>();
            UnityEditor.AssetDatabase.CreateAsset(instance, "Assets/BasicPipeAsset.asset");
        }
#endif

        // Function to return an instance of this pipeline
        // This implementation for Unity 2019.2
        protected override RenderPipeline CreatePipeline() {
            return new BasicPipeInstance(clearColor, mainShader, m_config); // Our custom renderer named BasicPipeInstance
        }

        // Old implementation from Unity 2018.4
        //  protected override IRenderPipeline InternalCreatePipeline()
        //  {
        //      return new BasicPipeInstance(clearColor);   // Our custom renderer named BasicPipeInstance
        //  }
    }
}