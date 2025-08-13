using UnityEngine;

public class CheckpointDirection : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Set gizmo color
        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * 2f; // Adjust length as needed
        Gizmos.DrawLine(start, end); // Draw a line
        Gizmos.DrawSphere(end, 0.1f); // Optional: Draw a small sphere at the end
    }
}
