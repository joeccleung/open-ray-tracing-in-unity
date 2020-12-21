using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Moq;

namespace OpenRT.UnitTests.Primitive
{
    public class RTMeshBVH_TriangleList_UnitTests
    {
        [Test]
        public void Serialize_OneTriangle_BVH()
        {
            // Assign
            Vector3[] vertices = new Vector3[3];
            vertices[0] = new Vector3(0, 0, 0);
            vertices[1] = new Vector3(0, 5, 0);
            vertices[2] = new Vector3(5, 0, 0);

            var normal = new Vector3(0, 0, -1); // Facing user
            var planeD = -1 * Vector3.Dot(normal, vertices[0]);
            var area = Vector3.Dot(normal, Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]));

            int[] trianglesVertexOrder = new int[3];
            trianglesVertexOrder[0] = 0;
            trianglesVertexOrder[1] = 1;
            trianglesVertexOrder[2] = 2;

            var actuator = new Mock<RTMeshBVHController.IActuator>();
            actuator.Setup(x => x.GetVertices()).Returns(vertices);
            actuator.Setup(x => x.LocalToWorld(It.IsAny<Vector3>())).Returns<Vector3>(v => v);

            RTMeshBVHController controller = new RTMeshBVHController(actuator: actuator.Object);

            // Act
            List<List<float>> triangles = controller.BuildBVHAndTriangleList(trianglesVertexOrder, vertices);

            // Assert
            Assert.AreEqual(1, triangles.Count);    // There is only 1 triangle
            var firstTriangle = triangles[0];
            Assert.AreEqual(14, firstTriangle.Count);   // There are 14 number of floats in one triangle
            Assert.AreEqual(vertices[0].x, firstTriangle[0]);   // First vertex
            Assert.AreEqual(vertices[0].y, firstTriangle[1]);
            Assert.AreEqual(vertices[0].z, firstTriangle[2]);
            Assert.AreEqual(vertices[1].x, firstTriangle[3]);   // Second vertex
            Assert.AreEqual(vertices[1].y, firstTriangle[4]);
            Assert.AreEqual(vertices[1].z, firstTriangle[5]);
            Assert.AreEqual(vertices[2].x, firstTriangle[6]);   // Thrid vertex
            Assert.AreEqual(vertices[2].y, firstTriangle[7]);
            Assert.AreEqual(vertices[2].z, firstTriangle[8]);
            Assert.AreEqual(normal.x, firstTriangle[9]);    // Normal
            Assert.AreEqual(normal.y, firstTriangle[10]);
            Assert.AreEqual(normal.z, firstTriangle[11]);
            Assert.AreEqual(planeD, firstTriangle[12]);     // Plane D
            Assert.AreEqual(area, firstTriangle[13]);       // Area
        }

        [Test]
        public void Serialize_Quard_BVH()
        {
            // Assign
            Vector3[] vert = new Vector3[4];
            vert[0] = new Vector3(0, 0, 0);
            vert[1] = new Vector3(0, 5, 0);
            vert[2] = new Vector3(5, 0, 0);
            vert[3] = new Vector3(5, 5, 0);

            var normal1 = new Vector3(0, 0, -1); // Facing user
            var planeD1 = -1 * Vector3.Dot(normal1, vert[0]);
            var area1 = Vector3.Dot(normal1, Vector3.Cross(vert[1] - vert[0], vert[2] - vert[0]));

            var normal2 = new Vector3(0, 0, -1); // Facing user
            var planeD2 = -1 * Vector3.Dot(normal2, vert[1]);
            var area2 = Vector3.Dot(normal2, Vector3.Cross(vert[3] - vert[1], vert[2] - vert[1]));

            int[] trianglesVertexOrder = new int[6];
            trianglesVertexOrder[0] = 0;
            trianglesVertexOrder[1] = 1;
            trianglesVertexOrder[2] = 2;
            trianglesVertexOrder[3] = 1;
            trianglesVertexOrder[4] = 3;
            trianglesVertexOrder[5] = 2;

            var actuator = new Mock<RTMeshBVHController.IActuator>();
            actuator.Setup(x => x.GetVertices()).Returns(vert);
            actuator.Setup(x => x.LocalToWorld(It.IsAny<Vector3>())).Returns<Vector3>(v => v);

            RTMeshBVHController controller = new RTMeshBVHController(actuator: actuator.Object);

            // Act
            List<List<float>> triangles = controller.BuildBVHAndTriangleList(trianglesVertexOrder, vert);

            // Assert
            Assert.AreEqual(2, triangles.Count);    // There are 2 triangle
            var firstTriangle = triangles[0];
            Assert.AreEqual(14, firstTriangle.Count);   // There are 14 number of floats in one triangle
            Assert.AreEqual(vert[0].x, firstTriangle[0]);   // First vertex
            Assert.AreEqual(vert[0].y, firstTriangle[1]);
            Assert.AreEqual(vert[0].z, firstTriangle[2]);
            Assert.AreEqual(vert[1].x, firstTriangle[3]);   // Second vertex
            Assert.AreEqual(vert[1].y, firstTriangle[4]);
            Assert.AreEqual(vert[1].z, firstTriangle[5]);
            Assert.AreEqual(vert[2].x, firstTriangle[6]);   // Thrid vertex
            Assert.AreEqual(vert[2].y, firstTriangle[7]);
            Assert.AreEqual(vert[2].z, firstTriangle[8]);
            Assert.AreEqual(normal1.x, firstTriangle[9]);    // Normal
            Assert.AreEqual(normal1.y, firstTriangle[10]);
            Assert.AreEqual(normal1.z, firstTriangle[11]);
            Assert.AreEqual(planeD1, firstTriangle[12]);     // Plane D
            Assert.AreEqual(area1, firstTriangle[13]);       // Area

            var secondTriangle = triangles[0];
            Assert.AreEqual(14, secondTriangle.Count);   // There are 14 number of floats in one triangle
            Assert.AreEqual(vert[0].x, secondTriangle[0]);   // First vertex
            Assert.AreEqual(vert[0].y, secondTriangle[1]);
            Assert.AreEqual(vert[0].z, secondTriangle[2]);
            Assert.AreEqual(vert[1].x, secondTriangle[3]);   // Second vertex
            Assert.AreEqual(vert[1].y, secondTriangle[4]);
            Assert.AreEqual(vert[1].z, secondTriangle[5]);
            Assert.AreEqual(vert[2].x, secondTriangle[6]);   // Thrid vertex
            Assert.AreEqual(vert[2].y, secondTriangle[7]);
            Assert.AreEqual(vert[2].z, secondTriangle[8]);
            Assert.AreEqual(normal1.x, secondTriangle[9]);    // Normal
            Assert.AreEqual(normal1.y, secondTriangle[10]);
            Assert.AreEqual(normal1.z, secondTriangle[11]);
            Assert.AreEqual(planeD1, secondTriangle[12]);     // Plane D
            Assert.AreEqual(area1, secondTriangle[13]);       // Area
        }
    }
}
