using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Managers;
using System;


namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        //Required Components
        [SerializeField]
        private CharacterController _characterController;

        [SerializeField]
        private CinemachineCamera _playerCamera;


        //Grounded Movement Fields
        [SerializeField]
        private float _maxWalkSpeed = 8.0f;

        [SerializeField]
        private float _maxSprintSpeed = 15.0f;

        [SerializeField]
        private float _acceleration = 10.0f;

        private float _currentSpeedForward = 0.0f;
        public float CurrentSpeedForward { get { return _currentSpeedForward; } private set { _currentSpeedForward = value; } }

        private float _currentSpeedRight = 0.0f;
        public float CurrentSpeedRight { get { return _currentSpeedRight; } private set { _currentSpeedRight = value; } }

        public float CurrentSpeed { get; private set; } = 0.0f;


        //Jump Movement Fields
        [SerializeField]
        private float _gravity = -10.0f;

        [SerializeField]
        private float _jumpHeight = 2.0f;

        public Vector3 JumpDirection { get; private set; } = Vector3.zero;

        public bool IsJumping { get; private set; } = false;

        private float _verticalVelocity;

        private const float TERMINAL_VELOCITY = -50.0f;

        private bool GhostInputsHandled = false;


        public event Action PlayerLanded;
        public event Action PlayerJumped;

        private void OnEnable()
        {
            //ControlInputManagerV2.Instance.EnableCharacterControls();
        }

        private void OnDisable()
        {
            //ControlInputManagerV2.Instance.EnableUIControls();
        }

        private void Awake()
        {
            if (_characterController == null)
                _characterController = GetComponent<CharacterController>();

            if (_playerCamera == null)
                throw new MissingReferenceException("Character controller missing playe camera.");

            //MovePlayerAlias = MovePlayerV2;
        }

        // Start is called before the first frame update
        private void Start()
        {
            InputSystem.ResetDevice(Keyboard.current);
            InputSystem.ResetDevice(Joystick.current);
        }

        // Update is called once per frame
        private void Update()
        {
            //MovePlayerAlias();
            MovePlayer();
            JumpAndFall();
        }

        private void LateUpdate()
        {
        }

        /*private void MovePlayer()
        {
            Vector2 movementInputs = ControlInputManagerV2.Instance.MovementInput.action.ReadValue<Vector2>();

            if (!GhostInputsHandled)
            {
                InputSystem.ResetDevice(Keyboard.current);
                InputSystem.ResetDevice(Joystick.current);
                GhostInputsHandled = true;
            }

            float targetSpeed;
            if (_characterController.isGrounded)
                targetSpeed = ControlInputManagerV2.Instance.SprintInput.action.IsPressed() ? _maxSprintSpeed : _maxWalkSpeed;
            else
                targetSpeed = CurrentSpeed;

            Vector3 inputDirection = new Vector3(movementInputs.x, 0.0f, movementInputs.y).normalized;

            if (movementInputs != Vector2.zero)
            {
                var cameraForward = _playerCamera.transform.forward;
                cameraForward.y = 0.0f;
                var cameraRight = _playerCamera.transform.right;
                cameraRight.y = 0.0f;

                inputDirection = cameraRight.normalized * movementInputs.x + cameraForward.normalized * movementInputs.y;
            }
            else
            {
                targetSpeed = 0.0f;
            }

            if (CurrentSpeed != targetSpeed)
            {
                CurrentSpeed = Mathf.Lerp(CurrentSpeed, targetSpeed, Time.deltaTime * _acceleration);
                CurrentSpeed = Mathf.Round(CurrentSpeed * 1000f) / 1000f;
                //CurrentSpeed += _acceleration * Time.deltaTime * (CurrentSpeed < targetSpeed ? 1 : -1);
            }

            _characterController.Move(inputDirection.normalized * CurrentSpeed * Time.deltaTime + (new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime));
        }*/

        private void MovePlayer()
        {
            Vector2 movementInputs = ControlInputManager.Instance.MovementInput.action.ReadValue<Vector2>();
            Debug.Log(movementInputs);

            float targetSpeedForward = CalculateTargetForwardSpeed(movementInputs.y);
            float targetSpeedRight = CalculateTargetHorizontalSpeed(movementInputs.x);

            CalculateCurrentSpeed(targetSpeedForward, ref _currentSpeedForward);
            CalculateCurrentSpeed(targetSpeedRight, ref _currentSpeedRight);

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
                }
                else
                {
                    CurrentSpeed = 0;
                }

                JumpDirection = inputDirection; //preserve current direction in case player jumps this frame

                _characterController.Move(inputDirection * Time.deltaTime + (new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime));
            }
            else
            {
                CurrentSpeed = 0;
                _characterController.Move(JumpDirection * Time.deltaTime + (new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime));
            }
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

        /*public void ToggleMovementType()
        {
            if (MovePlayerAlias == MovePlayer)
            {
                MovePlayerAlias = MovePlayerV2;
                Debug.Log("Movement Function changed to V2");
            }
            else
            {
                MovePlayerAlias = MovePlayer;
                Debug.Log("Movement Function changed to V1");
            }
        }*/
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
