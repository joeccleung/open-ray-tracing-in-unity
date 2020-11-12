using System.Collections;
using System.Collections.Generic;
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

            RTBoundingBox box = RTBoundingBox.RTBoundingBoxFromTriangle(primitiveCounter: 0,
                                                                        vertices[0],
                                                                        vertices[1],
                                                                        vertices[2]);

            // Act
            builder.AddBoundingBox(box);
            builder.Construct();

            // Assert
            Assert.AreEqual(new Vector3(0, 0, 0), builder.Root.bounding.min);
            Assert.AreEqual(new Vector3(5, 5, 0), builder.Root.bounding.max);
            Assert.AreEqual(0, builder.Root.bounding.primitiveBegin);
            Assert.AreEqual(1, builder.Root.bounding.primitiveCount);
            Assert.AreEqual(1, builder.Root.children.Count);
            Assert.IsNull(builder.Root.left);
            Assert.AreEqual(-1, builder.Root.leftID);
            Assert.IsNull(builder.Root.right);
            Assert.AreEqual(-1, builder.Root.rightID);

        }
        }
    }
}
