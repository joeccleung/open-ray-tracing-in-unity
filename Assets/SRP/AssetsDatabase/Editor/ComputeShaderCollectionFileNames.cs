using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OpenRT
{
    public class ComputeShaderCollectionFileNames
    {
        // The filenames listed below are ignored by compute shader importer
        public const string CUSTOM_INTERSECT_SHADER_COLLECTION_FILENAME = "CustomGeometry";
        public const string CUSTOM_SHADER_COLLECTION_DIR = "Assets/SRP/ComputeShader/Shading/";
        public const string CUSTOM_LIGHT_SHADER_COLLECTION_FILENAME = "CustomLights";
        public const string CUSTOM_SHADER_COLLECTION_FILENAME = "CustomShaderCollection";
        public const string GPU_MAIN_PROCESS_FILENAME = "JoeShade";


        public static bool ShouldProcess(in string path)
        {
            string ext = Path.GetExtension(path);
            string filename = Path.GetFileNameWithoutExtension(path);

            if (ext == ".compute")
            {
                if (filename == CUSTOM_INTERSECT_SHADER_COLLECTION_FILENAME)
                {
                    return false;
                }

                if (filename == CUSTOM_SHADER_COLLECTION_FILENAME)
                {
                    return false;
                }

                if (filename == CUSTOM_LIGHT_SHADER_COLLECTION_FILENAME)
                {
                    return false;
                }

                if (filename == GPU_MAIN_PROCESS_FILENAME)
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
