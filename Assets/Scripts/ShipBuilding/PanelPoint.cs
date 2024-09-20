using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelPoint : MonoBehaviour
{
    public Framework Framework;
    public Face face;
    public Placement placement;
    public float thickness = 0.3f;
    public float colliderHeight;
    public float colliderWidth;
    public float colliderLength;
    
    public Orientation orientation;

    void LateUpdate() {
        switch (orientation) {
            case Orientation.Vertical: {
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
                
                break; 
            }
            // case Orientation.Cross: {
            //     colliderLength = Vector3.Distance(Corner1.transform.position, Corner2.transform.position);
            //     transform.position = (Corner2.transform.position - Corner1.transform.position) / 2 + Corner1.transform.position;
            //     //transform.rotation = Quaternion.LookRotation(Corner1.transform.position - Corner2.transform.position);
            //     transform.localScale = new Vector3(thickness, thickness, colliderLength / Corner1.transform.localScale.x);
            //     break;
            // }
            // case Orientation.Horizontal: {
            //     colliderLength = Vector3.Distance(Corner1.transform.position, Corner2.transform.position);
            //     transform.position = (Corner2.transform.position - Corner1.transform.position) / 2 + Corner1.transform.position;
            //     //transform.rotation = Quaternion.LookRotation(Corner1.transform.position - Corner2.transform.position);
            //     transform.localScale = new Vector3(thickness, thickness, colliderLength / Corner1.transform.localScale.x);
            //     break;
            // }
        }
    }
}

    public enum Orientation {
        Vertical,
        Cross,
        Horizontal
    }

    public enum Placement {
        Top,
        Middle,
        Bottom
    }

    public enum Face {
        One,
        Two,
        Three,
        Four,
        Five,
        Six
    }
