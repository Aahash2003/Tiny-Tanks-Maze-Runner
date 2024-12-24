using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // Reference to the penguin (or player)
    public Vector3 offset;    // Offset between the camera and the target
    public float smoothSpeed = 0.125f; // Speed of the smooth transition

    void LateUpdate()
    {
        // Ensure the camera follows the target with an offset
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
