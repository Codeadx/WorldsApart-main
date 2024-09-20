using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCam : MonoBehaviour
{
    Vector3 rotation = new Vector3(0, 0, 0);


    public float zoomLevel;
    public float sensitivity = 1f;
    public float speed = 30f;
    public float maxZoom = 30f;
    float zoomPosition;

    void Update() {
        if(Input.GetMouseButton(1)) {
            Orbit();
        }

        zoomLevel += Input.mouseScrollDelta.y * sensitivity;
        zoomLevel = Mathf.Clamp(zoomLevel, -maxZoom, 0);
        zoomPosition = Mathf.MoveTowards(zoomPosition, zoomLevel, speed * Time.deltaTime);
        transform.position = transform.forward * zoomPosition;
    }

    void Orbit() {
        rotation = transform.localEulerAngles;
        rotation.x -= Input.GetAxis("Mouse Y");
        rotation.y += Input.GetAxis("Mouse X");
        transform.localEulerAngles = rotation;
    }
}
