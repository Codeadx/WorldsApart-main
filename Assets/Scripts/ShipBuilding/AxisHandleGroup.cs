using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisHandleGroup : MonoBehaviour
{
    public AxisHandle[] AxisHandles;
    public void AssignControlPoint(ControlPoint controlPoint) {
        for(int i = 0; i < AxisHandles.Length; i ++) {
            AxisHandles[i].CurrentControlPoint = controlPoint;
        }
    }
}
