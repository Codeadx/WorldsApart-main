using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineWaveDebug : MonoBehaviour
{
    LineRenderer line;
    [SerializeField] bool enableLerp = false;
    [SerializeField] float desiredDuration = 3f;
    [SerializeField] int points = 20;
    [SerializeField] float amplitude = 1;
    [SerializeField] float frequency = 1 * 2 * Mathf.PI;
    [SerializeField] float movementSpeed = 1;
    [SerializeField] List<Vector3> ghostPositions;
    [SerializeField] Vector3[] linePositions;
    [SerializeField] Vector3[] newPos;
    [SerializeField] Transform startPosition;
    [SerializeField] Transform endPosition;
    [SerializeField] float elapsedTime;
    [SerializeField] float elapsedTime2;

    // Start is called before the first frame update
    void Start()
    {
        linePositions = new Vector3[points];
        newPos = new Vector3[linePositions.Length];
        line = GetComponent<LineRenderer>();
    }

    void Draw()
    {
        line.positionCount = points;
        float percentageCompletion = elapsedTime / desiredDuration;
        //Set lineRenderer points count = int points
        for (int i = 0; i < points; i++)
        {
            float progress = (float)i / (points);
            float z = Mathf.Lerp(startPosition.position.z, endPosition.position.z, progress);
            float y = amplitude * Mathf.Sin((z * frequency) + (elapsedTime * movementSpeed)) + 2f;
            linePositions[i] = new Vector3(0, y, z);
            
            //Lerp the line from [0] to [i] over specified deltaTime
            newPos[i] = Vector3.Lerp(linePositions[0], linePositions[i], percentageCompletion);
            line.SetPositions(newPos);
        }
    }

    void Update()
    {
        Draw();
        elapsedTime += Time.deltaTime;
        if (enableLerp == true && elapsedTime >= desiredDuration)
        {
            elapsedTime += Time.deltaTime;
            //Create a list of ghostPositions which are points along the line from startPos to endPos if they were straight
            if (ghostPositions.Count < points)
            {
                for (float i = 0f; i <= 1; i += 1f / points)
                {
                    ghostPositions.Add(Vector3.Lerp(startPosition.position, endPosition.position, i));
                }
            }

            elapsedTime2 += Time.deltaTime;
            float time = 1f;
            float percentageCompletion = elapsedTime2 / time;
            for (int i = 0; i < points; i++)
            {
                linePositions[i] = Vector3.Lerp(linePositions[i], ghostPositions[i], percentageCompletion);
                line.SetPositions(linePositions);
            }

            for (int i = 0; i <= 39; i++)
            {
                Debug.DrawLine(ghostPositions[0], ghostPositions[i], Color.red);
            }
        }
    }
}
