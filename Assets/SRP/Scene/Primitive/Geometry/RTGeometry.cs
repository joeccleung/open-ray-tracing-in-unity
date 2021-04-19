using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{
    [ExecuteAlways]
    public abstract class RTGeometry : MonoBehaviour, IRTGeometryData
    {
        [HideInInspector, SerializeField] private string intersectShaderGUID = string.Empty;

        protected RTBoundingBox boundingBox = RTBoundingBox.Empty;
        protected bool prevFrameIsEnable = false;

        protected virtual void Start()
        {
            prevFrameIsEnable = false;
        }

        public virtual List<float> GetAccelerationStructureGeometryData(int geoLocalToGlobalIndexOffset, int mappingLocalToGlobalIndexOffset)
        {
            return null;
        }

        public virtual List<int> GetAccelerationStructureGeometryMapping(int geoLocalToGlobalIndexOffset, int mappingLocalToGlobalIndexOffset)
        {
            return null;
        }

        public abstract RTBoundingBox GetTopLevelBoundingBox(int assginedPrimitiveId);

        public abstract int GetCount();

        public abstract List<float> GetGeometryInstanceData(int geoLocalToGlobalIndexOffset, int mappingLocalToGlobalIndexOffset);  // Either a standalone geometry or the BVH (not the triangles inside BVH)

        public string GetIntersectShaderGUID()
        {
            return intersectShaderGUID;
        }

        public virtual bool IsAccelerationStructure()
        {
            return false;
        }

        public abstract bool IsDirty();

        public abstract bool IsGeometryValid();

        public abstract Vector3[] GetNormals();

        public abstract int GetStride();
    }
}