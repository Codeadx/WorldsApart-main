using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastToControlPoint : MonoBehaviour
{
    public GameObject axisHandlesPrefab;
    GameObject axisHandlesInstance;
    public GameObject axisHandlesMirrored;
    GameObject axisHandles2ndInstance;
    public Framework selectedFramework;
    public AxisHandle selectedHandle;
    public ControlPoint selectedControlPoint;
    public ControlPoint lastSelectedControlPoint;

    void Awake() {
        axisHandlesInstance = Instantiate(axisHandlesPrefab);
        axisHandlesInstance.SetActive(false);
        // axisHandles2ndInstance = Instantiate(axisHandlesMirrored);
        // axisHandles2ndInstance.SetActive(false);
    }

    void LateUpdate() {
        if(lastSelectedControlPoint != null && Input.GetKeyDown(KeyCode.R)) {
            axisHandlesInstance.SetActive(false);
            if(lastSelectedControlPoint.Index < 16) {
                selectedFramework.ResetAllPoints(lastSelectedControlPoint.Index, selectedFramework.FindInitialPosition(lastSelectedControlPoint.Index));
            } else {
                selectedFramework.MoveSelectedAndAssociated(lastSelectedControlPoint.Index, lastSelectedControlPoint.Axis, selectedFramework.FindInitialPosition(lastSelectedControlPoint.Index));
            }
                
            for(int i = 0; i < selectedFramework.newRPos.Length; i++) {
                selectedFramework.newRPos[i] = Vector3.zero;
            }
            selectedHandle = null;
            selectedControlPoint = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Input.GetMouseButton(0)) {
            if(Physics.Raycast(ray, out hit)) {
                if(selectedControlPoint == null) { 
                    if(hit.transform.TryGetComponent(out ControlPoint controlPoint)) {
                        switch (controlPoint.PointType) {
                            case PointType.TransformPoint:
                                selectedFramework = hit.transform.GetComponentInParent<Framework>();
                                selectedControlPoint = controlPoint;
                                selectedControlPoint.AssignAxisHandles(axisHandlesInstance);
                                axisHandlesInstance.SetActive(true);
                            break;

                            case PointType.RoundingPoint:
                                selectedFramework = hit.transform.GetComponentInParent<Framework>();
                                selectedControlPoint = controlPoint;
                                selectedControlPoint.AssignControlPoint(hit.transform.gameObject);
                                axisHandlesInstance.SetActive(false);
                            break;
                        }
                    }
                } else if (selectedControlPoint != null && selectedHandle == null) {
                    if(hit.transform.TryGetComponent(out AxisHandle handle) && hit.transform.CompareTag("TransformHandle")) {
                        selectedHandle = handle;
                        handle.CurrentControlPoint.Axis = handle.Axis;
                    } else if(hit.transform.CompareTag("RoundingPoint")) {
                        if(hit.transform.TryGetComponent(out ControlPoint point)) {
                            switch (point.PointType) {
                                case PointType.RoundingPoint:
                                    selectedControlPoint = point;
                                    selectedControlPoint.AssignControlPoint(hit.transform.gameObject);
                                    axisHandlesInstance.SetActive(false);
                                    selectedHandle = handle;
                                    handle.CurrentControlPoint.Axis = handle.Axis;
                                break;
                            }
                        }
                    } else if(hit.transform.CompareTag("TransformPoint")) {
                        if(hit.transform.TryGetComponent(out ControlPoint point)) {
                            switch (point.PointType) {
                                case PointType.TransformPoint:
                                    selectedControlPoint = point;
                                    selectedControlPoint.AssignAxisHandles(axisHandlesInstance);
                                    axisHandlesInstance.SetActive(true);
                                break;
                            }
                        }
                    }
                }
            }
        } else if (Input.GetMouseButtonUp(0) && selectedHandle != null && selectedControlPoint != null) { lastSelectedControlPoint = selectedControlPoint; selectedHandle = null; }
        //Clamp movement betwen 2 values
        Vector3 planeDistFromCamera = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, axisHandlesInstance.transform.position.z);
        Plane myPlane = new Plane(Vector3.forward, planeDistFromCamera);
        float distance = 0;
        if(myPlane.Raycast(ray, out distance)) {
            Vector3 hitPoint = ray.GetPoint(distance);
            if(selectedHandle != null) {
                //Move the points on specified Axis
                switch(selectedHandle.Axis) {
                    case Axis.X:
                    float minX, maxX, minY, maxY, minZ, maxZ;
                        minX = selectedFramework.FindInitialPosition(selectedHandle.CurrentControlPoint.Index).x - 0.45f;    maxX = selectedFramework.FindInitialPosition(selectedHandle.CurrentControlPoint.Index).x + 0.45f;
                        Vector3 delta = new Vector3(Mathf.Clamp(hitPoint.x + 0.15f, minX, maxX), axisHandlesInstance.transform.position.y, axisHandlesInstance.transform.position.z);
                                if (Input.GetKey(KeyCode.LeftShift)) {
            // Round the delta to 1 decimal place
            delta.x = Mathf.Round(delta.x * 10f) / 10f;
            delta.y = Mathf.Round(delta.y * 10f) / 10f;
            delta.z = Mathf.Round(delta.z * 10f) / 10f;
        }
                        axisHandlesInstance.transform.position = delta;
                        selectedFramework.MoveSelectedAndAssociated(selectedHandle.CurrentControlPoint.Index, selectedHandle.CurrentControlPoint.Axis, hitPoint + new Vector3(0.15f, 0.15f, 0.15f));
                    break;

                    case Axis.Y:
                        minY = selectedFramework.FindInitialPosition(selectedHandle.CurrentControlPoint.Index).y - 0.45f;    maxY = selectedFramework.FindInitialPosition(selectedHandle.CurrentControlPoint.Index).y + 0.45f;
                        delta = new Vector3(axisHandlesInstance.transform.position.x, Mathf.Clamp(hitPoint.y - 0.15f, minY, maxY), axisHandlesInstance.transform.position.z);
                                if (Input.GetKey(KeyCode.LeftShift)) {
            // Round the delta to 1 decimal place
            delta.x = Mathf.Round(delta.x * 10f) / 10f;
            delta.y = Mathf.Round(delta.y * 10f) / 10f;
            delta.z = Mathf.Round(delta.z * 10f) / 10f;
        }
                        axisHandlesInstance.transform.position = delta;
                        selectedFramework.MoveSelectedAndAssociated(selectedHandle.CurrentControlPoint.Index, selectedHandle.CurrentControlPoint.Axis, hitPoint + new Vector3(0.15f, -0.15f, 0.15f));
                    break;
                    //New Raycast on Z plane and clamp movement
                    case Axis.Z:
                        planeDistFromCamera = new Vector3(axisHandlesInstance.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
                        Plane zPlane = new Plane(Vector3.right, planeDistFromCamera);
                        distance = 0;
                        if(zPlane.Raycast(ray, out distance)) { hitPoint = ray.GetPoint(distance); }
                        minZ = selectedFramework.FindInitialPosition(selectedHandle.CurrentControlPoint.Index).z - 0.45f;    maxZ = selectedFramework.FindInitialPosition(selectedHandle.CurrentControlPoint.Index).z + 0.45f;
                        delta = new Vector3(axisHandlesInstance.transform.position.x, axisHandlesInstance.transform.position.y, Mathf.Clamp(hitPoint.z + 0.15f, minZ, maxZ));
                                if (Input.GetKey(KeyCode.LeftShift)) {
            // Round the delta to 1 decimal place
            delta.x = Mathf.Round(delta.x * 10f) / 10f;
            delta.y = Mathf.Round(delta.y * 10f) / 10f;
            delta.z = Mathf.Round(delta.z * 10f) / 10f;
        }
                        axisHandlesInstance.transform.position = delta;
                        selectedFramework.MoveSelectedAndAssociated(selectedHandle.CurrentControlPoint.Index, selectedHandle.CurrentControlPoint.Axis, hitPoint + new Vector3(0.15f, 0.15f, 0.15f));
                    break;
                }
            }
        }
    }
}
