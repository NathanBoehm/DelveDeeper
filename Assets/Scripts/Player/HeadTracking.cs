using EditorAttributes;
using System.Collections;
using UnityEngine;

public class HeadTracking : MonoBehaviour
{
    [SerializeField] float _maxLookLength = 2.0f;
    [SerializeField] float _maxLookDeg = 140f;
    [SerializeField] float _bodyWeight = 0.1f;
    [SerializeField] float _headWeight = 0.4f;
    [SerializeField] float _eyesWeight = 1.0f;
    [SerializeField] float _clampWeight = 0.5f;
    [SerializeField] float _pollingInterval_s = 1f;
    [SerializeField] float _maxHTTDistance = 5f;
    [SerializeField, Required] Animator _animator;


    [SerializeField, Required] HeadTrackingTarget _defaultTarget;

    //targetAcquired -> lerp between _startingLook and _destinationLook to set _currentLook
    //on _destinationLook update -> _startingLook = _currentLook;

    HeadTrackingTarget _currentTarget;
    Vector3 _startingLook;
    Vector3 _currentLook;
    Vector3 _destinationLook;

    private float _elapsedTime = 0f;
    private float _lookLength = 0f;

    private float MaxLookAngle => (_bodyWeight + _headWeight) * _maxLookDeg;


    private float LookDirectionDegrees(Vector3 target) 
    {
        Vector3 lookDirection = target - this.gameObject.transform.position;
        return Vector3.Dot(this.gameObject.transform.forward, lookDirection) * Mathf.Rad2Deg;
    } 

    private void Start()
    {
        StartCoroutine(TargetUpdateLoop());
    }

    private IEnumerator TargetUpdateLoop()
    {
        while (true)
        {
            var target = HeadTrackingTarget.FindNearest(
                this.gameObject.transform.position,
                this.gameObject.transform.forward,
                _maxHTTDistance,
                MaxLookAngle);

            if (target == null)
            {
                Debug.Log("Target was null");
                target = _defaultTarget;
            }

            //if (target != _currentTarget)
            {
                //Debug.Log($"new target: {target.name}");
                _currentTarget = target;
            }

            yield return new WaitForSeconds(_pollingInterval_s);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        SetTarget(_currentTarget.transform.position);
        _elapsedTime += Time.deltaTime;
        _currentLook = Vector3.Lerp(_startingLook, _destinationLook, _elapsedTime / _lookLength);
    }

    private void SetTarget(Vector3 target)
    {
        _destinationLook = target;
        _startingLook = _currentLook;

        _elapsedTime = 0f;
        var startingLookDirection = _startingLook - this.gameObject.transform.position;
        var destinationLookDirection = _destinationLook - this.gameObject.transform.position;
        var lookDirectionChangeDeg = Vector3.Dot(startingLookDirection, destinationLookDirection) * Mathf.Rad2Deg;

        //var lookDirectionChangeDeg = Mathf.Abs(LookDirectionDegrees(_destinationLook) - LookDirectionDegrees(_startingLook));
        if (lookDirectionChangeDeg > 3) Debug.Log($"deg change: {lookDirectionChangeDeg}");
        _lookLength = _maxLookLength * (lookDirectionChangeDeg / _maxLookDeg);
        if (lookDirectionChangeDeg > 3) Debug.Log($"anim length: {_lookLength}");
    }

    private void OnAnimatorIK(int layerIndex)
    {
        //Debug.Log(_currentLook);
        _animator.SetLookAtPosition(_currentLook);
        _animator.SetLookAtWeight(1, _bodyWeight, _headWeight, _eyesWeight, _clampWeight);
    }
}
