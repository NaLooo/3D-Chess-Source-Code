using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }
}
