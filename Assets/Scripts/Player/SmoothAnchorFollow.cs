using UnityEngine;

public class SmoothAnchorFollow : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _positionSmoothing = 20f;
    [SerializeField] private float _rotationSmoothing = 20f;

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _cameraTransform.position, Time.deltaTime * _positionSmoothing);
        transform.rotation = Quaternion.Slerp(transform.rotation, _cameraTransform.rotation, Time.deltaTime * _rotationSmoothing);
    }
}
