using System.Collections;
using System.Collections.Generic;
using OpenRT;
using UnityEngine;

public abstract class IRTMeshBVH : RTGeometry, RTMeshBVHController.IActuator
{
    public abstract List<List<float>> BuildBVHAndTriangleList(int geoLocalToGlobalIndexOffset,
                                                              int mappingLocalToGlobalIndexOffset);
    public abstract List<List<float>> BuildBVHAndTriangleList(Vector3[] normals, int[] trianglesVertexOrder, Vector3[] vertices);
    public abstract BVHNode GetRoot();
    public abstract int[] GetTrianglesVertexOrder(int bitmap);
    public abstract Vector3[] GetVertices();
    public abstract Vector3 LocalToWorld(Vector3 local);
}
