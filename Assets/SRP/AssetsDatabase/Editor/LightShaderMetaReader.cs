using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public class LightShaderMetaReader : IShaderMetaReader {
        public bool CanHandle(string fileContent, out string shaderName) {
            throw new System.NotImplementedException();
        }

        public void CustomShaderMeta(string fileContent) {
            throw new System.NotImplementedException();
        }
    }
}