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
    public float podSpacing = 25f;
    public float verticalOffset = 7.5f;
    public float spawnTimeVariation = 2f;

    [Header("Curved Path Settings")]
    public Vector3 curveMidpoint = new Vector3(0, 0, -10);

    [Header("Whale Sound Settings")]
    public AudioClip whaleSound; // Assign whale sound MP3 in Inspector
    private AudioSource audioSource;
    private List<GameObject> activeWhales = new List<GameObject>();

    private void Start()
    {
        // Setup the audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = whaleSound;
        audioSource.loop = true; // Make sure it loops
        audioSource.playOnAwake = false; // Prevent auto-play on scene start
    }

    public void StartWhaleEvent()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play(); // Start playing whale sounds
        }

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

        // Stop the whale sound when no more whales exist
        audioSource.Stop();
    }

    private IEnumerator SpawnWhalePod()
    {
        Vector3[] baseOffsets = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(podSpacing, -verticalOffset, 10f),
            new Vector3(-podSpacing, -verticalOffset, -10f),
            new Vector3(podSpacing * 0.5f, -verticalOffset * 2, -20f)
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
            Vector3 randomOffset = new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), Random.Range(-10f, 10f));
            Vector3 spawnPosition = spawnBasePosition + baseOffsets[i] + randomOffset;

            float delay = Random.Range(0, spawnTimeVariation);
            yield return new WaitForSeconds(delay);

            SpawnWhale(spawnPosition, whaleScales[i]);
        }
    }

    private void SpawnWhale(Vector3 position, float scale)
    {
        GameObject newWhale = Instantiate(whalePrefab, position, Quaternion.Euler(0, 0, 0));
        newWhale.transform.localScale *= scale;
        activeWhales.Add(newWhale);
        StartCoroutine(MoveWhaleThroughCurve(newWhale));
    }

    private IEnumerator MoveWhaleThroughCurve(GameObject whale)
    {
        float t = 0f;

        Vector3 startPos = whale.transform.position;
        Vector3 endPos = destinationPosition;
        Vector3 midPoint = curveMidpoint + new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-5f, 5f));

        while (t < 1f)
        {
            t += Time.deltaTime * (whaleSpeed / 20f);

            Vector3 pointA = Vector3.Lerp(startPos, midPoint, t);
            Vector3 pointB = Vector3.Lerp(midPoint, endPos, t);
            whale.transform.position = Vector3.Lerp(pointA, pointB, t);

            Vector3 direction = (whale.transform.position - pointB).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            whale.transform.rotation = Quaternion.Slerp(whale.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            yield return null;
        }

        activeWhales.Remove(whale);
        Destroy(whale);

        // Stop audio if all whales are gone
        if (activeWhales.Count == 0)
        {
            audioSource.Stop();
        }
    }
}
