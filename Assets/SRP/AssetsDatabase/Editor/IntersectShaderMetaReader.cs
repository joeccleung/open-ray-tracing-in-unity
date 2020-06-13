using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public class IntersectShaderMetaReader : IShaderMetaReader {
        public bool CanHandle(string fileContent, out string shaderName) {
            int start = fileContent.IndexOf("// [intersect(");
            if (start == -1) {
                // Missing shader name
                shaderName = null;
                return false;
            }

            int end = fileContent.IndexOf(")]");
            if (end == -1) {
                // malformed shader name directive
                shaderName = null;
                return false;
            }

            shaderName = fileContent.Substring(start + 14, end - start - 14);
            return true;
        }

        public void CustomShaderMeta(string fileContent) {
            throw new System.NotImplementedException();
        }
    }
}