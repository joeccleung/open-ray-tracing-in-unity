using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn1000Spheres : MonoBehaviour
{
    [SerializeField] private int height = 0;
    [SerializeField] private int width = 0;
    [SerializeField] private GameObject prefab;

    void Start()
    {
        float spacing = 5;

        for(int w = 0; w < width; w++)
        {
            for(int h = 0; h < height; h++)
            {
                Instantiate(prefab, new Vector3(w * spacing, 0, h * spacing), Quaternion.identity);
            }
        }
    }
}
