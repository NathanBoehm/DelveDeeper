using EditorAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeadTrackingTarget : MonoBehaviour
{
    public static List<HeadTrackingTarget> Targets = new List<HeadTrackingTarget>();

    public Transform Transform { get; private set; }

    [field: SerializeField] public bool IsDefault { get; private set; } = false;
    [field: SerializeField] public int Priority { get; private set; }


    private void Awake()
    {
        Transform = this.gameObject.transform;
        Targets.Add(this);
    }

    public static HeadTrackingTarget FindNearest(Vector3 requestorPosition, Vector3 requestorForward, float maxDistance, float deg)
    {
        if (Targets.Count == 0) return null;

        return Targets
            .Where(t => !t.IsDefault)
            .Where(t => Vector3.Distance(requestorPosition.NewY(0), t.transform.position.NewY(0f)) < maxDistance)
            .Where(t => IsWithinDegRange(t.Transform.position, requestorPosition, requestorForward, deg))
            .OrderBy(t => Vector3.Distance(requestorPosition, t.transform.position))
            .FirstOrDefault();
    }

    private static bool IsWithinDegRange(Vector3 target, Vector3 requestorPos, Vector3 requestorForward, float deg)
    {
        Vector3 toTarget = (target.NewY(0f) - requestorPos.NewY(0f)).normalized;
        float dotProduct = Vector3.Dot(requestorForward.NewY(0f).normalized, toTarget);
        return dotProduct >= Mathf.Cos(deg * Mathf.Deg2Rad);
    }
}
