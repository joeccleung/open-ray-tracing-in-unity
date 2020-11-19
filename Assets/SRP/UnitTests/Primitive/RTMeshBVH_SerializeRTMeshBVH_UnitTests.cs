using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Moq;

namespace OpenRT.UnitTests.Primitive
{
    public class RTMeshBVH_SerializeRTMeshBVH_UnitTests
    {
        [Test]
        public void Serialize_OneTriangle_BVH()
        {
            // Assign
            Vector3[] vertices = new Vector3[3];
            vertices[0] = new Vector3(0, 0, 0);
            vertices[1] = new Vector3(0, 5, 0);
            vertices[2] = new Vector3(5, 0, 0);

            int[] trianglesVertexOrder = new int[3];
            trianglesVertexOrder[0] = 0;
            trianglesVertexOrder[1] = 1;
            trianglesVertexOrder[2] = 2;

            var actuator = new Mock<RTMeshBVHController.IActuator>();
            actuator.Setup(x => x.GetVertices()).Returns(vertices);

            RTMeshBVHController controller = new RTMeshBVHController(actuator: actuator.Object);

            // Act
            List<List<float>> triangles = controller.BuildBVHAndTriangleList(trianglesVertexOrder, vertices);

            // Assert
            Assert.AreEqual(1, triangles.Count);

        }
    }
}
