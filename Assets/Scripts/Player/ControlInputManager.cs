using EditorAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

namespace Managers
{
    public class ControlInputManager : MonoBehaviour
    {
        public static ControlInputManager Instance { get; private set; }

        //Movement Input
        [Required, HideProperty]
        public InputActionReference MovementInput;
        [Required, HideProperty]
        public InputActionReference LookInput;
        [Required, HideProperty]
        public InputActionReference SprintInput;
        [Required, HideProperty]
        public InputActionReference JumpInput;

        //InteractionInput
        [Required, HideProperty]
        public InputActionReference AttackInput;
        [Required, HideProperty]
        public InputActionReference ReadyWeaponInput;
        [Required, HideProperty]
        public InputActionReference InteractInput;


        //UI Controls
        [Required, HideProperty]
        public InputActionReference Navigate;
        [Required, HideProperty]
        public InputActionReference Submit;
        [Required, HideProperty]
        public InputActionReference Cancel;
        [Required, HideProperty]
        public InputActionReference Point;
        [Required, HideProperty]
        public InputActionReference Click;
        [Required, HideProperty]
        public InputActionReference RightClick;
        [Required, HideProperty]
        public InputActionReference MiddleClick;
        [Required, HideProperty]
        public InputActionReference ScrollWheel;

        [SerializeField, TabGroup(nameof(MovementInputs), nameof(InteractionInputs), nameof(UIInputs))]
        private Void PlayerInputsTabGroup;
        [SerializeField, HideInInspector, VerticalGroup(nameof(MovementInput), nameof(LookInput), nameof(SprintInput), nameof(JumpInput))]
        private Void MovementInputs;
        [SerializeField, HideInInspector, VerticalGroup(nameof(AttackInput), nameof(ReadyWeaponInput), nameof(InteractInput))]
        private Void InteractionInputs;
        [SerializeField, HideInInspector, VerticalGroup(nameof(Navigate), nameof(Submit), nameof(Cancel), nameof(Point), nameof(Click), nameof(RightClick), nameof(MiddleClick), nameof(ScrollWheel))]
        private Void UIInputs;

        //UI Input
        //[Required]
        //public InputActionReference InventoryToggleInput;
        //[Required]
        //public InputActionReference UpInput;
        //[Required]
        //public InputActionReference DownInput;
        //[Required]
        //public InputActionReference LeftInput;
        //[Required]
        //public InputActionReference RightInput;
        //[Required]
        //public InputActionReference CancelInput;
        //[Required]
        //public InputActionReference OpenJournalInput;
        //[Required]
        //public InputActionReference MousePointInput;

        //Haggle Input
        //[Required]
        //public InputActionReference HaggleUpInput;
        //[Required]
        //public InputActionReference HaggleDownInput;
        //[Required]
        //public InputActionReference HaggleLeftInput;
        //[Required]
        //public InputActionReference HaggleRightInput;

        //Interaction input
        //[Required] public InputActionReference InteractInput;
        //[Required] public InputActionReference ClickInput;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            SetControlStartState();
        }

        private enum ControlType //split this up for dialog and such so u can't toggle inventory in dialog
        {
            Character,
            UI,
            Haggle
        }

        public void EnableCharacterControls()
        {
            EnableControls(ControlType.Character);
        }

        public void EnableUIControls()
        {
            EnableControls(ControlType.UI);
        }

        public void EnableHaggleControls()
        {
            EnableControls(ControlType.Haggle);
        }

        private void EnableControls(ControlType type)
        {
            if (type == ControlType.Character) MovementInput.action.Enable(); else MovementInput.action.Disable();
            if (type == ControlType.Character) LookInput.action.Enable(); else LookInput.action.Disable();
            if (type == ControlType.Character) SprintInput.action.Enable(); else SprintInput.action.Disable();
            if (type == ControlType.Character) JumpInput.action.Enable(); else JumpInput.action.Disable();
            if (type == ControlType.Character) AttackInput.action.Enable(); else AttackInput.action.Disable();
            if (type == ControlType.Character) ReadyWeaponInput.action.Enable(); else ReadyWeaponInput.action.Disable();
            //if (type == ControlType.Character) InteractInput.action.Enable(); else InteractInput.action.Disable();
            //if (type == ControlType.Character) ClickInput.action.Enable(); else ClickInput.action.Disable();
            if (type == ControlType.Character)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (type == ControlType.UI) Navigate.action.Enable(); else Navigate.action.Disable();
            if (type == ControlType.UI) Submit.action.Enable(); else Submit.action.Disable();
            if (type == ControlType.UI) Cancel.action.Enable(); else Cancel.action.Disable();
            if (type == ControlType.UI) Point.action.Enable(); else Point.action.Disable();
            if (type == ControlType.UI) Click.action.Enable(); else Click.action.Disable();
            if (type == ControlType.UI) RightClick.action.Enable(); else RightClick.action.Disable();
            if (type == ControlType.UI) MiddleClick.action.Enable(); else MiddleClick.action.Disable();
            if (type == ControlType.UI) ScrollWheel.action.Enable(); else ScrollWheel.action.Disable();

            /*if (type == ControlType.UI) UpInput.action.Enable(); else UpInput.action.Disable();
            if (type == ControlType.UI) DownInput.action.Enable(); else DownInput.action.Disable();
            if (type == ControlType.UI) LeftInput.action.Enable(); else UpInput.action.Disable();
            if (type == ControlType.UI) RightInput.action.Enable(); else DownInput.action.Disable();
            if (type == ControlType.UI)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            
            if (type == ControlType.Haggle)
            { 
                HaggleUpInput.action.Enable();
                HaggleDownInput.action.Enable();
                HaggleLeftInput.action.Enable();
                HaggleRightInput.action.Enable();

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                HaggleUpInput.action.Disable();
                HaggleDownInput.action.Disable();
                HaggleLeftInput.action.Disable();
                HaggleRightInput.action.Disable();
            }*/
        }

        private void SetControlStartState()
        {
            //always enabled
            //InventoryToggleInput.action.Enable();
            //CancelInput.action.Enable();
            //ClickInput.action.Enable(); //FIX THIS - what did I mean by fix this?
            //OpenJournalInput.action.Enable();

            //UpInput.action.Disable(); 
            //DownInput.action.Disable();

            EnableCharacterControls();
        }
    }
}