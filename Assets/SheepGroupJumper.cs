using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SheepGroupJumper : MonoBehaviour
{
    public GameObject sheepPrefab; // Assign the sheep group prefab
    public Transform[] cloudDestinations; // The clouds they jump between
    public float jumpHeight = 2f; // Height of the jump
    public float jumpDuration = 1f; // How long each jump takes
    public int maxJumps = 10; // How many loops before despawning
    public AudioClip baaSound; // Assign in Inspector

    private GameObject currentSheepGroup; // Reference to the active sheep group
    private int jumpCount = 0;
    private AudioSource audioSource;

    public void SpawnSheep()
    {
        if (sheepPrefab == null || cloudDestinations.Length < 2)
        {
            Debug.LogError("SheepHandler: Missing required references!");
            return;
        }

        // Spawn the sheep group at the first cloud position
        currentSheepGroup = Instantiate(sheepPrefab, cloudDestinations[0].position, Quaternion.identity);

        // Attach an AudioSource to the sheep group if not already there
        audioSource = currentSheepGroup.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = currentSheepGroup.AddComponent<AudioSource>();
        }

        audioSource.clip = baaSound;
        audioSource.playOnAwake = false; // Prevent auto-playing
        StartCoroutine(JumpLoop(currentSheepGroup));
    }

    IEnumerator JumpLoop(GameObject sheepGroup)
    {
        int cloudIndex = 0;

        while (jumpCount < maxJumps)
        {
            // Determine next target cloud
            Transform targetCloud = cloudDestinations[cloudIndex];

            // Rotate towards target before jumping
            RotateTowardsTarget(sheepGroup, targetCloud);

            // Wait before jumping (to allow rotation to complete)
            yield return new WaitForSeconds(1.5f);

            // Play "Baa" sound before jumping
            if (audioSource != null && baaSound != null)
            {
                audioSource.Play();
            }

            // Jump to target cloud
            yield return JumpToCloud(sheepGroup, targetCloud);

            // Wait before selecting next cloud
            yield return new WaitForSeconds(1.5f);

            // Select next cloud in cyclic order (looping back after last cloud)
            cloudIndex = (cloudIndex + 1) % cloudDestinations.Length;

            jumpCount++;
        }

        // Despawn the sheep group after max jumps
        Destroy(sheepGroup);
    }

    IEnumerator JumpToCloud(GameObject sheepGroup, Transform targetCloud)
    {
        if (sheepGroup == null || targetCloud == null) yield break;

        Vector3 startPosition = sheepGroup.transform.position;
        Vector3 targetPosition = targetCloud.position;
        float timeElapsed = 0;

        while (timeElapsed < jumpDuration)
        {
            float progress = timeElapsed / jumpDuration;

            // Arc movement for jumping effect
            float heightOffset = jumpHeight * Mathf.Sin(Mathf.PI * progress);
            sheepGroup.transform.position = Vector3.Lerp(startPosition, targetPosition, progress) + new Vector3(0, heightOffset, 0);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is accurate
        sheepGroup.transform.position = targetPosition;
    }

    void RotateTowardsTarget(GameObject sheepGroup, Transform targetCloud)
    {
        if (sheepGroup == null || targetCloud == null) return;

        Vector3 directionToTarget = targetCloud.position - sheepGroup.transform.position;
        directionToTarget.y = 0; // Keep rotation horizontal (no tilting up/down)

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            sheepGroup.transform.rotation = targetRotation;
        }
    }

    public void StartMorning()
    {
        jumpCount = 0;
        SpawnSheep(); // Restart sheep jumping when morning begins
    }
}
