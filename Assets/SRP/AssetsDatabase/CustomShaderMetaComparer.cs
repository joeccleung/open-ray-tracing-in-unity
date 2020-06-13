using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public class CustomShaderMetaComparer : IComparer<CustomShaderMeta> {
        public int Compare(CustomShaderMeta x, CustomShaderMeta y) {
            return x.name.CompareTo(y.name);
        }
    }
}