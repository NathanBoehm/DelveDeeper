using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Managers;
using System;
using EditorAttributes;
using System.Collections;


namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour, INetworkObjectInitializer
    {
        //Required Components
        [SerializeField, Required, HideProperty] private CharacterController _characterController;
        [SerializeField, Required, HideProperty] private CinemachineCamera _playerCamera;
        [SerializeField, Required, HideProperty] private GameObject _playerGameObject;
        [SerializeField, Required, HideProperty] private GameObject _cameraTrackingTarget;
        [SerializeField, Required, HideProperty] private Animator _animator;
        [SerializeField, Required, HideProperty] private AnimationController _animationController;
        [SerializeField, FoldoutGroup("Requried Components", nameof(_characterController), nameof(_playerCamera), nameof(_playerGameObject), nameof(_cameraTrackingTarget), nameof(_animator), nameof(_animationController))] private EditorAttributes.Void RequiredComponentsGroup;


        //Grounded Movement Fields
        public float CurrentSpeed { get; private set; } = 0.0f;

        [SerializeField, HideProperty] private float _maxWalkSpeed = 5.0f;
        [SerializeField, HideProperty] private float _maxSprintSpeed = 10.0f;
        [SerializeField, HideProperty] private float _acceleration = 5.0f;

        private bool _isDashing = false;
        private Vector3 _dashDirection = Vector3.zero;
        [SerializeField, HideProperty]
        private float _dashSpeed = 25f;
        [SerializeField, HideProperty]
        private float _dashLength = 0.25f;

        [SerializeField, FoldoutGroup("Movement", nameof(_maxWalkSpeed), nameof(_maxSprintSpeed), nameof(_acceleration), nameof(_dashSpeed), nameof(_dashLength))]
        private EditorAttributes.Void MovementFielsGroup;

        private float _currentSpeedForward = 0.0f;
        private float _currentSpeedRight = 0.0f;

        [SerializeField, HideProperty]//Tooltip("Minimum Rotation Speed")
        private float _baseCameraRotFollowSpeed = 8f;
        [SerializeField, HideProperty]//Tooltip("How much faster rotation speed can be per degree of difference in camera rotation speed")
        private float _rotationSpeedScaling = 0.5f;
        [SerializeField, HideProperty]//Tooltip("Maximum player object rotation speed")
        private float _maxCameraRotFollowSpeed = 30f;

        [SerializeField, FoldoutGroup("Camera Rotation Follow Speed", nameof(_baseCameraRotFollowSpeed), nameof(_rotationSpeedScaling), nameof(_maxCameraRotFollowSpeed))]
        private EditorAttributes.Void RotationFieldsGroup;

        //Jump Movement Fields
        public Vector3 JumpDirection { get; private set; } = Vector3.zero;
        public bool IsJumping { get; private set; } = false;

        private const float TERMINAL_VELOCITY = -50.0f;

        [SerializeField]
        private float _gravity = -10.0f;
        [SerializeField]
        private float _jumpHeight = 1.0f;

        private float _verticalVelocity;

        public event Action PlayerLanded;
        public event Action PlayerJumped;

        private void Awake()
        {
            if (_characterController == null)
                _characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            ControlInputManager.Instance.ReadyWeaponInput.action.performed += ReadyWeapon;
            ControlInputManager.Instance.AttackInput.action.performed += Attack;
            ControlInputManager.Instance.SprintInput.action.performed += Dash;
        }

        private void Dash(InputAction.CallbackContext context)
        {
            var inputs = ControlInputManager.Instance.MovementInput.action.ReadValue<Vector2>();
            if (_isDashing || inputs.y != 0)
                return;

            _isDashing = true;
            _dashDirection = inputs.x > 0 ? _playerCamera.transform.right * _dashSpeed : _playerCamera.transform.right * _dashSpeed * -1;
            StartCoroutine(ResetDash());
        }

        private IEnumerator ResetDash()
        {
            yield return new WaitForSeconds(_dashLength);
            _isDashing = false;
        }

        private void ReadyWeapon(InputAction.CallbackContext context)
        {
            _animator.SetBool("WeaponReady", !_animator.GetBool("WeaponReady"));
        }

        private void Attack(InputAction.CallbackContext context)
        {
            _animationController.Attack();
        }

        private void Update()
        {
            MovePlayer();
            JumpAndFall();
        }

        private void LateUpdate()
        {
            AlignPlayerWithCamera();
        }

        private void AlignPlayerWithCamera()
        {
            //remove y to prevent player from tilting
            var normalizedCameraForward = _playerCamera.transform.forward.NewY(0).normalized;
            var targetRot = Quaternion.LookRotation(normalizedCameraForward);
            _playerGameObject.transform.position = (_playerCamera.transform.position - normalizedCameraForward * 0.15f).NewY(_playerGameObject.transform.position.y);

            float angleDiff = Quaternion.Angle(_playerGameObject.transform.rotation, targetRot);
            float playerRotationSpeed = Mathf.Min(_baseCameraRotFollowSpeed + angleDiff * _rotationSpeedScaling, _maxCameraRotFollowSpeed);

            _playerGameObject.transform.rotation = Quaternion.Slerp(_playerGameObject.transform.rotation, targetRot, Time.deltaTime * 10);
        }

        private float GetPitch(Vector3 forward)
        {
            // Pitch is the angle up/down relative to the horizontal plane
            Vector3 flatForward = new Vector3(forward.x, 0, forward.z).normalized;
            float pitch = Vector3.SignedAngle(flatForward, forward, _playerCamera.transform.right);
            return pitch;
        }

        private void MovePlayer()
        {
            Vector2 movementInputs = ControlInputManager.Instance.MovementInput.action.ReadValue<Vector2>();

            float targetSpeedForward = CalculateTargetForwardSpeed(movementInputs.y);
            float targetSpeedRight = CalculateTargetHorizontalSpeed(movementInputs.x);

            CalculateCurrentSpeed(targetSpeedForward, ref _currentSpeedForward);
            CalculateCurrentSpeed(targetSpeedRight, ref _currentSpeedRight);

            Vector3 moveDirection;

            if (!IsJumping)
            {
                Vector3 inputDirection = new Vector3(movementInputs.x, 0.0f, movementInputs.y).normalized;

                if (movementInputs != Vector2.zero)
                {
                    var cameraForward = _playerCamera.transform.forward;
                    cameraForward.y = 0.0f;
                    var cameraRight = _playerCamera.transform.right;
                    cameraRight.y = 0.0f;

                    inputDirection = cameraRight.normalized * _currentSpeedRight + cameraForward.normalized * _currentSpeedForward;
                    CurrentSpeed = inputDirection.magnitude;
                    _animator.SetBool("Walking", true);
                }
                else
                {
                    CurrentSpeed = 0; 
                    _animator.SetBool("Walking", false);
                }

                JumpDirection = inputDirection; //preserve current direction in case player jumps this frame

                moveDirection = inputDirection * Time.deltaTime + (new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
                //_characterController.Move(inputDirection * Time.deltaTime + (new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime));
            }
            else
            {
                CurrentSpeed = 0;
                moveDirection = JumpDirection * Time.deltaTime + (new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
                //_characterController.Move(JumpDirection * Time.deltaTime + (new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime));
                _animator.SetBool("Walking", false);
            }

            if (_isDashing)
            {
                moveDirection = _dashDirection * Time.deltaTime + (new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
                _animator.SetBool("Walking", false);
            }


            _characterController.Move(moveDirection);
        }

        private float CalculateTargetHorizontalSpeed(float xInput)
        {
            if (xInput.IsClose(0f)) return 0f;

            if (_characterController.isGrounded)
                return _maxWalkSpeed * xInput;
            else
                return _currentSpeedRight;
        }

        private float CalculateTargetForwardSpeed(float yInput)
        {
            if (yInput.IsClose(0f)) return 0.0f;

            if (_characterController.isGrounded)
                return (ControlInputManager.Instance.SprintInput.action.IsPressed() && yInput > 0 ? _maxSprintSpeed : _maxWalkSpeed) * yInput;
            else
                return _currentSpeedForward;
        }

        private void CalculateCurrentSpeed(float targetSpeed, ref float currentSpeed)
        {
            if (currentSpeed != targetSpeed)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * _acceleration);
                currentSpeed = Mathf.Round(currentSpeed * 1000f) / 1000f;

                //remove extra delay when changing direction
                if ((currentSpeed < 0 && targetSpeed > 0) || (currentSpeed > 0 && targetSpeed < 0))
                {
                    currentSpeed = currentSpeed * -0.5f;
                }
            };
        }

        private void JumpAndFall()
        {
            if (_characterController.isGrounded)
            {
                _verticalVelocity = 0.0f;

                if (ControlInputManager.Instance.JumpInput.action.inProgress)
                {
                    //characterController.isGrounded seems to be unreliable when the player in on the ground
                    //but is always accurate when the player is in the air
                    //if (!IsJumping) -- shouldn't need this we have the is grounded check
                        PlayerJumped?.Invoke();
                    IsJumping = true;
                    _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                }
                else
                {
                    if (IsJumping)
                        PlayerLanded?.Invoke();
                    IsJumping = false;
                }
            }
            else
            {
                //apply gravity for downward velocity
                if (Mathf.Abs(_verticalVelocity) < Mathf.Abs(TERMINAL_VELOCITY))
                    _verticalVelocity += _gravity * Time.deltaTime;
            }
        }

        public void Initialize()
        {
            this.enabled = false;
            _playerCamera.enabled = false;
        }

        public void InitializeForOwner()
        {
            this.enabled = true;
            _playerCamera.enabled = true;
        }
    }

    public static class CustomExtensions
    {
        public static readonly float MaxCloseDiff = 0.0001f;
        public static bool IsClose(this float actual, float expected)
        {
            return Mathf.Abs(actual - expected) <= MaxCloseDiff;
        }
    }
}
