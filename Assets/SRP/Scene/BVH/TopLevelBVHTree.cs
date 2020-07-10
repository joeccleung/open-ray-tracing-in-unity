using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public class TopLevelBVH {
        private List<RTBoundingBox> m_boxes = new List<RTBoundingBox>();
        private BVHNode m_root;

        public void AddBoundingBox(RTBoundingBox box) {
            m_boxes.Add(box);
        }

        // Debug
        public List<RTBoundingBox> AllBoundingBoxes {
            get {
                return m_boxes;
            }
        }

        private BVHNode Build(List<RTBoundingBox> boxes) {

            RTBoundingBox rootBox = CombineAllBox(boxes);
            char rootBoxLongestAxis = rootBox.longestAxis;
            BVHNode rootNode = new BVHNode(rootBox);

            if (boxes.Count <= 1) {
                return rootNode;
            }

            List<RTBoundingBox> left = new List<RTBoundingBox>();
            List<RTBoundingBox> right = new List<RTBoundingBox>();

            var cutting = CuttingPlane(boxes);

            foreach (RTBoundingBox box in boxes) {
                switch (rootBoxLongestAxis) {
                    case 'x':
                        if (cutting.x < box.center.x) {
                            right.Add(box);
                        } else {
                            left.Add(box);
                        }
                        break;

                    case 'y':
                        if (cutting.y < box.center.y) {
                            right.Add(box);
                        } else {
                            left.Add(box);
                        }
                        break;

                    case 'z':
                        if (cutting.z < box.center.z) {
                            right.Add(box);
                        } else {
                            left.Add(box);
                        }
                        break;
                }
            }

            if (left.Count == 0 || right.Count == 0) {
                // Can no longer bisect the bounding box, group them into one
                rootNode.boxes.AddRange(left);
                rootNode.boxes.AddRange(right);
            } else {
                // Continue bisect
                rootNode.left = Build(left);
                rootNode.right = Build(right);
            }

            return rootNode;
        }

        public void Construct() {
            m_root = Build(m_boxes);
        }

        public void Clear() {
            m_boxes.Clear();
        }

        private RTBoundingBox Combine2Box(RTBoundingBox a, RTBoundingBox b) {
            var max = Vector3.Max(a.max, b.max);
            var min = Vector3.Min(a.min, b.min);
            var primitiveBegin = Mathf.Min(a.primitiveBegin, b.primitiveBegin);
            var primitiveEnd = Mathf.Max(a.primitiveEnd, b.primitiveEnd);

            return new RTBoundingBox(max, min, primitiveBegin, primitiveEnd);
        }

        private RTBoundingBox CombineAllBox(List<RTBoundingBox> boxes) {
            RTBoundingBox root = boxes[0];
            foreach (var box in boxes) {
                root = Combine2Box(root, box);
            }
            return root;
        }

        private Vector3 CuttingPlane(List<RTBoundingBox> boxes) {
            Vector3 cp = new Vector3(0f, 0f, 0f);
            float div = 1.0f / boxes.Count;
            foreach (var box in boxes) {
                cp = cp + (box.center * div);
            }
            return cp;
        }

        public BVHNode Root {
            get {
                return m_root;
            }
        }
    }
}