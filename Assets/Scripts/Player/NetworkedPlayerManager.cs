using UnityEngine;
using Unity.Netcode;
using Player;
using Unity.Cinemachine;
using Interactable;
using Managers;

public class NetworkedPlayerManager : NetworkBehaviour
{
    [SerializeField] PlayerController _controller;
    [SerializeField] CinemachineCamera _playerCamera;
    [SerializeField] ObjectIteraction _objectInteraction;

    [SerializeField] GameObject _UI;

    private INetworkInitializer[] _networkInitializers;

    private void Awake()
    {
        _networkInitializers = this.gameObject.GetComponents<INetworkInitializer>();
        _networkInitializers.ForEach(i => i.Initialize());
        //_controller.enabled = false;
        //_playerCamera.enabled = false;
        //_objectInteraction.enabled = false;
        _UI.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            ControlInputManager.Instance.EnableCharacterControls();
            _networkInitializers.ForEach(_networkInitializer => _networkInitializer.InitializeForOwner());
            //_controller.enabled = true;
            //_playerCamera.enabled = true;
            //_objectInteraction.enabled = true;
            _UI.SetActive(true);
        }
    }
}
