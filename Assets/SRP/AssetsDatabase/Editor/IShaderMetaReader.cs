using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public interface IShaderMetaReader {
        bool CanHandle(string fileContent, out string shaderName);
        void CustomShaderMeta(string fileContent);
    }
}