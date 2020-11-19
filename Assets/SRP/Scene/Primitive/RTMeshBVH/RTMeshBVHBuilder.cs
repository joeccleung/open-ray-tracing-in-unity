﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{

    /// <summary>
    /// Tree building algorithm from 
    /// Reference: https://github.com/CRCS-Graphics/2020.4.Kaihua.Hu.RealTime-RayTracer
    /// 
    /// </summary>
    /// 
    public class RTMeshBVHBuilder
    {
        public const int HARD_LIMIT_MAX_DEPTH = 31;
        public const int MIN_NUMBER_OF_GEO_IN_BOX = 0;

        private List<RTBoundingBox> m_boxes = new List<RTBoundingBox>();
        private BVHNode m_root;

        public void AddBoundingBox(RTBoundingBox box)
        {
            m_boxes.Add(box);
        }

        // Debug
        public List<RTBoundingBox> AllBoundingBoxes
        {
            get
            {
                return m_boxes;
            }
        }

        private BVHNode Build(List<RTBoundingBox> boxes, in int depth)
        {
            BVHNode rootNode = BVHNode.CombineAllBoxesAndPrimitives(boxes);
            char rootBoxLongestAxis = rootNode.bounding.longestAxis;

            if (boxes.Count <= 1)
            {
                return rootNode;
            }

            List<RTBoundingBox> left = new List<RTBoundingBox>();
            List<RTBoundingBox> right = new List<RTBoundingBox>();

            Bisect(boxes, rootBoxLongestAxis, left, right);

            if (left.Count <= MIN_NUMBER_OF_GEO_IN_BOX
                || right.Count <= MIN_NUMBER_OF_GEO_IN_BOX
                || depth == HARD_LIMIT_MAX_DEPTH)
            {
                // Can no longer bisect the bounding box, group them into one

            }
            else
            {
                // Continue bisect
                rootNode.left = Build(left, depth + 1);
                rootNode.right = Build(right, depth + 1);
            }

            return rootNode;
        }

        private void Bisect(List<RTBoundingBox> boxes, char rootBoxLongestAxis, List<RTBoundingBox> left, List<RTBoundingBox> right)
        {
            var cutting = CuttingPlane(boxes);

            foreach (RTBoundingBox box in boxes)
            {
                switch (rootBoxLongestAxis)
                {
                    case 'x':
                        if (cutting.x < box.center.x)
                        {
                            right.Add(box);
                        }
                        else
                        {
                            left.Add(box);
                        }
                        break;

                    case 'y':
                        if (cutting.y < box.center.y)
                        {
                            right.Add(box);
                        }
                        else
                        {
                            left.Add(box);
                        }
                        break;

                    case 'z':
                        if (cutting.z < box.center.z)
                        {
                            right.Add(box);
                        }
                        else
                        {
                            left.Add(box);
                        }
                        break;
                }
            }
        }

        public void Construct()
        {
            m_root = Build(m_boxes, 0);
        }

        public void Clear()
        {
            m_boxes.Clear();
        }

        private Vector3 CuttingPlane(List<RTBoundingBox> boxes)
        {
            Vector3 cp = new Vector3(0f, 0f, 0f);
            float div = 1.0f / boxes.Count;
            foreach (var box in boxes)
            {
                cp = cp + (box.center * div);
            }
            return cp;
        }

        public static void Flatten(in List<List<float>> sceneTriangles,
            out List<List<float>> flatten,
            out List<List<float>> reorderedPrimitives, in BVHNode root)
        {

            int id = 0;

            flatten = new List<List<float>>();
            reorderedPrimitives = new List<List<float>>();

            Queue<BVHNode> bfs = new Queue<BVHNode>();

            bfs.Enqueue(root);
            while (bfs.Count > 0)
            {
                var node = bfs.Dequeue();

                if (node.left != null)
                {
                    id++;
                    node.leftID = id;
                    bfs.Enqueue(node.left);
                }
                if (node.right != null)
                {
                    id++;
                    node.rightID = id;
                    bfs.Enqueue(node.right);
                }

                RTBoundingBox box = node.bounding.SetLeftRight(
                    left: node.leftID,
                    right: node.rightID
                );

                if (node.leftID == -1 && node.rightID == -1)
                {
                    // Leaf Node
                    box.primitiveBegin = reorderedPrimitives.Count;
                    box.primitiveCount = node.children.Count;
                    foreach (var child in node.children)
                    {
                        reorderedPrimitives.Add(sceneTriangles[child.primitiveBegin]);
                    }
                }

                flatten.Add(box.Serialize()); //TODO: Support overlapping bounding box and include all their primitives IDs
            }
        }

        public BVHNode Root
        {
            get
            {
                return m_root;
            }
        }
    }
}

