using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class surveyorWheelGizmo : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] Animator animator;
    public MyCharacterController MyCharacterController;
    public Transform surveyorWheel;          
    private Vector3 lastPosition;
    float dist;
    float turnAngle;
    public float radius = 0.2f; 
    float angleCounter = 0f;
    public float stepDistance = 180f; 
    
    [Header("Rotation")]
    Vector3 previousVelocity;
    Vector3 previousPosition;
    Vector3 pos;

    void Start()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    void Update()
    {   
        lastPosition = new Vector3(lastPosition.x, 0F, lastPosition.z);
        Vector3 currPosition = new Vector3 (transform.position.x, 0F, transform.position.z);

        float dist = Vector3.Distance(lastPosition, currPosition);
        float turnAngle = (dist / (2 * Mathf.PI * radius)) * 360F;
 
        surveyorWheel.Rotate(new Vector3(0f, -turnAngle, 0f));
            
        angleCounter += turnAngle;

        if (angleCounter > stepDistance)
        {
            angleCounter = 0;
        }
    
        animator.SetFloat("runstage", (angleCounter/stepDistance));
        if (animator.GetFloat("runstage") > 1f)
        {
            animator.SetFloat("runstage", 0);
        } 

        animator.SetFloat("glidestage", (angleCounter/stepDistance));
        if (animator.GetFloat("glidestage") > 1f)
        {
            animator.SetFloat("glidestage", 0);
        } 

        if (animator.GetBool("_isSprinting") == true)
        {
            //stepDistance = 360f;
        }
            lastPosition = currPosition;
    }
           
    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        
        Vector3[] pos = new Vector3[8];
        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < 360; i += 90)
            {
                pos[j] = new Vector3((radius * Mathf.Cos(i / (180f / Mathf.PI))), 1, (radius * Mathf.Sin(i / (180f / Mathf.PI))));
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(pos[j], 0.05f);
            }
            
            for (int i = 45; i < 360; i += 90)
            {
                pos[j] = new Vector3((radius * Mathf.Cos(i / (180f / Mathf.PI))), 1, (radius * Mathf.Sin(i / (180f / Mathf.PI))));
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(pos[j], 0.025f);
            }
        }
    }
}
