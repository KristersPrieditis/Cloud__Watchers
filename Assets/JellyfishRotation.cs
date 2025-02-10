using UnityEngine;

public class JellyfishRotation : MonoBehaviour
{
    public float rotationSpeed = 10f; // Adjustable rotation speed

    void Update()
    {
        // Rotate slowly over time
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
