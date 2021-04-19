using System.Collections;
using System.Collections.Generic;
using OpenRT;
using UnityEngine;

public class SwitchMaterialDemo : MonoBehaviour
{
    [SerializeField] private RTRenderer m_renderer;
    [SerializeField] private RTMaterial m_phongMat;
    [SerializeField] private RTMaterial m_translucentMat;

    public void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            m_renderer.material = m_phongMat;
        }

        if (Input.GetKey(KeyCode.W))
        {
            m_renderer.material = m_translucentMat;
        }

        transform.localRotation *= Quaternion.Euler(1, 0, 0);
    }
}
