using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public class BVHNode {

        public List<RTBoundingBox> boxes = new List<RTBoundingBox>(1);

        public BVHNode left;
        public int leftID; // For referrencing in flatten array
        public BVHNode right;
        public int rightID; // For referrencing in flatten array

        public BVHNode(RTBoundingBox box) {
            boxes.Add(box);
        }
    }
}