using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{
public class RTLight : MonoBehaviour {
   [HideInInspector, SerializeField] public int lightIndex = 0;
   [HideInInspector, SerializeField] public string shaderGUID = string.Empty;

   public void OnDrawGizmos() {
      Gizmos.color = Color.yellow;
      Gizmos.DrawLine(transform.position, transform.position + transform.forward);
   }

   public virtual RTLightInfo GetLightInfo()
   {
       return new RTLightInfo(
           position: transform.position,
           rotation: Vector3.Normalize(-1 * transform.up),     // We assume light points downward
           type: lightIndex
       );
   }

   public virtual bool IsDirty()
   {
       return true;
   }
}
}
