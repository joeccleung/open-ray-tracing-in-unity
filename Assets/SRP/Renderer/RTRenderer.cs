using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpenRT {
    public class RTRenderer : MonoBehaviour {
        [SerializeField] public RTMaterial material;

        protected RTGeometry m_geom = null;

        /// <summary>
        /// This returns geometry type enum for renderer
        /// </summary>
        /// <returns>geometry type.</returns>
        public RTGeometryType GeometryType {
            get {
                if (m_geom == null) {
                    return RTGeometryType.Nothing;
                }

                return m_geom.GetGeometryType();
            }
        }
    }
}