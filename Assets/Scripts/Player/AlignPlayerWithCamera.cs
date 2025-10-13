using Unity.Cinemachine;
using UnityEngine;

[DefaultExecutionOrder(110)]
public class AlignPlayerWithCamera : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _playerCamera;
    [SerializeField] private GameObject _playerGameObject;

    [SerializeField] private float _baseCameraRotFollowSpeed;
    [SerializeField] private float _rotationSpeedScaling;
    [SerializeField] private float _maxCameraRotFollowSpeed;

    private void LateUpdate()
    {
        var normalizedCameraForward = _playerCamera.transform.forward.NewY(0).normalized;
        var targetRot = Quaternion.LookRotation(normalizedCameraForward);
        _playerGameObject.transform.position = (_playerCamera.transform.position - normalizedCameraForward * 0.15f).NewY(_playerGameObject.transform.position.y);

        float angleDiff = Quaternion.Angle(_playerGameObject.transform.rotation, targetRot);
        float playerRotationSpeed = Mathf.Min(_baseCameraRotFollowSpeed + angleDiff * _rotationSpeedScaling, _maxCameraRotFollowSpeed);

        _playerGameObject.transform.rotation = Quaternion.Slerp(_playerGameObject.transform.rotation, targetRot, Time.deltaTime * 10);
    }
}
