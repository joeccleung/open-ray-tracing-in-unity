using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace OpenRT {
    public class BVHNode {

        public RTBoundingBox bounding;

        public BVHNode left;
        public int leftID = -1; // For referrencing in flatten array
        public BVHNode right;
        public int rightID = -1; // For referrencing in flatten array
        public List<RTBoundingBox> children;

        public static BVHNode CombineAllBoxesAndPrimitives(List<RTBoundingBox> boxes) {
            BVHNode node = new BVHNode();
            node.bounding = CombineAllBox(boxes);
            node.children = boxes;

            return node;
        }

        private static RTBoundingBox Combine2Box(RTBoundingBox a, RTBoundingBox b) {
            var max = Vector3.Max(a.max, b.max);
            var min = Vector3.Min(a.min, b.min);

            return new RTBoundingBox(-1, -1,
                max,
                min,
                Mathf.Min(a.primitiveBegin, b.primitiveBegin),
                a.primitiveCount + b.primitiveCount);
        }

        private static RTBoundingBox CombineAllBox(List<RTBoundingBox> boxes) {
            RTBoundingBox root = new RTBoundingBox();
            foreach (var box in boxes) {
                root = Combine2Box(root, box);
            }
            return root;
        }
    }
}