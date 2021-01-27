using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace OpenRT.UnitTests.Primitive
{
    public class RTMeshBVHBuilder_Triangle_UnitTests
    {
        [Test]
        public void One_Triangle()
        {
            // Assign
            RTMeshBVHBuilder builder = new RTMeshBVHBuilder();
            Vector3[] vertices = new Vector3[3];
            vertices[0] = new Vector3(0, 0, 0);
            vertices[1] = new Vector3(0, 5, 0);
            vertices[2] = new Vector3(5, 0, 0);

            RTBoundingBox box = RTBoundingBox.RTBoundingBoxFromTriangle(triangleIndex: 0,
                                                                        vertices[0],
                                                                        vertices[1],
                                                                        vertices[2]);

            // Act
            builder.AddBoundingBox(box);
            builder.Construct(0);

            // Assert
            Assert.AreEqual(new Vector3(0, 0, 0), builder.Root.bounding.min);
            Assert.AreEqual(new Vector3(5, 5, 0), builder.Root.bounding.max);
            Assert.AreEqual(1, builder.Root.bounding.geoIndices.ToList().Count);
            Assert.AreEqual(1, builder.Root.children.Count);
            Assert.IsNull(builder.Root.left);
            Assert.AreEqual(-1, builder.Root.leftID);
            Assert.IsNull(builder.Root.right);
            Assert.AreEqual(-1, builder.Root.rightID);

        }

        [Test]
        public void Two_Triangle()
        {
            // Assign
            RTMeshBVHBuilder builder = new RTMeshBVHBuilder();
            Vector3[] triangle1 = new Vector3[3];
            triangle1[0] = new Vector3(0, 0, 0);
            triangle1[1] = new Vector3(0, 5, 0);
            triangle1[2] = new Vector3(5, 0, 0);

            RTBoundingBox box1 = RTBoundingBox.RTBoundingBoxFromTriangle(triangleIndex: 0,
                                                                         triangle1[0],
                                                                         triangle1[1],
                                                                         triangle1[2]);

            Vector3[] triangle2 = new Vector3[3];
            triangle2[0] = new Vector3(10, 0, 0);
            triangle2[1] = new Vector3(10, 5, 0);
            triangle2[2] = new Vector3(15, 0, 0);

            RTBoundingBox box2 = RTBoundingBox.RTBoundingBoxFromTriangle(triangleIndex: 1,
                                                                         triangle2[0],
                                                                         triangle2[1],
                                                                         triangle2[2]);

            // Act
            builder.AddBoundingBox(box1);
            builder.AddBoundingBox(box2);

            builder.Construct(0);

            // Assert
            Assert.AreEqual(new Vector3(0, 0, 0), builder.Root.bounding.min);
            Assert.AreEqual(new Vector3(15, 5, 0), builder.Root.bounding.max);
            Assert.AreEqual(1, builder.Root.bounding.geoIndices.ToList().Count);
            Assert.AreEqual(2, builder.Root.children.Count);

            Assert.IsNotNull(builder.Root.left);
            var leftNode = builder.Root.left;
            Assert.AreEqual(new Vector3(0, 0, 0), leftNode.bounding.min);
            Assert.AreEqual(new Vector3(5, 5, 0), leftNode.bounding.max);
            Assert.AreEqual(1, leftNode.bounding.geoIndices.ToList().Count);
            Assert.AreEqual(1, leftNode.children.Count);

            Assert.IsNotNull(builder.Root.right);
            var rightNode = builder.Root.right;
            Assert.AreEqual(new Vector3(10, 0, 0), rightNode.bounding.min);
            Assert.AreEqual(new Vector3(15, 5, 0), rightNode.bounding.max);
            Assert.AreEqual(1, rightNode.bounding.geoIndices.ToList().Count);
            Assert.AreEqual(1, rightNode.children.Count);

        }
    }
}
