using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace OpenRT
{
    public class BVHNode
    {

        public RTBoundingBox bounding;

        public BVHNode left;
        public int leftID = -1; // For referrencing in flatten array
        public BVHNode right;
        public int rightID = -1; // For referrencing in flatten array
        public List<RTBoundingBox> children;

        public static BVHNode CombineAllBoxesAndPrimitives(List<RTBoundingBox> boxes)
        {
            BVHNode node = new BVHNode();
            node.bounding = CombineAllBox(boxes);
            node.children = boxes;

            return node;
        }

        private static RTBoundingBox Combine2Box(RTBoundingBox a, RTBoundingBox b)
        {
            var max = Vector3.Max(a.max, b.max);
            var min = Vector3.Min(a.min, b.min);

            a.geoIndices.UnionWith(b.geoIndices); // The union of triangle indices from both bounding boxes
                                                  // Use UnionWith instead of Linq Union to avoid creating a new IEnumerable

            return new RTBoundingBox(-1,
                                     -1,
                                     max,
                                     min,
                                     a.geoIndices);
        }

        private static RTBoundingBox CombineAllBox(List<RTBoundingBox> boxes)
        {
            RTBoundingBox root = RTBoundingBox.Empty;
            foreach (var box in boxes)
            {
                root = Combine2Box(root, box);
            }
            return root;
        }
    }
}