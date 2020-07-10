using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public class BVHVisualizer : MonoBehaviour {
        public void OnDrawGizmos() {

            Queue<BVHNode> bfs = new Queue<BVHNode>();
            Queue<int> depthQ = new Queue<int>();
            bfs.Enqueue(SceneParser.Instance.sceneParseResult.TopLevelBVH.Root);
            depthQ.Enqueue(0);

            while (bfs.Count > 0) {
                var node = bfs.Dequeue();
                var depth = depthQ.Dequeue();
                Gizmos.color = Rainbow(depth);
                Gizmos.DrawWireCube(node.boxes[0].center, node.boxes[0].size);
                if (node.left != null) {
                    bfs.Enqueue(node.left);
                    depthQ.Enqueue(depth + 1);
                }
                if (node.right != null) {
                    bfs.Enqueue(node.right);
                    depthQ.Enqueue(depth + 1);
                }
            }

        }

        private Color Rainbow(int depth) {
            switch (depth % 7) {
                case 0:
                    return Color.red;

                case 1:
                    return new Color(1, 0.5f, 0);

                case 2:
                    return new Color(1, 1, 0);

                case 3:
                    return new Color(0, 1, 0);

                case 4:
                    return new Color(0, 0.5f, 1);

                case 5:
                    return new Color(0, 0, 1);

                case 6:
                    return new Color(0.5f, 0, 1);

                default:
                    return Color.white;
            }
        }
    }
}