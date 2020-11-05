using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{
    public struct RTRay
    {
        Vector3 color;
        Vector3 origin;
        Vector3 direction;
        float tmin;
        float tmax;
        uint type;
        float weight;
        uint gen;
    
        public static int Stride
        {
            get {
                return sizeof(float) * (3 + 3 + 3 + 1 + 1 + 1) + sizeof(uint) * (1 + 1);
            }
        }
    }
}

