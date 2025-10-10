using EditorAttributes;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RotatingObj : NetworkBehaviour
{
    private float deg = 180f;
    private const float len = 1.5f;

    private bool _doingAKickFlip = false;

    [Button]
    [Rpc(SendTo.Server)]
    public void DoAKickFlipRpc()
    {
        if (_doingAKickFlip) return;

        _doingAKickFlip = true;
        StartCoroutine(Rotate());


    }

    private IEnumerator Rotate()
    {
        float elapsedTime = 0f;
        while (elapsedTime < len)
        {
            //this.gameObject.SetLocalEulerRotation().AddX((deg / len) * Time.deltaTime);
            this.gameObject.transform.Rotate(Vector3.right, (deg/len) * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _doingAKickFlip = false;
    }
}
