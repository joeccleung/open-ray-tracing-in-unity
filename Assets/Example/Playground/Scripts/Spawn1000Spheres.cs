using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn1000Spheres : MonoBehaviour {
    [SerializeField] private int height = 0;
    [SerializeField] private int width = 0;
    [SerializeField] private bool scattered = false;
    [SerializeField] private GameObject prefab;

    void Start() {
        float spacing = 5;

        for (int w = 0; w < width; w++) {
            for (int h = 0; h < height; h++) {
                Vector3 position = Vector3.zero;
                if (scattered) {
                    position = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
                } else {
                    position = new Vector3(w * spacing, 0, h * spacing);
                }
                Instantiate(prefab, position, Quaternion.identity);
            }
        }
    }
}