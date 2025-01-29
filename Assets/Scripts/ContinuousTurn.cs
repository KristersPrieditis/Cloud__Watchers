using UnityEngine;
using UnityEngine.InputSystem;

public class ContinuousTurn : MonoBehaviour
{
    [Header("Turn Settings")]
    public float turnSpeed = 90f; // Degrees per second
    public InputActionProperty turnAction; // Input action for turning

    private Transform rigTransform;

    private void Start()
    {
        rigTransform = transform; // Reference to the XR Origin's transform
    }

    private void Update()
    {
        // Read input as a Vector2 (X for horizontal turning)
        Vector2 turnInput = turnAction.action.ReadValue<Vector2>();

        // Use only the X-axis for continuous turning
        float turnValue = turnInput.x;

        // Rotate the rig if there's sufficient input
        if (Mathf.Abs(turnValue) > 0.1f) // Avoid jitter from small thumbstick movements
        {
            float rotation = turnValue * turnSpeed * Time.deltaTime;
            rigTransform.Rotate(0, rotation, 0);
        }
    }

    private void OnEnable()
    {
        if (turnAction != null && turnAction.action != null)
        {
            turnAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (turnAction != null && turnAction.action != null)
        {
            turnAction.action.Disable();
        }
    }
}