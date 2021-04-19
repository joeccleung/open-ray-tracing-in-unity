using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public struct RTLightInfo {
        Vector3 position;
        Vector3 rotation;
        int type;


        public RTLightInfo(Vector3 position, int type)
        {
            this.position = position;
            this.rotation = Vector3.zero;
            this.type = type;
        }

        public RTLightInfo(Vector3 position, Vector3 rotation, int type) {
            this.position = position;
            this.rotation = rotation;
            this.type = type;
        }

        public static int Stride {
            get {
                return sizeof(int) * 1 + sizeof(float) * 3 * 2;
            }
        }
    }
}