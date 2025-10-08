using Unity.Netcode;
using UnityEngine;

public class PlayerGameState : MonoBehaviour, INetworkInitializer
{
    //[SerializeField] GameObject _serverUI;
    [field: SerializeField] public NetworkVariable<int> PlayerHealth { get; private set; }

    public void Initialize()
    {
        //_serverUI.SetActive(true);
    }

    public void InitializeForOwner()
    {
        //_serverUI.SetActive(false);
    }
}
