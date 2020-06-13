using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public class CustomShaderDatabaseFile {
        public Dictionary<string, CustomShaderMeta> intersect = new Dictionary<string, CustomShaderMeta>();

        public Dictionary<string, CustomShaderMeta> closetHit = new Dictionary<string, CustomShaderMeta>();

        public Dictionary<string, CustomShaderMeta> lights = new Dictionary<string, CustomShaderMeta>();
    }

}