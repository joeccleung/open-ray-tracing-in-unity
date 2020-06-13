using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public class ClosetHitShaderMetaReader : IShaderMetaReader {
        public bool CanHandle(string fileContent, out string shaderName) {
            int start = fileContent.IndexOf("// [shader(");
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

            shaderName = fileContent.Substring(start + 11, end - start - 11);
            return true;
        }

        public void CustomShaderMeta(string fileContent) {
            throw new System.NotImplementedException();
        }
    }
}