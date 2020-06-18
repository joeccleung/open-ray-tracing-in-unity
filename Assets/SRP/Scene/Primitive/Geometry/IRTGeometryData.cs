using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public interface IRTGeometryData {
        List<float> GetGeometryInstanceData();
        int GetCount();
        int GetStride();
    }

}