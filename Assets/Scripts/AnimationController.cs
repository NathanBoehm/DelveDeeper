using EditorAttributes;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private TwoBoneIKConstraint _rightArmIK;
    [SerializeField] private float _blendSpeed = 5f;
    [SerializeField] private Animator _animator;
    [ShowInInspector]
    private float _targetWeight = 1f;
    private int _attackStateHash;

    private void Start()
    {
        _attackStateHash = Animator.StringToHash("Attack");
    }

    void Update()
    {
        //if (_targetWeight == 0f)
            _rightArmIK.weight = _targetWeight;// Mathf.Lerp(_rightArmIK.weight, _targetWeight, Time.deltaTime * _blendSpeed);
        //else
            //Mathf.Lerp(_rightArmIK.weight, _targetWeight, Time.deltaTime * _blendSpeed);
    }

    public void SetArmIKEnabled(bool enabled)
    {
        _targetWeight = enabled ? 1f : 0f;
    }

    public void Attack()
    {
        //_animator.Update(0f);
        //SetArmIKEnabled(false);
        _animator.SetTrigger("Attack");
    }
}
