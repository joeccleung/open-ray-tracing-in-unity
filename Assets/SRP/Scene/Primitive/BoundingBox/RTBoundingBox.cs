using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    /// <summary>
    /// Reference: https://github.com/CRCS-Graphics/2020.4.Kaihua.Hu.RealTime-RayTracer
    /// 
    /// </summary>
    public struct RTBoundingBox {

        public const int stride = 2 * 4 * 3 + 4 * 4; // 2 Vector3 + 4 int

        public int leftID; // For referrencing in flatten array
        public int rightID; // For referrencing in flatten array
        public Vector3 max;
        public Vector3 min;
        public int primitiveBegin;
        public int primitiveCount;

        public static RTBoundingBox Empty {
            get {
                return new RTBoundingBox(
                    leftID: -1,
                    rightID: -1,
                    max : new Vector3(float.MinValue, float.MinValue, float.MinValue),
                    min : new Vector3(float.MaxValue, float.MaxValue, float.MaxValue),
                    primitiveBegin : 0,
                    primitiveCount : 0
                );
            }
        }

        public RTBoundingBox(int leftID, int rightID, Vector3 max, Vector3 min, int primitiveBegin, int primitiveCount) {
            this.leftID = leftID;
            this.rightID = rightID;
            this.max = max;
            this.min = min;
            this.primitiveBegin = primitiveBegin;
            this.primitiveCount = primitiveCount;
        }

        public RTBoundingBox(
            Vector3 max,
            Vector3 min,
            int primitive) {

            this.leftID = -1;
            this.rightID = -1;
            this.max = max;
            this.min = min;
            this.primitiveBegin = primitive;
            this.primitiveCount = 1;
        }

        public Vector3 center {
            get {
                return (max + min) / 2f;
            }
        }

        public char longestAxis {
            get {
                char res;

                if (size.x >= size.y) {
                    res = 'x';
                    if (size.x < size.z) {
                        res = 'z';
                    }
                } else {
                    res = 'y';
                    if (size.y < size.z) {
                        res = 'z';
                    }
                }

                return res;
            }
        }

        public RTBoundingBox SetLeftRight(int left, int right) {
            return new RTBoundingBox(
                leftID: left,
                rightID: right,
                max: this.max,
                min: this.min,
                primitiveBegin: this.primitiveBegin,
                primitiveCount: this.primitiveCount
            );
        }

        public Vector3 size {
            get {
                return (max - min);
            }
        }
    }

}