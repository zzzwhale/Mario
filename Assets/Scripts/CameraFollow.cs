using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Drag Mario here
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void LateUpdate()
    {
        if (target == null) return;

        float targetX = target.position.x + offset.x;
        float lockedY = offset.y;

        // Use offset.z (-10) instead of target.position.z (0)
        Vector3 desiredPosition = new Vector3(targetX, lockedY, offset.z);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}