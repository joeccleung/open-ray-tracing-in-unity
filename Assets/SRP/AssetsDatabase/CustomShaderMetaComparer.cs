using System.Collections;
using System.Collections.Generic;
using GUID = System.String;

namespace OpenRT {
    public class CustomShaderMetaComparer : IComparer<CustomShaderMeta> {
        public int Compare(CustomShaderMeta x, CustomShaderMeta y) {
            return x.name.CompareTo(y.name);
        }
    }

    public class CustomShaderMetaGUIDComparer : IComparer<GUID>
    {
        public int Compare(GUID x, GUID y)
        {
            return x.CompareTo(y);
        }
    }
}