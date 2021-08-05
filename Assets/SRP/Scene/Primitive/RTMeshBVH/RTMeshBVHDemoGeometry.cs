using System.Collections;
using System.Collections.Generic;
using OpenRT;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class RTMeshBVHDemoGeometry : IRTMeshBVH
{
    private RTMeshBVHController m_controller;
    private RTMeshBVHController controller
    {
        get
        {
            m_controller = m_controller ?? new RTMeshBVHController(this);
            return m_controller;
        }
    }

    [SerializeField] private Mesh demoMesh;
    [SerializeField] private MeshFilter meshFilter;
    private int m_meshHashCode = 0;
    [SerializeField] private int m_minNumberOfGeoPerBox = 0;
    [SerializeField] private int numberOfTriangelsFromDemoMesh = 1;

    public void Awake()
    {
        var deformedMesh = new Mesh
        {
            vertices = GetVertices(),   // Must set vertices first
            normals = GetNormals(),
            triangles = GetTrianglesVertexOrder(0),
        };

        meshFilter.mesh = deformedMesh;
    }

    public override void BuildBVHAndTriangleList(int geoLocalToGlobalIndexOffset,
                                                 int mappingLocalToGlobalIndexOffset)
    {
        controller.BuildBVHAndTriangleList(geoLocalToGlobalIndexOffset,
                                           mappingLocalToGlobalIndexOffset,
                                           m_minNumberOfGeoPerBox);
    }

    public override void BuildBVHAndTriangleList(Vector3[] normals,
                                                 int[] trianglesVertexOrder,
                                                 Vector2[] uvs,
                                                 Vector3[] vertices)
    {
        controller.BuildBVHAndTriangleList(m_minNumberOfGeoPerBox,
                                           normals,
                                           trianglesVertexOrder,
                                           uvs,
                                           vertices);
    }

    public override List<float> GetAccelerationStructureGeometryData(int geoLocalToGlobalIndexOffset, int mappingLocalToGlobalIndexOffset)
    {
        return controller.GetAccelerationStructureGeometryData(geoLocalToGlobalIndexOffset, mappingLocalToGlobalIndexOffset, m_minNumberOfGeoPerBox);
    }

    public override List<int> GetAccelerationStructureGeometryMapping(int geoLocalToGlobalIndexOffset, int mappingLocalToGlobalIndexOffset)
    {
        return controller.GetAccelerationStructureGeometryMapping(geoLocalToGlobalIndexOffset, mappingLocalToGlobalIndexOffset, m_minNumberOfGeoPerBox);
    }

    public override RTBoundingBox GetTopLevelBoundingBox(int assignedPrimitiveId)
    {
        return controller.GetTopLevelBoundingBox(assignedPrimitiveId);
    }

    public override int GetCount()
    {
        return 1;
    }

    public override List<float> GetGeometryInstanceData(int geoLocalToGlobalIndexOffset, int mappingLocalToGlobalIndexOffset)
    {
        return controller.GetGeometryInstanceData(geoLocalToGlobalIndexOffset, mappingLocalToGlobalIndexOffset, m_minNumberOfGeoPerBox);
    }

    public override Vector3[] GetNormals()
    {
        return demoMesh.normals;
    }

    public override BVHNode GetRoot()
    {
        return controller.GetRoot();
    }

    public override int GetStride()
    {
        return sizeof(float);
    }

    public override int[] GetTrianglesVertexOrder(int bitmap)
    {
        return demoMesh.triangles.Take(numberOfTriangelsFromDemoMesh * 3).ToArray();
    }

    public override Vector2[] GetUVs()
    {
        return demoMesh.uv;
    }

    public override Vector3[] GetVertices()
    {
        return demoMesh.vertices;
    }

    public override bool IsAccelerationStructure()
    {
        return true;
    }

    public override bool IsDirty()
    {
        if (transform.hasChanged)
        {
            transform.hasChanged = false;
            return true;
        }

        int _curHashCode = demoMesh.GetHashCode();
        if (m_meshHashCode != _curHashCode)
        {
            m_meshHashCode = _curHashCode;
            return true;
        }

        return false;
    }

    public override bool IsGeometryValid()
    {
        return true;
    }

    public override Vector3 LocalToWorldVertex(Vector3 local)
    {
        return transform.localToWorldMatrix.MultiplyPoint(local);
    }

    public override Vector3 LocalToWorldDirection(Vector3 local)
    {
        return transform.localToWorldMatrix.MultiplyVector(local);
    }
}
