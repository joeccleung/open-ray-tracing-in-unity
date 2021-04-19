using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{
    public class LightShaderMetaReader : IShaderMetaReader
    {
        public const int PREFIX_LENGTH = 10;

        public bool CanHandle(string fileContent, out string shaderName)
        {
            int start = fileContent.IndexOf("// [light(");  // PREFIX_LENGTH = 10
            if (start == -1)
            {
                // Missing shader name
                shaderName = null;
                return false;
            }

            int end = fileContent.IndexOf(")]");
            if (end == -1)
            {
                // malformed shader name directive
                shaderName = null;
                return false;
            }

            shaderName = fileContent.Substring(start + PREFIX_LENGTH, end - start - PREFIX_LENGTH);
            return true;
        }

        public void CustomShaderMeta(string fileContent)
        {
            throw new System.NotImplementedException();
        }
    }
}