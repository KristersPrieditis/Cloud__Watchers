using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject fishPrefab;  // Assign your Fish prefab here
    public Transform spawnLocation; // The central point where fish spawn
    public float spawnRadius = 5f;  // Radius around the spawn point
    public int fishCount = 10;  // Number of fish to spawn

    private List<GameObject> spawnedFish = new List<GameObject>();

    public void SpawnFish()
    {
        if (spawnedFish.Count > 0) return; // Prevent spawning duplicates

        for (int i = 0; i < fishCount; i++)
        {
            Vector3 randomPosition = spawnLocation.position + (Random.insideUnitSphere * spawnRadius);
            randomPosition.y = spawnLocation.position.y; // Keep fish at the correct height

            GameObject fish = Instantiate(fishPrefab, randomPosition, Quaternion.identity);
            spawnedFish.Add(fish);
        }
    }

    public void DespawnFish()
    {
        foreach (GameObject fish in spawnedFish)
        {
            if (fish != null) Destroy(fish);
        }
        spawnedFish.Clear();
    }
}
