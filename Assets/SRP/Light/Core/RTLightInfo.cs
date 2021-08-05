using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{

    /// <summary>
    /// RTLightInfo is eqivalent to primitive in representing a game object in the scene
    /// It contains the basic transformation info and the light type
    /// The properties relevant to each light type is stored under each light
    /// /// </summary>
    public struct RTLightInfo
    {
        Vector3 position;
        Vector3 rotation;
        int instanceIndex;
        int type;

        public RTLightInfo(Vector3 position, Vector3 rotation, int instanceIndex, int type)
        {
            this.position = position;
            this.rotation = rotation;
            this.instanceIndex = instanceIndex;
            this.type = type;
        }

        public static int Stride
        {
            get
            {
                // 2 integer fields
                // 2 Vector3 fields
                return sizeof(int) * 2 + sizeof(float) * 3 * 2;
            }
        }
    }
}