using EditorAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AnimationController : MonoBehaviour
{
    [SerializeField]  TwoBoneIKConstraint   _rightArmIK;
    [SerializeField]  Animator              _animator;

    //Vector3(-0.3,-0.3,0.15)
    //Quaternion(-0.912862003,0.0115841515,0.404512763,-0.0540200658)
    //Quaternion(0.905445218,0.018434355,-0.422215998,0.0395326056)
    [SerializeField] Transform _handTarget;
    Vector3 _startingHandTargetPos;
    Quaternion _startingHandTargetOrientation;

    [SerializeField] ProceduralAttackAnimation _swingLeftPrep;
    [SerializeField] ProceduralAttackAnimation _swingLeft;
    [SerializeField] ProceduralAttackAnimation _swingLeftReset;


    private void Start()
    { 
        _startingHandTargetPos = _handTarget.localPosition;
        _startingHandTargetOrientation = _handTarget.localRotation;
    }

    void Update()
    {

    }

    public void Attack()
    {
        StartCoroutine(SwingLeft());
    }

    private IEnumerator SwingLeft()
    {
        Func<IEnumerator> leftSwingReset = () => ProceduralAttackAnimation(_swingLeftReset, _swingLeft.EndPos, Quaternion.Euler(_swingLeft.EndEulerOrientation));
        Func<IEnumerator> leftSwing = () => ProceduralAttackAnimation(_swingLeft, _swingLeftPrep.EndPos, Quaternion.Euler(_swingLeftPrep.EndEulerOrientation), leftSwingReset);
        StartCoroutine(ProceduralAttackAnimation(_swingLeftPrep, _startingHandTargetPos, _startingHandTargetOrientation, leftSwing));

        yield return null;
    }

    [Button]
    private void SetStartPose()
    {
        _handTarget.SetLocalPositionAndRotation(_swingLeftPrep.EndPos, Quaternion.Euler(_swingLeftPrep.EndEulerOrientation));
    }

    [Button]
    private void SetEndPose()
    {
        _handTarget.SetLocalPositionAndRotation(_swingLeft.EndPos, Quaternion.Euler(_swingLeft.EndEulerOrientation));
    }

    //Vector3(0.286000013,-0.268999994,0.316000015)
    //Vector3(344.884552,351.914459,188.822525)

    private IEnumerator ProceduralAttackAnimation(ProceduralAttackAnimation anim, Vector3 startPos, Quaternion startRot, Func<IEnumerator> followUp = null)
    {
        var elapsedTime = 0f;
        var endRot = Quaternion.Euler(anim.EndEulerOrientation);

        while (elapsedTime < anim.Length)
        {
            var animCompletion = elapsedTime / anim.Length;
            var interp = (anim.Curve != null) ? anim.Curve.Evaluate(animCompletion) : Mathf.SmoothStep(0, 1, animCompletion);
            _handTarget.SetLocalPositionAndRotation(
                Vector3.Lerp(startPos, anim.EndPos, interp),
                Quaternion.Slerp(startRot, endRot, interp));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(anim.EndDelay);
        if (followUp != null) StartCoroutine(followUp());
    }
}

[Serializable]
public class ProceduralAttackAnimation
{
    [field: SerializeField] public Vector3 EndPos { get; private set; }
    [field: SerializeField] public Vector3 EndEulerOrientation { get; private set; }

    [field: SerializeField] public float Length { get; private set; } = 0.3f;
    [field: SerializeField] public AnimationCurve Curve {  get; private set; }
    [field: SerializeField] public float EndDelay { get; private set; } = 0f;
}
