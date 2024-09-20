using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGunDebug : MonoBehaviour
{
    Rigidbody rb;
    float distance;
    public Transform hookHitPoint;
    public float tetherLength;

    public float maxTension = 2f;
    public float reelSpeed = 5f;
    public float currentTension = 0f;
    public float tensionIncreaseRate = 5f;
    public float tensionDecreaseRate = 10f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tetherLength = Vector3.Distance(transform.position, hookHitPoint.transform.position);
        Mathf.Clamp(currentTension, 0f, 5f);
    }

    // Update is called once per frame
    void LateUpdate()
    { 
        Vector3 directionToGrapple = Vector3.Normalize(hookHitPoint.transform.position - transform.position);
        
        if (Input.GetKey(KeyCode.LeftShift)) {
            currentTension -= reelSpeed * Time.fixedDeltaTime;
            if (currentTension < 0f) {
                currentTension = 0f;
            } if (currentTension > maxTension) {
                    currentTension = maxTension;
                }
        } else if (Input.GetKey(KeyCode.LeftControl)) {
            currentTension += reelSpeed * Time.fixedDeltaTime;
        } else {
            if (distance > tetherLength) {
                // Increase tension
                currentTension += tensionIncreaseRate * Time.fixedDeltaTime;

            } else {
                // Decrease tension
                currentTension -= tensionDecreaseRate * Time.fixedDeltaTime;
                if (currentTension < 0f) {
                    currentTension = 0f;
                }
            }
        }
        
        float speedTowardsGrapplePoint = Mathf.Round(Vector3.Dot(rb.velocity, directionToGrapple) * 100) / 100; //How much the velocity is pointing in the opposite direction of the tether point
        distance = Vector3.Distance(hookHitPoint.transform.position, transform.position);
        if (speedTowardsGrapplePoint < 0) {
            rb.velocity -= speedTowardsGrapplePoint * currentTension * directionToGrapple;
        }
    }
}
