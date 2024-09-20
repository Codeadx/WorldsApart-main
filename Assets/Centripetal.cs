using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Centripetal : MonoBehaviour
{
    public float radius;
    public Transform target;

    public Rigidbody rb;

    void Start()
    {

    }

    void FixedUpdate()
    {
        Vector3 velocity = rb.velocity;
        float speed = velocity.magnitude;
        float centripetalAcceleration = (speed * speed) / radius;

        // Apply the centripetal acceleration to the object
        Vector3 directionToTarget = target.position - transform.position;
        Vector3 centripetalForce = directionToTarget.normalized * centripetalAcceleration * rb.mass;
        rb.AddForce(centripetalForce, ForceMode.Acceleration);
    }
}
