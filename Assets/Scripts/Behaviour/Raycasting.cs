using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using KinematicCharacterController;
 
public class Raycasting : MonoBehaviour
{
    public Animator animator;
    public Transform Camera, Character, gunTip;
    public KinematicCharacterMotor Motor;
    public LineRenderer line;
    float distance;
    public LayerMask whatIsGrappleable;
    public bool tethered = false;
    public GameObject originalHook;
    public List<Vector3> hookPositions;
    public List<GameObject> hooks;
    private GameObject hookClone;
    private Vector3[] lineArray;
    private Vector3[] ghostPositions;
    private Vector3 desiredPosition;
    private Vector3 hookHitPoint;
    private Vector3 hookStartPoint;
    public List<Transform> ropeShotVisualizerSpawnPoints;
    public List<Vector3> ropePoints;
    private bool solved = false;
    public float distanceToTarget;
    public float maxDistance;
    private Vector3 endPoint;
 
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
    }
    void CheckInputs()
    {
        if (Input.GetMouseButtonDown(0))
        {  
            Ray ray = new Ray (Camera.position, Camera.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, whatIsGrappleable))
            {
                hookPositions.Add(hit.point);
                hookHitPoint = hit.point;
                hookStartPoint = gunTip.position;

                tethered = true;
                line.positionCount = 8;

                GameObject hookClone = Instantiate(originalHook, hookPositions[0], Quaternion.identity);
                hooks.Add(hookClone);

                ropePoints.Add(hookStartPoint);
                animator.SetBool("_isGrappling", true);
            }
        }
 
        if (tethered == true)
        {
            if (hooks.Count > 0)
            {                  
                Vector3[] lineArray = new Vector3[ropeShotVisualizerSpawnPoints.Count + 1];
                lineArray[0] = gunTip.position;

                endPoint = hooks[0].transform.position + (-hooks[0].transform.forward * 0.15f);
                Vector3 lineDir = (ropePoints[0] - lineArray[0]);

                Vector3[] ghostPositions = new Vector3[ropeShotVisualizerSpawnPoints.Count];
                for (int i = 0; i < ropeShotVisualizerSpawnPoints.Count; i++)
                {
                    ghostPositions[i] = lineArray[0] + (lineDir.normalized * lineDir.magnitude * (1 - Mathf.Clamp01((float)i / (float)ropeShotVisualizerSpawnPoints.Count)));
                }

                Vector3 parentPosition = endPoint;
                Vector3 parentTarget = ropePoints[0];
                
                for (int i = 0; i < ropeShotVisualizerSpawnPoints.Count; i++)
                {
                    maxDistance = (parentTarget - lineArray[0]).magnitude;
                    distanceToTarget = (parentTarget - parentPosition).magnitude;
                    Vector3 desiredPosition = Vector3.Lerp(ropeShotVisualizerSpawnPoints[i].position, parentPosition, 1 - Mathf.Clamp01(distanceToTarget/maxDistance));
                    lineArray[ropeShotVisualizerSpawnPoints.Count - 1 - i] = desiredPosition;
                    parentPosition = desiredPosition;
                    parentTarget = ghostPositions[i];                   
                }
                DrawRope();
            }
        }
        
        if (solved == true)
        {
            ropePoints[0] = Vector3.Lerp(ropePoints[0], hookHitPoint, 1); 
        }

        if (Input.GetMouseButtonUp(0))
        {
            hookPositions.Clear();
            line.positionCount = 0;
            tethered = false;
            solved = false;
            ropePoints.Clear();
            animator.SetBool("_isGrappling", false);
        }
        
        if (hooks.Count > 0)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Destroy(hooks[0].gameObject);
                hooks.Clear();
            }
        }
    }
    void Update()
    {     
        CheckInputs(); 
    }

    public void DrawRope()
    {
        lineArray[lineArray.Length - 1] = endPoint;
        lineArray[0] = gunTip.position;
        line.SetPositions(lineArray);
        solved = true;
    }
    public void OnDrawGizmos()
    {
    }
 
    public Vector3 GetHookHitPoint()
    {
        return hookHitPoint;
    }
 
    public Vector3 GetHookStartPoint()
    {
        return hookStartPoint;
    }
 
    public float GetDistance()
    {
        return distance;
    }
}
 