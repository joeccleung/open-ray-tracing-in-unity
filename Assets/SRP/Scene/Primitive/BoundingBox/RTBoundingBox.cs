﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{
    /// <summary>
    /// Reference: https://github.com/CRCS-Graphics/2020.4.Kaihua.Hu.RealTime-RayTracer
    /// 
    /// </summary>
    public struct RTBoundingBox
    {
        public const int NUMBER_OF_FLOAT = 2 * 3 + 4;
        public const int stride = 2 * 4 * 3 + 4 * 4; // 2 Vector3 + 4 int

        public int leftID; // For referrencing in flatten array
        public int rightID; // For referrencing in flatten array
        public Vector3 max;
        public Vector3 min;
        public int primitiveBegin;
        public int primitiveCount;

        /// <summary>
        /// Initialize the RTBoundingBox as empty content box.
        /// Calling empty constructor does not achieve this result.
        /// </summary>
        /// <value></value>
        public static RTBoundingBox Empty
        {
            get
            {
                return new RTBoundingBox(
                    leftID: -1,
                    rightID: -1,
                    max: new Vector3(float.MinValue, float.MinValue, float.MinValue),
                    min: new Vector3(float.MaxValue, float.MaxValue, float.MaxValue),
                    primitiveBegin: 0,
                    primitiveCount: 0
                );
            }
        }

        public RTBoundingBox(int leftID, int rightID, Vector3 max, Vector3 min, int primitiveBegin, int primitiveCount)
        {
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
            int primitive)
        {

            this.leftID = -1;
            this.rightID = -1;
            this.max = max;
            this.min = min;
            this.primitiveBegin = primitive;
            this.primitiveCount = 1;
        }

        public static void AddVerticesToBox(ref RTBoundingBox boundingBox, Vector3 vertex)
        {
            boundingBox.min = Vector3.Min(boundingBox.min, vertex);
            boundingBox.max = Vector3.Max(boundingBox.max, vertex);
        }

        public Vector3 center
        {
            get
            {
                return (max + min) / 2f;
            }
        }

        public char longestAxis
        {
            get
            {
                char res;

                if (size.x >= size.y)
                {
                    res = 'x';
                    if (size.x < size.z)
                    {
                        res = 'z';
                    }
                }
                else
                {
                    res = 'y';
                    if (size.y < size.z)
                    {
                        res = 'z';
                    }
                }

                return res;
            }
        }

        public List<float> Serialize()
        {
            return new List<float>(){
                leftID,
                rightID,
                max.x,
                max.y,
                max.z,
                min.x,
                min.y,
                min.z,
                primitiveBegin,
                primitiveCount
            };
        }

        public static RTBoundingBox RTBoundingBoxFromTriangle(int primitiveCounter, Vector3 v0, Vector3 v1, Vector3 v2)
        {
            RTBoundingBox box = Empty;
            AddVerticesToBox(ref box, v0);
            AddVerticesToBox(ref box, v1);
            AddVerticesToBox(ref box, v2);
            box.primitiveBegin = primitiveCounter;
            box.primitiveCount = 1;
            return box;
        }

        public RTBoundingBox SetLeftRight(int left, int right)
        {
            return new RTBoundingBox(
                leftID: left,
                rightID: right,
                max: this.max,
                min: this.min,
                primitiveBegin: this.primitiveBegin,
                primitiveCount: this.primitiveCount
            );
        }

        public Vector3 size
        {
            get
            {
                return (max - min);
            }
        }
    }

}