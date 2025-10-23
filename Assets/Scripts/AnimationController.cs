using EditorAttributes;
using NUnit;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AnimationController : MonoBehaviour
{
    enum State
    {
        Idle,
        SwingLeft,
        SwingRight,
        SwingLeftDown
    }

    [SerializeField] TwoBoneIKConstraint _rightArmIK;
    [SerializeField] Transform _handTarget;
    [SerializeField] Animator _animator;
    [SerializeField, Required] PlayerCameraManager _playerCameraManager;

    [SerializeField] ProceduralAttackAnimation _swingLeftPrep;
    [SerializeField] ProceduralAttackAnimation _swingLeft;
    [SerializeField] ProceduralAttackAnimation _swingLeftReset;

    [SerializeField] ProceduralAttackAnimation _swingRightPrep;
    [SerializeField] ProceduralAttackAnimation _swingRightIntermediate;
    [SerializeField] ProceduralAttackAnimation _swingRight;
    [SerializeField] ProceduralAttackAnimation _swingRightReset;

    [SerializeField] ProceduralAttackAnimation _swingLeftDownPrep;
    [SerializeField] ProceduralAttackAnimation _swingLeftDown;
    [SerializeField] ProceduralAttackAnimation _swingLeftDownReset;

    private State _state = State.Idle;
    private Func<IEnumerator> _reset = null;
    private Queue<Func<IEnumerator>> _animActions = new Queue<Func<IEnumerator>>();
    private bool _nextAttackQueued = false;

    private void Start()
    {

    }

    void Update()
    {

    }

    public void Attack()
    {
        if (_nextAttackQueued) //only allow the player to queue one attack
            return;

        switch(_state)
        {
            case State.Idle:
                SwingLeft();
                break;
            case State.SwingLeft:
                SwingRight();
                break;
            case State.SwingRight:
                SwingLeftDown();
                break;
            case State.SwingLeftDown:
                SwingLeft();
                break;
            default:
                throw new Exception($"Invalid attack state {_state}");
        }
    }

    public void Walk()
    {
        _playerCameraManager.PlayWalkNoise();
        _animator.SetBool("Walking", true);
    }

    public void StopWalk()
    {
        _playerCameraManager.StopWalkNoise();
        _animator.SetBool("Walking", false);
    }

    private void SwingRight()
    {
        _state = State.SwingRight;

        Func<IEnumerator> rightSwingReset = () => ProceduralAttackAnimation(_swingRightReset, _playerCameraManager.PlayMainCamera);
        Func<IEnumerator> rightSwing = () => ProceduralAttackAnimation(_swingRight, _playerCameraManager.PlaySwingRightCamera);
        Func<IEnumerator> rightSwingIntermediate = () => ProceduralAttackAnimation(_swingRightIntermediate);
        Func<IEnumerator> rightSwingPrep = () => ProceduralAttackAnimation(_swingRightPrep, _playerCameraManager.PlaySwingRightPrepCamera);

        if (_animActions.Count > 0)
        {
            //attack anims in progress, add attack to queue - don't start immeadiately
            _animActions.Enqueue(rightSwingPrep);
            _nextAttackQueued = true;
        }
        else
        {
            StopAllCoroutines(); //stop any reset animation happening
            StartCoroutine(rightSwingPrep());
        }

        _animActions.Enqueue(rightSwingIntermediate);
        _animActions.Enqueue(rightSwing);

        _reset = rightSwingReset;

    }

    private void SwingLeft()
    {
        _state = State.SwingLeft;

        Func<IEnumerator> leftSwingReset = () => ProceduralAttackAnimation(_swingLeftReset, _playerCameraManager.PlayMainCamera);
        Func<IEnumerator> leftSwing = () => ProceduralAttackAnimation(_swingLeft, _playerCameraManager.PlaySwingLeftCamera);
        Func<IEnumerator> leftSwingPrep = () => ProceduralAttackAnimation(_swingLeftPrep, _playerCameraManager.PlaySwingLeftPrepCamera);

        if (_animActions.Count > 0)
        {
            //attack anims in progress, add attack to queue - don't start immeadiately
            _animActions.Enqueue(leftSwingPrep);
            _nextAttackQueued = true;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(leftSwingPrep());
        }

        _animActions.Enqueue(leftSwing);
        _reset = leftSwingReset;
    }

    private void SwingLeftDown()
    {
        _state = State.SwingLeftDown;

        Func<IEnumerator> leftDownSwingReset = () => ProceduralAttackAnimation(_swingLeftDownReset, _playerCameraManager.PlayMainCamera);
        Func<IEnumerator> leftDownSwing = () => ProceduralAttackAnimation(_swingLeftDown, _playerCameraManager.PlaySwingDownLeftCamera);
        Func<IEnumerator> leftDownSwingPrep = () => ProceduralAttackAnimation(_swingLeftDownPrep, _playerCameraManager.PlaySwingDownLeftPrepCamera);

        if (_animActions.Count > 0)
        {
            //attack anims in progress, add attack to queue - don't start immeadiately
            _animActions.Enqueue(leftDownSwingPrep);
            _nextAttackQueued = true;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(leftDownSwingPrep());
        }

        _animActions.Enqueue(leftDownSwing);
        _reset = leftDownSwingReset;
    }

    [Button]
    private void SetStartPose()
    {
        _handTarget.SetLocalPositionAndRotation(_swingRightPrep.EndTransform.localPosition, _swingLeftPrep.EndTransform.localRotation);
    }

    [Button]
    private void SetEndPose()
    {
        _handTarget.SetLocalPositionAndRotation(_swingRight.EndTransform.localPosition, _swingLeft.EndTransform.localRotation);
    }

    //Vector3(0.286000013,-0.268999994,0.316000015)
    //Vector3(344.884552,351.914459,188.822525)

    private IEnumerator ProceduralAttackAnimation(ProceduralAttackAnimation anim, Action effect = null)
    {
        var startPos = _handTarget.localPosition;
        var startRot = _handTarget.localRotation;

        var elapsedTime = 0f;
        var endRot = anim.EndTransform.localRotation;

        if (anim.IsAnimStart)
            _nextAttackQueued = false;

        effect?.Invoke();

        while (elapsedTime < anim.Length)
        {
            var animCompletion = elapsedTime / anim.Length;
            var interp = (anim.Curve != null) ? anim.Curve.Evaluate(animCompletion) : Mathf.SmoothStep(0, 1, animCompletion);

            _handTarget.SetLocalPositionAndRotation(
                    Vector3.Lerp(startPos, anim.EndTransform.localPosition, interp),
                    Quaternion.Slerp(startRot, endRot, interp));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(anim.EndDelay);

        if (_animActions.Count > 0)
        {
            var nextPhase = _animActions.Dequeue();
            StartCoroutine(nextPhase());
        }
        else if (_reset != null)
        {
            StartCoroutine(_reset());
            _reset = null;
        }
        else
        {
            _state = State.Idle;
        }
    }

    /*private IEnumerator ProceduralAttackAnimation(ProceduralAttackAnimation anim, Vector3 startPos, Quaternion startRot, Func<IEnumerator> followUp = null)
    {
        var elapsedTime = 0f;

        var endRot = anim.EndTransform.localRotation;


        // > 179 deg rotations will make the quaternion slerp negative
        //so split the rotation into two smaller chunks
        float totalAngleYRotation = Mathf.Abs(NormalizeAngle(endRot.eulerAngles.y) - NormalizeAngle(startRot.eulerAngles.y));
        float splitPointDegrees = 179;

        Debug.Log($"end rot y: {NormalizeAngle(endRot.eulerAngles.y)}, start rot {NormalizeAngle(startRot.eulerAngles.y)}, totalAngleRot: {totalAngleYRotation}");

        if (anim.RotateClockwise) //&& totalAngleYRotation > splitPointDegrees)
        {
            Debug.Log("split requried");

            var phaseOneRotationY = splitPointDegrees;
            //var phaseTwoRotationY = (endRot.eulerAngles.y - anim.EndTransform.localRotation.eulerAngles.y) % splitPointDegrees;

            var phaseOnePercentageOfAnim = phaseOneRotationY / totalAngleYRotation;
            //var phaseTwoPercentageOfAnim = phaseTwoRotationY / totalAngleYRotation;

            var phaseOneRotation = Quaternion.Euler(endRot.eulerAngles.x * phaseOnePercentageOfAnim, phaseOneRotationY, startRot.eulerAngles.z * phaseOnePercentageOfAnim);
            //var phaseTwoRotation = endRot;

            var phaseOneLength = anim.Length * phaseOnePercentageOfAnim;
            //var phaseTwoLength = anim.Length * phaseTwoPercentageOfAnim;

            var startEuler = startRot.eulerAngles;
            var endEuler = endRot.eulerAngles;
            if (endEuler.y < startEuler.y)
                endEuler.y += 360f;

            while (elapsedTime < anim.Length)
            {
                var animCompletion = elapsedTime / anim.Length;
                var interp = (anim.Curve != null) ? anim.Curve.Evaluate(animCompletion) : Mathf.SmoothStep(0, 1, animCompletion);

                var slerpedVector = Vector3.Slerp(startEuler, endEuler, interp);
                //Debug.Log($"vector slerp step: {slerpedVector}");
                _handTarget.SetLocalPositionAndRotation(
                    Vector3.Lerp(startPos, anim.EndTransform.localPosition, interp),
                    Quaternion.Euler(slerpedVector));
                //elapsedTime < phaseOneLength ?
                //Quaternion.Slerp(startRot, phaseOneRotation, interp) :
                //Quaternion.Slerp(phaseOneRotation, endRot, interp));

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            while (elapsedTime < anim.Length)
            {
                var animCompletion = elapsedTime / anim.Length;
                var interp = (anim.Curve != null) ? anim.Curve.Evaluate(animCompletion) : Mathf.SmoothStep(0, 1, animCompletion);

                _handTarget.SetLocalPositionAndRotation(
                        Vector3.Lerp(startPos, anim.EndTransform.localPosition, interp),
                        Quaternion.Slerp(startRot, endRot, interp));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        yield return new WaitForSeconds(anim.EndDelay);
        if (followUp != null) StartCoroutine(followUp());
    }*/

    // Helper method to normalize Euler angles to a consistent 0-360 range
    private Vector3 NormalizeEulerAngles(Vector3 euler)
    {
        return new Vector3(
            NormalizeAngle(euler.x),
            NormalizeAngle(euler.y),
            NormalizeAngle(euler.z)
        );
    }

    // Helper method to normalize a single angle to 0-360 range
    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0f)
            angle += 360f;
        return angle;
    }
}

[Serializable]
public class ProceduralAttackAnimation
{
    [field: SerializeField] public Transform EndTransform { get; private set; }
    [field: SerializeField] public float Length { get; private set; } = 0.3f;
    [field: SerializeField] public AnimationCurve Curve { get; private set; }
    [field: SerializeField] public float EndDelay { get; private set; } = 0f;
    [field: SerializeField] public bool IsAnimStart { get; private set; } = false;
}