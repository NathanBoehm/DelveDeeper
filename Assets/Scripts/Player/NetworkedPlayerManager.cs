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

    private INetworkObjectInitializer[] _networkInitializers;

    private void Awake()
    {
        _networkInitializers = this.gameObject.GetComponents<INetworkObjectInitializer>();
        _networkInitializers.ForEach(i => i.Initialize());
        _UI.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (HasAuthority)
        {
            ControlInputManager.Instance.EnableCharacterControls();
            _networkInitializers.ForEach(_networkInitializer => _networkInitializer.InitializeForOwner());
            _UI.SetActive(true);
        }
    }
}
