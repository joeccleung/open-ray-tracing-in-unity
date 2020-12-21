using System.Collections;
using System.Collections.Generic;
using OpenRT;
using UnityEngine;

public class RTMeshBVHDemoGeometry : RTGeometry, RTMeshBVHController.IActuator
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

    private Vector3[] vertices = new Vector3[] {
        new Vector3(0, 0, 0),
        new Vector3(0, 5, 0),
        new Vector3(5, 5, 0),
        new Vector3(5, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 5),
        new Vector3(5, 0, 5),
        new Vector3(5, 0, 0) };

    private Vector3[] normal = new Vector3[] {
        new Vector3(0, 0, -1),
        new Vector3(0, 0, -1),
        new Vector3(0, 0, -1),
        new Vector3(0, 0, -1),
        new Vector3(0, -1, 0),
        new Vector3(0, -1, 0),
        new Vector3(0, -1, 0),
        new Vector3(0, -1, 0)
    };

    private int[] triangle = new int[] {
        0, 1, 2,
        0, 2, 3,
        4, 7, 6,
        4, 6, 5
    };

    public override RTBoundingBox GetBoundingBox()
    {
        return controller.GetBoundingBox();
    }

    public override int GetCount()
    {
        return 1;
    }

    public override List<float> GetGeometryInstanceData()
    {
        return controller.GetGeometryInstanceData();
    }

    public override Vector3[] GetNormals()
    {
        return normal;
    }

    public override int GetStride()
    {
        return sizeof(float);
    }

    public int[] GetTrianglesVertexOrder(int bitmap)
    {
        return triangle;
    }

    public Vector3[] GetVertices()
    {
        return vertices;
    }

    public Vector3 LocalToWorld(Vector3 local)
    {
        return transform.localToWorldMatrix.MultiplyPoint(local);
    }

    public override bool IsUnevenStride()
    {
        return true;
    }

}
