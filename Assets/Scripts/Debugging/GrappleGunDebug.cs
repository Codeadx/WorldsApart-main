using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleGunDebug : MonoBehaviour
{
    [SerializeField] Vector3 hookHitPoint;
    Vector3 directionToGrapple;
    [SerializeField] float desiredDuration = 3f;
    float percentageCompletion;
    Vector3 Gravity = new Vector3 (0f, -32.2f, 0f);
    float desiredRopeLength = 8f;
    public bool tethered;
    float elaspedTime;
    [SerializeField] private float _distance;
    [SerializeField] private float _drag = 0.01f;
    [SerializeField] float speedTowardsGrapplePoint;
    [SerializeField] float elapsedTime;
    float tetherLength;
    
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        tethered = true;
        rb = GetComponent<Rigidbody>();
        tetherLength = Vector3.Distance(hookHitPoint, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
        percentageCompletion = elaspedTime / desiredDuration;

        rb.velocity += Gravity * Time.deltaTime;
        rb.velocity *= (1f / (1f + (_drag * Time.deltaTime)));

        if (tethered == true)
        {
            directionToGrapple = Vector3.Normalize(hookHitPoint - transform.position);
            speedTowardsGrapplePoint = Mathf.Round(Vector3.Dot(rb.velocity, directionToGrapple) * 100) / 100; //How much the velocity is pointing in the opposite direction of the tether point
            _distance = Vector3.Distance(hookHitPoint, transform.position);
                
            if (speedTowardsGrapplePoint < 0)
            {
                if (_distance > tetherLength)
                {
                    rb.velocity -= speedTowardsGrapplePoint * directionToGrapple;
                    //desiredRopeLength = Vector3.Distance(transform.position, hookHitPoint);
                }
            }     
            if (Vector3.Distance(hookHitPoint, transform.position) < desiredRopeLength)
            {
                rb.velocity -= Vector3.Normalize(transform.position - hookHitPoint) * desiredRopeLength;
                desiredRopeLength = Vector3.Distance(transform.position, hookHitPoint);
            }
        }
        LerpPosition();
    }
    void LerpPosition()
    {
        if (Input.GetKey(KeyCode.LeftShift) && tethered) 
        {
            elaspedTime += Time.deltaTime;
            rb.velocity += Vector3.Lerp(hookHitPoint - transform.position, directionToGrapple, percentageCompletion);
        }      
        
        if (Input.GetKey(KeyCode.LeftControl) && tethered) 
        {
            elaspedTime += Time.deltaTime;
            rb.velocity -= Vector3.Lerp(hookHitPoint - transform.position, directionToGrapple, percentageCompletion);
        }      
        else if (!tethered)
        {
            elapsedTime = 0;
        }      
    }

}
