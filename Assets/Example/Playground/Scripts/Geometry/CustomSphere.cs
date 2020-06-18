using System.Collections;
using System.Collections.Generic;
using OpenRT;
using UnityEngine;

namespace Example {
    public class CustomSphere : OpenRT.RTGeometry {

        [SerializeField] private CustomSphereData sphereData;

        public override int GetCount()
        {
            return sphereData.GetCount();
        }

        public override List<float> GetGeometryInstanceData()
        {
            sphereData.SetGameObjectCenter(transform.position);
            return sphereData.GetGeometryInstanceData();
        }

        public override int GetStride()
        {
            return sphereData.GetStride();
        }
    }

}