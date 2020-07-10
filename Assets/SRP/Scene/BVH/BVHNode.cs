using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public class BVHNode {

        public List<RTBoundingBox> boxes = new List<RTBoundingBox>(1);

        public BVHNode left;
        public BVHNode right;

        public BVHNode(RTBoundingBox box)
        {
            boxes.Add(box);
        }
    }
}