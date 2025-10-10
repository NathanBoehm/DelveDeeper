using UnityEngine;

public class ViewConeVisualizer : MonoBehaviour
{
    public float angle = 45f;      // Half-angle of the cone
    public float maxDistance = 5f; // How far the cone extends
    public Color fillColor = new Color(0f, 1f, 0f, 0.25f); // Semi-transparent green
    public Vector3 positionOffset = Vector3.zero;

    private void OnDrawGizmos()
    {
        Vector3 position = transform.position + positionOffset;
        Vector3 forward = transform.forward;

        // Flatten forward to XZ plane
        Vector3 forwardXZ = new Vector3(forward.x, 0f, forward.z).normalized;

        // Draw the cone edges for reference
        Quaternion leftRot = Quaternion.AngleAxis(-angle, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(angle, Vector3.up);

        Vector3 leftDir = leftRot * forwardXZ;
        Vector3 rightDir = rightRot * forwardXZ;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(position, leftDir * maxDistance);
        Gizmos.DrawRay(position, rightDir * maxDistance);

        // Draw the arc as a filled fan
        DrawFilledConeXZ(position, forwardXZ, angle, maxDistance, fillColor);
    }

    private void DrawFilledConeXZ(Vector3 position, Vector3 forwardXZ, float angle, float radius, Color color)
    {
        int segments = 30; // smoothness of the arc
        float step = angle * 2f / segments;

        // Precompute rotation offset
        Quaternion startRot = Quaternion.AngleAxis(-angle, Vector3.up);

        Vector3 prevPoint = position + (startRot * forwardXZ) * radius;

        // Begin drawing filled triangles
        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = -angle + step * i;
            Quaternion rot = Quaternion.AngleAxis(currentAngle, Vector3.up);
            Vector3 nextPoint = position + (rot * forwardXZ) * radius;

            // Draw a triangle between position, prevPoint, and nextPoint
            Gizmos.color = color;
            DrawTriangle(position, prevPoint, nextPoint);

            prevPoint = nextPoint;
        }
    }

    private void DrawTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        // Unity's Gizmos can't fill triangles, but we can simulate a filled effect
        // by drawing many overlapping transparent triangles
        // Here we approximate it using a line mesh style
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, a);
    }
}