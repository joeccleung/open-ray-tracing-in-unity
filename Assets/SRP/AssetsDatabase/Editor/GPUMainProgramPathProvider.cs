namespace OpenRT {

    public class GPUMainProgramPathProvider {
        public const string CUSTOMER_SHADER_COLLECTION_DIR = "Assets/SRP/ComputeShader/Shading/";
        public const string GPU_MAIN_PROCESS_FILENAME = "JoeShade";

        /// <summary>
        /// Convert the shader root path to relative path w.r.t the GPU main process
        ///
        /// Currently, we just assume the custom shaders located within the same folder as the GPU main process (as prototype)
        ///</summary>
        public static string RelativeToGPUMain(string shaderPath) {
            return shaderPath.Replace(CUSTOMER_SHADER_COLLECTION_DIR, "");
        }
    }
}