using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingForLoopTest : MonoBehaviour
{
    [SerializeField] LineRenderer lr;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Transform endPosition;
    [SerializeField] Vector3[] ghostPositions;
    [SerializeField] Vector3[] ropePoint;
    [SerializeField] Vector3[] desiredPosition;
    [SerializeField] float desiredDuration;
    [SerializeField] float maxDistance;
    [SerializeField] float distanceToTarget;
    [SerializeField] List<Transform> ropeShotVisualizerSpawnPoints;
    float elapsedTime; 
    Vector3 parentTarget;


    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = ropeShotVisualizerSpawnPoints.Count;
        ropePoint = new Vector3[1];
        ghostPositions = new Vector3[ropeShotVisualizerSpawnPoints.Count];
        desiredPosition = new Vector3[ropeShotVisualizerSpawnPoints.Count];
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        ropePoint[0] = Vector3.Lerp(startPosition, endPosition.position, elapsedTime / desiredDuration);
        //Create an Array of points using Transforms
        Vector3[] lineArray = new Vector3[ropeShotVisualizerSpawnPoints.Count + 1];
        lineArray[0] = startPosition;
        //Define the target and end of the ghost line
        Vector3 endPoint = endPosition.position;
        Vector3 lineDir = ropePoint[0] - lineArray[0];
        //Create a ghost line - where the points would be if the line were straight
        for (int i = 0; i < 5; i++)
        {
            ghostPositions[i] = lineArray[0] + (lineDir.normalized * lineDir.magnitude * (1 - Mathf.Clamp01((float) i / (float)ropeShotVisualizerSpawnPoints.Count)));
        }
        Vector3 parentPosition = endPoint;
        Vector3 parentTarget = ropePoint[0];

        for (int i = 0; i < 1; i++)
        {
            maxDistance = (parentTarget - lineArray[0]).magnitude;
            distanceToTarget = (parentTarget - parentPosition).magnitude;
            desiredPosition[0] = Vector3.Lerp(ropeShotVisualizerSpawnPoints[i].position, parentPosition, 1 - Mathf.Clamp01(distanceToTarget / maxDistance));
            lineArray[8] = desiredPosition[0];
        }

        for (int i = 1; i < 2; i++)
        {
            maxDistance = (ghostPositions[0] - lineArray[0]).magnitude;
            distanceToTarget = (ghostPositions[0] - desiredPosition[0]).magnitude;
            desiredPosition[1] = Vector3.Lerp(ropeShotVisualizerSpawnPoints[1].position, desiredPosition[0], 1 - Mathf.Clamp01(distanceToTarget / maxDistance));
            lineArray[7] = desiredPosition[1];
        }

        for (int i = 2; i < 3; i++)
        {
            maxDistance = (ghostPositions[1] - lineArray[0]).magnitude;
            distanceToTarget = (ghostPositions[1] - desiredPosition[1]).magnitude;
            desiredPosition[2] = Vector3.Lerp(ropeShotVisualizerSpawnPoints[2].position, desiredPosition[1], 1 - Mathf.Clamp01(distanceToTarget / maxDistance));
            lineArray[6] = desiredPosition[2];
        }

        for (int i = 3; i < 4; i++)
        {
            maxDistance = (ghostPositions[2] - lineArray[0]).magnitude;
            distanceToTarget = (ghostPositions[2] - desiredPosition[2]).magnitude;
            desiredPosition[3] = Vector3.Lerp(ropeShotVisualizerSpawnPoints[3].position, desiredPosition[2], 1 - Mathf.Clamp01(distanceToTarget / maxDistance));
            lineArray[5] = desiredPosition[3];
        }

        for (int i = 4; i < 5; i++)
        {
            maxDistance = (ghostPositions[3] - lineArray[0]).magnitude;
            distanceToTarget = (ghostPositions[3] - desiredPosition[3]).magnitude;
            desiredPosition[4] = Vector3.Lerp(ropeShotVisualizerSpawnPoints[4].position, desiredPosition[3], 1 - Mathf.Clamp01(distanceToTarget / maxDistance));
            lineArray[4] = desiredPosition[4];
        }

        for (int i = 5; i < 6; i++)
        {
            maxDistance = (ghostPositions[4] - lineArray[0]).magnitude;
            distanceToTarget = (ghostPositions[4] - desiredPosition[4]).magnitude;
            desiredPosition[5] = Vector3.Lerp(ropeShotVisualizerSpawnPoints[5].position, desiredPosition[4], 1 - Mathf.Clamp01(distanceToTarget / maxDistance));
            lineArray[3] = desiredPosition[5];
        }

        for (int i = 6; i < 7; i++)
        {
            maxDistance = (ghostPositions[5] - lineArray[0]).magnitude;
            distanceToTarget = (ghostPositions[5] - desiredPosition[5]).magnitude;
            desiredPosition[6] = Vector3.Lerp(ropeShotVisualizerSpawnPoints[6].position, desiredPosition[5], 1 - Mathf.Clamp01(distanceToTarget / maxDistance));
            lineArray[2] = desiredPosition[6];
        }

        for (int i = 7; i < 8; i++)
        {
            maxDistance = (ghostPositions[6] - lineArray[0]).magnitude;
            distanceToTarget = (ghostPositions[6] - desiredPosition[6]).magnitude;
            desiredPosition[7] = Vector3.Lerp(ropeShotVisualizerSpawnPoints[7].position, desiredPosition[6], 1 - Mathf.Clamp01(distanceToTarget / maxDistance));
            lineArray[1] = desiredPosition[7];
        }

        //lineArray[lineArray.Length - 1] = endPoint;
        lineArray[0] = startPosition;
        lr.SetPositions(lineArray);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (ghostPositions.Length != 0)
        {
            for (int i = 0; i < ropeShotVisualizerSpawnPoints.Count; i++)
            {
                Gizmos.DrawWireSphere(ghostPositions[i], 0.05f);
            }
        }
        
        Gizmos.color = Color.red;
        if (desiredPosition.Length != 0)
        {
            for (int i = 0; i < ropeShotVisualizerSpawnPoints.Count; i++)
            {
                Gizmos.DrawWireSphere(desiredPosition[i], 0.05f);
            }
        }
        
    }
}
