using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinController : MonoBehaviour {
   [SerializeField] private SinMat m_material;
   
   void Update() {
      m_material.sin = Mathf.Abs(Mathf.Sin(Time.time % 360 * 9 * Mathf.Deg2Rad));
   }
}