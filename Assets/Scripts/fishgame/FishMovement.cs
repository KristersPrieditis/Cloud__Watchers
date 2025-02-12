using System.Collections;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public FishType type;
    public float swimSpeed = 3f; // Speed of swimming
    public float turnSpeed = 2f; // Speed of turning
    public float wobbleAmount = 0.5f; // Wobble intensity
    public float wobbleSpeed = 2f; // Wobble speed

    private Vector3 targetDirection;

    void Start()
    {
        StartCoroutine(ChangeDirectionRoutine());
    }

    void Update()
    {
        // Apply forward movement
        transform.position += transform.forward * swimSpeed * Time.deltaTime;

        // Smoothly rotate towards the target direction
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        // Add a wobble effect
        float wobble = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
        transform.Rotate(Vector3.up, wobble);
    }

    IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            // Pick a new random direction
            targetDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f), Random.Range(-1f, 1f)).normalized;
            yield return new WaitForSeconds(Random.Range(2f, 5f)); // Change direction every few seconds
        }
    }
}

