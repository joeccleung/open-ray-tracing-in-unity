using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using OpenRT;

public class Test : MonoBehaviour
{
   [SerializeField] RTMaterial material;

   void Start()
   {
      Debug.Assert(material != null, "Material is null");
      CommandBuffer cb = new CommandBuffer();
      PipelineMaterialToBuffer.MaterialToBuffer(material, ref cb);
   }
}
