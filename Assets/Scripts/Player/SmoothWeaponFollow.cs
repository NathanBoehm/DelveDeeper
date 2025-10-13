using UnityEngine;

public class SmoothWeaponFollow : MonoBehaviour
{
    [SerializeField] private Transform _anchorTarget;
    [SerializeField] private float _smoothing = 30f;

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _anchorTarget.position, Time.deltaTime * _smoothing);
        transform.rotation = Quaternion.Slerp(transform.rotation, _anchorTarget.rotation, Time.deltaTime * _smoothing);
    }
}
