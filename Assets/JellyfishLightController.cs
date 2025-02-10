using UnityEngine;

public class JellyfishLightController : MonoBehaviour
{
    public Light groupLight; // Assign the light in the inspector
    public Color startColor = Color.cyan;
    public Color endColor = Color.magenta;
    public float colorChangeSpeed = 1f; // Adjust for slower/faster transition

    private float t = 0;

    void Update()
    {
        if (groupLight == null) return;

        // Smoothly transition between colors
        t += Time.deltaTime * colorChangeSpeed;
        groupLight.color = Color.Lerp(startColor, endColor, Mathf.PingPong(t, 1));
    }
}
