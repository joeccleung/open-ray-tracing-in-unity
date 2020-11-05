using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRT
{
public class RTLight : MonoBehaviour {
   [HideInInspector, SerializeField] public int shaderIndex = 0;
   [HideInInspector, SerializeField] public string shaderGUID = string.Empty;

   private void OnDrawGizmosSelected() {
      Gizmos.color = Color.yellow;
      Gizmos.DrawLine(transform.position, transform.position + transform.forward);
   }

   public virtual RTLightInfo GetLightInfo()
   {
       return new RTLightInfo(
           position: transform.position,
           type: shaderIndex
       );
   }
}
}
