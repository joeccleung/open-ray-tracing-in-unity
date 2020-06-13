using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT {
    [RequireComponent(typeof(RTMesh))]
    public class RTMeshRenderer : RTRenderer {
        public RTMesh myMesh {
            get {
                if (m_geom == null) {
                    m_geom = GetComponent<RTMesh>();
                }

                return (RTMesh) m_geom;
            }
        }

        public void GetGeometry(out List<RTTriangle_t> tris, out RTMaterial mat) {

            tris = null;
            mat = null;

            if (myMesh == null) {
                return;
            }

            // FIXME: Here we cut corner to assign shader index as material index. They are not the same. 
            // We do this just to prototype custom shader framework
            myMesh.GetGeometry(materialIdx: material.shaderIndex, tris: out tris);
            mat = material;
        }
    }
}