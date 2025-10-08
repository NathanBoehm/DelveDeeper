using Managers;
using System;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactable
{
    public class ObjectIteraction : MonoBehaviour, INetworkInitializer
    {
        [SerializeField]
        private CinemachineCamera _playerCamera;

        [SerializeField]
        private TextMeshProUGUI _interactionText;

        [SerializeField]
        private float _interactDistance = 3.0f;

        [SerializeField]
        private float _interactionPopupDelay = 0.1f;

        private bool _interactionDelayRoutineStarted = false;

        private IInteractable _targetedItem;


        private void Start()
        {
            ControlInputManager.Instance.InteractInput.action.performed += HandleInteract;
        }

        private void FixedUpdate()
        {
            //var interactableLayers = LayerMask.GetMask(new string[3] { "Interactable", "NPC", "GameItem" });

            if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit hitInfo, _interactDistance))
            {
                if (hitInfo.transform.TryGetComponent(out IInteractable interactable) && interactable.InteractionEnabled)
                {
                    _interactionDelayRoutineStarted = true;
                    if (_targetedItem != interactable)
                        StartCoroutine(InteractionTextPopupDelay(interactable));
                    _targetedItem = interactable;
                    //_interactionText.text = $"'{ControlInputManager.Instance.InteractInput.action.GetBindingDisplayString()}' - {interactable.HighlightText}";
                    //_targetedItem = interactable;
                }
                else
                {
                    _interactionDelayRoutineStarted = false;
                    StopAllCoroutines();
                    _interactionText.text = "";
                    _targetedItem = null;
                }
            }
            else
            {
                _interactionDelayRoutineStarted = false;
                StopAllCoroutines();
                _interactionText.text = "";
                _targetedItem = null;
            }
        }

        private void HandleInteract(InputAction.CallbackContext context)
        {
            if (_targetedItem == null)
                return;

            _targetedItem.Interact(this.gameObject);
        }

        private IEnumerator InteractionTextPopupDelay(IInteractable ogTarget)
        {
            yield return new WaitForSeconds(_interactionPopupDelay);
            if (_targetedItem != null && ogTarget == _targetedItem)
            {
                _interactionText.text = $"'{ControlInputManager.Instance.InteractInput.action.GetBindingDisplayString()}' - {_targetedItem.HighlightText}";
            }
        }

        public void Initialize()
        {
            this.enabled = false;
        }

        public void InitializeForOwner()
        {
            this.enabled = true;
        }
    }
}