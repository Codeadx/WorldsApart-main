using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    public Axis Axis;
    public int Index;
    public ActiveOnStart ActiveOnStart;
    public PointType PointType;
    
        
    public void AssignAxisHandles(GameObject handles) {
        handles.transform.position = transform.position;
        if(handles.TryGetComponent(out AxisHandleGroup group)) {
            group.AssignControlPoint(this);
        }
    }

    public void AssignControlPoint(GameObject handles) {
        if(handles.TryGetComponent(out AxisHandle handle)) {
            handle.CurrentControlPoint = this;
        }
    }
}
