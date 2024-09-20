using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public MyCharacterController Character;
    public MyGun myGun;
    public Transform xHair;

    void Update() {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, myGun.range)) {
            if(Vector3.Distance(Character.transform.position, hit.point) < myGun.range) {
                xHair.transform.Rotate(0, 0, 180 * Time.deltaTime);
            }
        } else {
            xHair.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 10f);
        } 
    }
}
