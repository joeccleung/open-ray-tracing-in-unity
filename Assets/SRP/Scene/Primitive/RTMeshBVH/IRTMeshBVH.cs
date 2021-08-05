using System.Collections;
using System.Collections.Generic;
using OpenRT;
using UnityEngine;

public abstract class IRTMeshBVH : RTGeometry, RTMeshBVHController.IActuator
{
    public abstract void BuildBVHAndTriangleList(int geoLocalToGlobalIndexOffset,
                                                 int mappingLocalToGlobalIndexOffset);
    public abstract void BuildBVHAndTriangleList(Vector3[] normals,
                                                 int[] trianglesVertexOrder,
                                                 Vector2[] uvs,
                                                 Vector3[] vertices);
    public abstract BVHNode GetRoot();
    public abstract int[] GetTrianglesVertexOrder(int bitmap);
    public abstract Vector2[] GetUVs();
    public abstract Vector3[] GetVertices();
    public abstract Vector3 LocalToWorldDirection(Vector3 local);

    public abstract Vector3 LocalToWorldVertex(Vector3 local);
}
