using UnityEngine;

public class JellyfishGroupSpawner : MonoBehaviour
{
    public GameObject jellyfishGroupPrefab; // Assign the group prefab in the Inspector
    public Transform spawnCloud;  // Spawn location
    public Transform despawnCloud; // Destination location
    public float moveSpeed = 2f;  // Speed of movement
    public float bobbingHeight = 0.5f; // How high/low the bobbing effect goes
    public float bobbingSpeed = 2f; // Speed of the bobbing effect

    private GameObject spawnedGroup; // Reference to the spawned group
    private bool isMoving = false; // Controls movement
    private float initialY; // Initial Y position for bobbing

    public void SpawnJellyfishGroup()
    {
        if (jellyfishGroupPrefab == null || spawnCloud == null || despawnCloud == null)
        {
            Debug.LogError("JellyfishGroupSpawner: Missing required references!");
            return;
        }

        // Spawn the jellyfish group at the spawn cloud location
        spawnedGroup = Instantiate(jellyfishGroupPrefab, spawnCloud.position, Quaternion.identity);
        initialY = spawnCloud.position.y; // Store initial Y position
        isMoving = true; // Start movement
    }

    void Update()
    {
        if (!isMoving || spawnedGroup == null || despawnCloud == null) return;

        // Calculate upward movement
        Vector3 currentPosition = spawnedGroup.transform.position;
        Vector3 moveDirection = (despawnCloud.position - currentPosition).normalized;
        Vector3 moveStep = moveDirection * moveSpeed * Time.deltaTime;

        // Ensure the group moves **upward only** while bobbing
        float bobbingOffset = Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
        Vector3 newPosition = currentPosition + moveStep;
        newPosition.y += bobbingOffset; // Apply bobbing effect **on top of upward movement**

        // Apply the new position
        spawnedGroup.transform.position = newPosition;

        // Check if the group reached the despawn cloud
        if (Vector3.Distance(spawnedGroup.transform.position, despawnCloud.position) < 0.1f)
        {
            Destroy(spawnedGroup); // Despawn the jellyfish group
            isMoving = false; // Stop movement
        }
    }
}
