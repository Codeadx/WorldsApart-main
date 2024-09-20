using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionDebug : MonoBehaviour
{
    public int maxReflectionCount = 5;
    public float maxStepDistance = 200;

    private void OnDrawGizmos()
    {
        DrawPredictionReflectionPattern(this.transform.position + this.transform.forward * 0.75f, this.transform.forward, maxReflectionCount);
    }

    void DrawPredictionReflectionPattern(Vector3 position, Vector3 direction, int reflectionsRemaining)
    {
        if(reflectionsRemaining == 0)
        {
            return;
        }

        Vector3 startingPosition = position;

        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, maxStepDistance))
        {
            direction = Vector3.Reflect(direction, hit.normal);
            position = hit.point;
        }
        else
        {
            position += direction * maxStepDistance;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startingPosition, position);

        DrawPredictionReflectionPattern(position, direction, reflectionsRemaining - 1);
    }
}
