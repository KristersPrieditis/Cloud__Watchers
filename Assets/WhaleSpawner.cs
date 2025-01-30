using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WhaleSpawner : MonoBehaviour
{
    [Header("Whale Settings")]
    public GameObject whalePrefab;
    public float whaleSpeed = 5f;
    public float rotationSpeed = 2f;

    [Header("Pod Formation Settings")]
    public Vector3 spawnBasePosition = new Vector3(-50, 0, -30);
    public Vector3 destinationPosition = new Vector3(50, 0, 30);
    public float podSpacing = 25f; // Increased for larger whales
    public float verticalOffset = 7.5f; // Increased for better separation
    public float spawnTimeVariation = 2f;

    [Header("Curved Path Settings")]
    public Vector3 curveMidpoint = new Vector3(0, 0, -10);

    private List<GameObject> activeWhales = new List<GameObject>();

    public void StartWhaleEvent()
    {
        StartCoroutine(SpawnWhalePod());
    }

    public void StopWhaleEvent()
    {
        foreach (GameObject whale in activeWhales)
        {
            if (whale != null)
            {
                Destroy(whale);
            }
        }
        activeWhales.Clear();
    }

    private IEnumerator SpawnWhalePod()
    {
        Vector3[] baseOffsets = new Vector3[]
        {
            new Vector3(0, 0, 0),  // Lead whale (center)
            new Vector3(podSpacing, -verticalOffset, 10f), // Right whale further apart
            new Vector3(-podSpacing, -verticalOffset, -10f), // Left whale further apart
            new Vector3(podSpacing * 0.5f, -verticalOffset * 2, -20f) // Small calf behind the pod
        };

        float[] whaleScales = new float[]
        {
            1.3f,
            1.0f,
            0.85f,
            0.6f
        };

        for (int i = 0; i < baseOffsets.Length; i++)
        {
            // Increase random offset range to avoid overlapping
            Vector3 randomOffset = new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), Random.Range(-10f, 10f));
            Vector3 spawnPosition = spawnBasePosition + baseOffsets[i] + randomOffset;

            float delay = Random.Range(0, spawnTimeVariation);
            yield return new WaitForSeconds(delay);

            SpawnWhale(spawnPosition, whaleScales[i]);
        }
    }

    private void SpawnWhale(Vector3 position, float scale)
    {
        GameObject newWhale = Instantiate(whalePrefab, position, Quaternion.Euler(0, 0, 0)); // No forced rotation
        newWhale.transform.localScale *= scale;
        activeWhales.Add(newWhale);
        StartCoroutine(MoveWhaleThroughCurve(newWhale));
    }

    private IEnumerator MoveWhaleThroughCurve(GameObject whale)
    {
        float t = 0f; // Interpolation factor

        Vector3 startPos = whale.transform.position;
        Vector3 endPos = destinationPosition;
        Vector3 midPoint = curveMidpoint + new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-5f, 5f)); // Slight random midpoint variation

        while (t < 1f)
        {
            t += Time.deltaTime * (whaleSpeed / 20f); // Control speed with interpolation

            // Quadratic Bezier Curve: Lerp between two Lerp positions
            Vector3 pointA = Vector3.Lerp(startPos, midPoint, t);
            Vector3 pointB = Vector3.Lerp(midPoint, endPos, t);
            whale.transform.position = Vector3.Lerp(pointA, pointB, t);

            // Flip the rotation to make whales face correctly
            Vector3 direction = (whale.transform.position - pointB).normalized; // Reverse direction
            Quaternion targetRotation = Quaternion.LookRotation(direction); // Face away from destination
            whale.transform.rotation = Quaternion.Slerp(whale.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            yield return null;
        }

        // Remove whale when it reaches destination
        activeWhales.Remove(whale);
        Destroy(whale);
    }
}
