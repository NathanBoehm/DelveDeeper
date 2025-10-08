using Unity.Netcode;
using UnityEngine;

public class NetworkHostJoin : MonoBehaviour
{
    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        this.gameObject.SetActive(false);
    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();
        this.gameObject.SetActive(false);
    }
}
