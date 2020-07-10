using System.Collections;
using System.Collections.Generic;
using OpenRT;
using UnityEngine;

namespace Example {
    public class CustomSphere : OpenRT.RTGeometry {

        [SerializeField] private CustomSphereData sphereData;

        public override RTBoundingBox GetBoundingBox() {
            return boundingBox;
        }

        public override int GetCount() {
            return sphereData.GetCount();
        }

        public override List<float> GetGeometryInstanceData() {

            sphereData.SetGameObjectCenter(transform.position);

            Vector3 r3 = new Vector3(sphereData.radius, sphereData.radius, sphereData.radius);
            boundingBox.min = sphereData.center - r3;
            boundingBox.max = sphereData.center + r3;

            return sphereData.GetGeometryInstanceData();
        }

        public override int GetStride() {
            return sphereData.GetStride();
        }
    }

}