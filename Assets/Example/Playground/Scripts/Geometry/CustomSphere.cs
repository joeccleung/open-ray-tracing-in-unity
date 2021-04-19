using System.Collections;
using System.Collections.Generic;
using OpenRT;
using UnityEngine;

namespace Example
{
    public class CustomSphere : OpenRT.RTGeometry
    {

        [SerializeField] private CustomSphereData sphereData;
        private Vector3 m_previousSphereCenter = Vector3.zero;
        private float m_previousSphereRadius = 0;

        public override int GetCount()
        {
            return sphereData.GetCount();
        }

        public override List<float> GetGeometryInstanceData(int geoLocalToGlobalIndexOffset, int mappingLocalToGlobalIndexOffset)
        {

            sphereData.SetGameObjectCenter(transform.position);

            Vector3 r3 = new Vector3(sphereData.radius, sphereData.radius, sphereData.radius);
            boundingBox.min = sphereData.center - r3;
            boundingBox.max = sphereData.center + r3;

            return sphereData.GetGeometryInstanceData(geoLocalToGlobalIndexOffset, mappingLocalToGlobalIndexOffset);
        }

        public override Vector3[] GetNormals()
        {
            return null;
        }

        public override int GetStride()
        {
            return sphereData.GetStride();
        }

        public override RTBoundingBox GetTopLevelBoundingBox(int assignedPrimitiveId)
        {
            boundingBox.geoIndices = new HashSet<int> { assignedPrimitiveId };
            return boundingBox;
        }

        public override bool IsDirty()
        {
            bool isDirty = false;

            if (prevFrameIsEnable != gameObject.activeInHierarchy)
            {
                prevFrameIsEnable = gameObject.activeInHierarchy;
                isDirty = true;
            }

            if (transform.hasChanged)
            {
                transform.hasChanged = false;
                isDirty = true;
            }

            if (m_previousSphereCenter != sphereData.center && m_previousSphereRadius == sphereData.radius)
            {
                m_previousSphereCenter = sphereData.center;
                m_previousSphereRadius = sphereData.radius;
                isDirty = true;
            }

            return isDirty;
        }

        public override bool IsGeometryValid()
        {
            return true;
        }
    }

}