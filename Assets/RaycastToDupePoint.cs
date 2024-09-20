using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastToDupePoint : MonoBehaviour
{
    
    void Update() {
        HandleRaycast();
    }

    void HandleRaycast() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Input.GetMouseButtonDown(0)) {
            if(Physics.Raycast(ray, out hit)) {
                if(hit.transform.TryGetComponent(out HorizontalDuplicationPoint dupePoint)) {
                    for(int i = 0; i < dupePoint.AssociatedPoints.Length; i++) {
                        dupePoint.AssociatedPoints[i].gameObject.SetActive(true);
                        hit.transform.gameObject.GetComponent<MeshRenderer>().enabled = false;
                        if(dupePoint.AttachedPoint.TryGetComponent(out FinalDuplicationPoint aPoint)) {
                            for(int j = 0; j < aPoint.AssociatedPoints.Length; j++) {
                                aPoint.AssociatedPoints[j].gameObject.SetActive(true);
                            } for(int k = 0; k < aPoint.AllColliders.Length; k++) {
                                aPoint.AllColliders[k].enabled = true;
                                aPoint.AllColliders[k].GetComponent<MeshRenderer>().enabled = true;
                            }
                        }
                    }
                } else if (hit.transform.TryGetComponent(out VerticalDuplicationPoint dPoint)) {
                    for(int i = 0; i < dPoint.AssociatedPoints.Length; i++) {
                        dPoint.AssociatedPoints[i].gameObject.SetActive(true);
                        hit.transform.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    }
                } else if (hit.transform.TryGetComponent(out FinalDuplicationPoint dPoint2)) {
                    for(int i = 0; i < dPoint2.AssociatedPoints.Length; i++) {
                        dPoint2.AssociatedPoints[i].gameObject.SetActive(true);
                        hit.transform.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    }
                }
            }
        }
    }
}
