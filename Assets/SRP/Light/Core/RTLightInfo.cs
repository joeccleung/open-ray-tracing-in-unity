using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    public struct RTLightInfo {
        int instance;
        Vector3 position;
        Vector3 rotation;
        int type;


        public RTLightInfo(Vector3 position, int type)
        {
            this.instance = -1;
            this.position = position;
            this.rotation = Vector3.zero;
            this.type = -1;
        }

        public RTLightInfo(int instance, Vector3 position, Vector3 rotation, int type) {
            this.instance = instance;
            this.position = position;
            this.rotation = rotation;
            this.type = type;
        }

        public static int Stride {
            get {
                return sizeof(int) * 2 + sizeof(float) * 3 * 2;
            }
        }
    }
}