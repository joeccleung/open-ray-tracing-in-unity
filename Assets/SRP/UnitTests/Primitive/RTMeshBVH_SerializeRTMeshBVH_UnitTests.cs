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
            actuator.Setup(x => x.LocalToWorld(It.IsAny<Vector3>())).Returns<Vector3>(v => v);

            RTMeshBVHController controller = new RTMeshBVHController(actuator: actuator.Object);

            // Act
            List<List<float>> triangles = controller.BuildBVHAndTriangleList(trianglesVertexOrder, vertices);
            RTMeshBVHBuilder.Flatten(triangles, out List<List<float>> flatten, out List<List<float>> reorderedPrimitives, controller.GetRoot());
            List<float> serialized = RTMeshBVHController.SerializeRTMeshBVH(flatten, reorderedPrimitives);

            // Assert
            Assert.AreEqual(1, flatten.Count);  // There is only 1 bounding box
            var box = flatten[0];
            Assert.AreEqual(10, box.Count); // 10 fields per box
            Assert.AreEqual(-1, box[0]); // No child node on the left
            Assert.AreEqual(-1, box[1]); // No child node on the right
            Assert.AreEqual(5, box[2]); // max.x
            Assert.AreEqual(5, box[3]); // max.y
            Assert.AreEqual(0, box[4]); // max.z
            Assert.AreEqual(0, box[5]); // min.x
            Assert.AreEqual(0, box[6]); // min.y
            Assert.AreEqual(0, box[7]); // min.z
            Assert.AreEqual(0, box[8]); // primitive begin
            Assert.AreEqual(1, box[9]); // primitive count

            Assert.AreEqual(1, reorderedPrimitives.Count);  // There is only 1 triangles
            var triangle = reorderedPrimitives[0];
            Assert.AreEqual(14, triangle.Count);    // There are 14 fields per triangles
            Assert.AreEqual(0, triangle[0]);    // First vertex
            Assert.AreEqual(0, triangle[1]);    // First vertex
            Assert.AreEqual(0, triangle[2]);    // First vertex
            Assert.AreEqual(0, triangle[3]);    // Second vertex
            Assert.AreEqual(5, triangle[4]);    // Second vertex
            Assert.AreEqual(0, triangle[5]);    // Second vertex
            Assert.AreEqual(5, triangle[6]);    // Third vertex
            Assert.AreEqual(0, triangle[7]);    // Third vertex
            Assert.AreEqual(0, triangle[8]);    // Third vertex

            Assert.AreEqual(1 + 1 + 10 + 14, serialized.Count);   // Size of the BVH + Size of the primitive list + BVH + Primitive List
            Assert.AreEqual(10, serialized[0]); // #1 = the size of the BVH
            Assert.AreEqual(14, serialized[1]); // #2 = the size of the primitive list
        }
    }
}
