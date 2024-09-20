using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    LineRenderer line;
    Camera Camera;
    [SerializeField] Transform gunTip;
    [SerializeField] Animator animator;
    [SerializeField] RopeRandomizer ropeRandomizer;
    public List<Transform> ropeShotVisualizerSpawnPoints;
    Vector3[] ghostPositions;
    float distanceToTarget;
    float maxDistance;
    float desiredDuration = 0.654f;
    [SerializeField] public Vector3 hookStartPoint;
    [SerializeField] public Vector3 hookHitPoint;
    [SerializeField] public float tetherLength;
    public Vector3 hitNormal;
    public float maximumTetherDistance = 100f;
    public float minimumTetherDistance = 10f;
    Vector3[] lineArray;
    Vector3[] ropePoints;
    Vector3 endPoint;
    Vector3 lineDir;
    Vector3 parentPosition;
    Vector3 parentTarget;
    Vector3 desiredPosition;
    float elapsedTime;
    public bool _tethered = false;
    bool _mouseIsDown = false;
    int randomSpawn;

    void Start() {
        line = GetComponent<LineRenderer>();
        ropePoints = new Vector3[1];
        ropeRandomizer = GetComponent<RopeRandomizer>();
        Camera = Camera.main;
    }

    private void LateUpdate() {
        if (Input.GetMouseButtonDown(0)) {
            randomSpawn = Random.Range(0, ropeRandomizer.spawnPoints.Length);
            _mouseIsDown = true;
            Ray ray = new Ray (Camera.transform.position, Camera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maximumTetherDistance)) {
                if (hit.collider != null) {
                    elapsedTime = 0;
                    _tethered = true;
                    hookStartPoint = gunTip.position;
                    hookHitPoint = hit.point;  
                    hitNormal = hit.normal;
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            _mouseIsDown = false;
            _tethered = false;
        }

        if (_tethered) {
            line.positionCount = ropeRandomizer.spawnPoints[randomSpawn].Count;
            elapsedTime += Time.deltaTime;
            ropePoints[0] = Vector3.Lerp(hookStartPoint, hookHitPoint, elapsedTime / desiredDuration);
            DrawRope();
        } else if (!_tethered) {
            line.positionCount = 0;
            elapsedTime = 0;
            ropePoints[0] = gunTip.position;
        }
        tetherLength = Vector3.Distance(transform.position, hookHitPoint);
    }

    void DrawRope() {
        line.positionCount = ropeRandomizer.spawnPoints[randomSpawn].Count;
        //Create the array of points from Transforms
        lineArray = new Vector3[ropeRandomizer.spawnPoints[randomSpawn].Count + 1];
        lineArray[0] = gunTip.position;
        //Define the target and end of the ghost line
        endPoint = hookHitPoint;
        lineDir = ropePoints[0] - lineArray[0];
        //Create the ghost line - where the points should be if the line were straight
        ghostPositions = new Vector3[ropeRandomizer.spawnPoints[randomSpawn].Count];
        for (int i = 0; i < ropeRandomizer.spawnPoints[randomSpawn].Count; i++) {
            ghostPositions[i] = lineArray[0] + (lineDir.normalized * lineDir.magnitude * (1 - Mathf.Clamp01((float) i / (float)ropeRandomizer.spawnPoints[randomSpawn].Count)));
        }
        //Calculate the positions of each point based off its parent - parentPosition and parentTarget need to run once outside of if statement
        parentPosition = endPoint;
        parentTarget = ropePoints[0];
        for (int i = 0; i < ropeRandomizer.spawnPoints[randomSpawn].Count; i++) {
            maxDistance = (parentTarget - lineArray[0]).magnitude;
            distanceToTarget = (parentTarget - parentPosition).magnitude;

            desiredPosition = Vector3.Lerp(ropeRandomizer.spawnPoints[randomSpawn][i].position, parentPosition, elapsedTime / desiredDuration);
            lineArray[ropeRandomizer.spawnPoints[randomSpawn].Count - 1 - i] = desiredPosition;
            parentPosition = lineArray[ropeRandomizer.spawnPoints[randomSpawn].Count - 1 - i];
            parentTarget = ghostPositions[i];
        }
        lineArray[lineArray.Length - 1] = endPoint;
        lineArray[0] = gunTip.position;
        line.SetPositions(lineArray);
    }
}
