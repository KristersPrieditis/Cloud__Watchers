using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    [Header("Cloud Settings")]
    public List<GameObject> clouds; // List of objects to move
    public float radius = 5f; // Radius around the central object
    public float speed = 1f; // Speed of movement
    public float jumpHeight = 1f; // Height of random jumps
    public float directionChangeInterval = 2f; // Interval for direction changes

    private Dictionary<GameObject, float> angleOffsets; // Tracks each cloud's angle offset
    private Dictionary<GameObject, Vector3> initialPositions; // Tracks each cloud's initial position

    void Start()
    {
        angleOffsets = new Dictionary<GameObject, float>();
        initialPositions = new Dictionary<GameObject, Vector3>();

        float angleStep = 360f / clouds.Count; // Step size to space the clouds evenly

        for (int i = 0; i < clouds.Count; i++)
        {
            GameObject cloud = clouds[i];

            // Assign an angle based on the index, ensuring the clouds are spaced evenly
            angleOffsets[cloud] = i * angleStep;
            initialPositions[cloud] = cloud.transform.position;

            // Start a random jump coroutine for each cloud
            StartCoroutine(RandomJump(cloud));
        }

        // Start the periodic direction changes
        StartCoroutine(ChangeDirections());
    }

    void Update()
    {
        foreach (GameObject cloud in clouds)
        {
            if (cloud != null)
            {
                // Calculate the position of the cloud along the circle
                angleOffsets[cloud] += speed * Time.deltaTime; // Move the cloud
                float angleInRadians = angleOffsets[cloud] * Mathf.Deg2Rad;

                Vector3 newPosition = new Vector3(
                    Mathf.Cos(angleInRadians) * radius,
                    initialPositions[cloud].y, // Keep the original height
                    Mathf.Sin(angleInRadians) * radius
                );

                cloud.transform.position = transform.position + newPosition; // Update position
            }
        }
    }

    private IEnumerator ChangeDirections()
    {
        while (true)
        {
            yield return new WaitForSeconds(directionChangeInterval);

            foreach (GameObject cloud in clouds)
            {
                if (cloud != null)
                {
                    // Randomly change speed and direction
                    speed = Random.Range(0.5f, 2f) * (Random.value > 0.5f ? 1 : -1);
                }
            }
        }
    }

    private IEnumerator RandomJump(GameObject cloud)
    {
        while (true)
        {
            // Wait for a random time before jumping
            yield return new WaitForSeconds(Random.Range(1f, 5f));

            if (cloud != null)
            {
                Vector3 originalPosition = cloud.transform.position;
                Vector3 jumpPosition = originalPosition + Vector3.up * jumpHeight;

                // Lerp to the jump position
                float elapsedTime = 0f;
                float jumpDuration = 0.5f;

                while (elapsedTime < jumpDuration)
                {
                    cloud.transform.position = Vector3.Lerp(originalPosition, jumpPosition, elapsedTime / jumpDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                cloud.transform.position = jumpPosition;

                // Lerp back to the original position
                elapsedTime = 0f;

                while (elapsedTime < jumpDuration)
                {
                    cloud.transform.position = Vector3.Lerp(jumpPosition, originalPosition, elapsedTime / jumpDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                cloud.transform.position = originalPosition;
            }
        }
    }
}