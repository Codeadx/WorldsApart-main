using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderPoint : MonoBehaviour
{
    public GameObject TransformPoint1;
    public GameObject TransformPoint2;
    public GameObject RoundingPoint1;
    public GameObject RoundingPoint2;
    public float width = 0.3f;
    public float colliderLength;
    public Orientation orientation;

    void LateUpdate() {
        switch (orientation) {
            case Orientation.Vertical: {
                colliderLength = Vector3.Distance(TransformPoint1.transform.localPosition, RoundingPoint1.transform.localPosition);
                transform.position = (RoundingPoint1.transform.position - TransformPoint1.transform.position) / 2 + TransformPoint1.transform.position;
                transform.rotation = Quaternion.LookRotation(TransformPoint1.transform.position - RoundingPoint1.transform.position);
                transform.localScale = new Vector3(width, width, colliderLength / TransformPoint1.transform.localScale.x);
                break; 
            }
            case Orientation.VerticalRPRP: {
                colliderLength = (RoundingPoint1.transform.localPosition - RoundingPoint2.transform.localPosition).magnitude;
                transform.position = (RoundingPoint2.transform.position - RoundingPoint1.transform.position) / 2 + RoundingPoint1.transform.position;
                transform.rotation = Quaternion.LookRotation(RoundingPoint1.transform.position - RoundingPoint2.transform.position);
                transform.localScale = new Vector3(width, width, colliderLength / RoundingPoint1.transform.localScale.x / 2);
                break; 
            }
            case Orientation.Cross: {
                colliderLength = Vector3.Distance(TransformPoint1.transform.position, TransformPoint2.transform.position);
                transform.position = (TransformPoint2.transform.position - TransformPoint1.transform.position) / 2 + TransformPoint1.transform.position;
                transform.rotation = Quaternion.LookRotation(TransformPoint1.transform.position - TransformPoint2.transform.position);
                transform.localScale = new Vector3(width, width, colliderLength / TransformPoint1.transform.localScale.x);
                break;
            }
            case Orientation.Horizontal: {
                colliderLength = Vector3.Distance(TransformPoint1.transform.position, TransformPoint2.transform.position);
                transform.position = (TransformPoint2.transform.position - TransformPoint1.transform.position) / 2 + TransformPoint1.transform.position;
                transform.rotation = Quaternion.LookRotation(TransformPoint1.transform.position - TransformPoint2.transform.position);
                transform.localScale = new Vector3(width, width, colliderLength / TransformPoint1.transform.localScale.x);
                break;
            }
        } 
    }

    public enum Orientation {
        Vertical,
        VerticalRPRP,

        Cross,
        Horizontal
    }
}
