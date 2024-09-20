using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisHandleSingle : MonoBehaviour
{
    public AxisHandle[] AxisHandle;
    public void AssignControlPoint(ControlPoint controlPoint) {
        AxisHandle[0].CurrentControlPoint = controlPoint;
    }
}
