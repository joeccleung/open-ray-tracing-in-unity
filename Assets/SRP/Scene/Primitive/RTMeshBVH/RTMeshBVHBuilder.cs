using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public const int HARD_LIMIT_MAX_DEPTH = 16;

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

        private BVHNode Build(List<RTBoundingBox> boxes,
                              in int depth,
                              in int minNumberOfGeoPerBox)
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

            if (left.Count == 0 || right.Count == 0)
            {
                // Bisect fail (e.g. all children aligned on the division plane), end the tree building
                return rootNode;
            }
            else
            {
                if (left.Count + right.Count > minNumberOfGeoPerBox
                    && left.Count > 0
                    && right.Count > 0
                    && depth < HARD_LIMIT_MAX_DEPTH)
                {
                    // Continue bisect
                    rootNode.left = Build(left, depth + 1, minNumberOfGeoPerBox);
                    rootNode.right = Build(right, depth + 1, minNumberOfGeoPerBox);
                }
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

        public void Construct(int minNumberOfGeoPerBox)
        {
            m_root = Build(m_boxes, 0, minNumberOfGeoPerBox);
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

        public static void Flatten(ref List<List<float>> flatten,
                                   int geoLocalToGlobalIndexOffset,
                                   int mappingLocalToGlobalIndexOffset,
                                   ref List<List<int>> accelerationGeometryMappingCollection,
                                   in BVHNode root)
        {
            int id = 0;

            flatten = new List<List<float>>();
            accelerationGeometryMappingCollection = new List<List<int>>();

            Queue<BVHNode> bfs = new Queue<BVHNode>();

            bfs.Enqueue(root);

            int mappingLocalIndexOffset = 0;

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

                if (box.leftID == -1 && box.rightID == -1)
                {
                    // Leaf Node
                    // Left child index will be -1 to indicate leaf node
                    // Right child index will be pointing to the Acceleration Structure - Geometry mapping index

                    // Firstly, we generate the mapping
                    var geoIndicesIter = box.geoIndices.GetEnumerator();

                    List<int> accelerationGeometryMapping = new List<int>();
                    while (geoIndicesIter.MoveNext())
                    {
                        accelerationGeometryMapping.Add(geoLocalToGlobalIndexOffset + geoIndicesIter.Current * RTMeshBVH.TRIANGLE_STRIDE);
                    }
                    accelerationGeometryMapping.Insert(0, accelerationGeometryMapping.Count);

                    accelerationGeometryMappingCollection.Add(accelerationGeometryMapping);

                    // Secondly, we assign the index to the mapping collection of the same geometry to the right child index
                    box.rightID = mappingLocalToGlobalIndexOffset + mappingLocalIndexOffset;
                    mappingLocalIndexOffset += accelerationGeometryMapping.Count;
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