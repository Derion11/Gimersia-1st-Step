using UnityEngine;

/// <summary>
/// Simple floating animation for card visuals (like Mario Bros coins).
/// Moves the object up and down in a smooth sine wave pattern.
/// </summary>
public class FloatingAnimation : MonoBehaviour
{
    [Header("Float Settings")]
    [Tooltip("How high the object floats (in world units)")]
    public float floatAmplitude = 0.3f;

    [Tooltip("How fast the object floats (cycles per second)")]
    public float floatSpeed = 2f;

    [Header("Optional Rotation")]
    [Tooltip("Enable rotation animation")]
    public bool enableRotation = false;

    [Tooltip("Rotation speed (degrees per second)")]
    public float rotationSpeed = 45f;

    private Vector3 startPosition;
    private float timeOffset;

    void Start()
    {
        // Store the initial position
        startPosition = transform.localPosition;

        // Random time offset so not all cards float in sync
        timeOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        // Calculate the floating offset using sine wave
        float yOffset = Mathf.Sin((Time.time * floatSpeed) + timeOffset) * floatAmplitude;

        // Apply the floating motion
        transform.localPosition = startPosition + new Vector3(0, yOffset, 0);

        // Optional rotation animation
        if (enableRotation)
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
