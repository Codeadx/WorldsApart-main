using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameTest : MonoBehaviour
{

    void Start() {
    }

    public Vector3[] Corners = new Vector3[8]
    {        
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 1, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1),
        new Vector3(1, 1, 1),
        new Vector3(0, 1, 1)
    };
}
