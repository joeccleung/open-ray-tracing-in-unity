using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{
    /// <summary>
    /// RTLight is eqivalent to material in representing a game object render properties in the scene
    /// It holds the light shader GUID, similar to material holds the closest hit shader GUID
    /// Each light type should sub-class RTLight
    /// </summary>
    public class RTLight : MonoBehaviour
    {
        [HideInInspector, SerializeField] public int lightIndex = 0;
        [HideInInspector, SerializeField] public string shaderGUID = string.Empty;

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }


        public virtual Vector3 Position()
        {
            return transform.position;
        }

        public virtual Vector3 Rotation()
        {
            return Vector3.Normalize(-1 * transform.up);     // We assume light points downward
        }

        public virtual bool IsDirty()
        {
            return true;
        }
    }
}
