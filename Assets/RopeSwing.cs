using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSwing : MonoBehaviour
{
    public Transform player;
    public Transform ropeAnchor;

    public float maxTension = 100f;
    public float tensionIncreaseRate = 5f;
    public float tensionDecreaseRate = 10f;
    public float ropeLength = 5f;
    public float swingForce = 10f;
    public float reelSpeed = 5f;

    public float currentTension = 0f;
    private Vector3 velocity = Vector2.zero;

    Vector3 playerPosition;
    Vector3 anchorPosition;

    void FixedUpdate()
    {
        Time.timeScale = 0.1f;

        playerPosition = player.position;
        anchorPosition = ropeAnchor.position;

        // Calculate distance and direction between player and anchor
        Vector3 direction = (playerPosition - anchorPosition).normalized;
        float distance = Vector3.Distance(playerPosition, anchorPosition);

        // Reel in or out based on left shift or left control key
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentTension -= reelSpeed * Time.fixedDeltaTime;

            if (currentTension < 0f)
            {
                currentTension = 0f;
            }
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            currentTension += reelSpeed * Time.fixedDeltaTime;

            if (currentTension > maxTension)
            {
                currentTension = maxTension;
            }
        }
        else
        {
            if (distance > ropeLength)
            {
                // Increase tension
                currentTension += tensionIncreaseRate * Time.fixedDeltaTime;

                if (currentTension > maxTension)
                {
                    currentTension = maxTension;
                }
            }
            else
            {
                // Decrease tension
                currentTension -= tensionDecreaseRate * Time.fixedDeltaTime;

                if (currentTension < 0f)
                {
                    currentTension = 0f;
                }
            }
        }

        // Calculate new player position based on tension
        Vector3 newPosition = anchorPosition + direction * (ropeLength + currentTension);

        // Calculate player velocity based on new and old position
        velocity = (newPosition - playerPosition) / Time.fixedDeltaTime;

        // Apply swing force
        Vector3 perpendicular = new Vector3(-direction.y, -direction.x);
        float swingVelocity = Vector3.Dot(velocity, perpendicular);
        playerPosition += perpendicular * swingVelocity * swingForce * Time.fixedDeltaTime;

        // Update player position
        if(currentTension >= maxTension) {
            player.position = playerPosition;
        }
    }
    
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerPosition, 1f);
    }
}
