using EditorAttributes;
using Interactable;
using System.Collections;
using UnityEngine;

public class WorldButton : MonoBehaviour, IInteractable
{
    [SerializeField] private Vector3    _pressedPosition;
    [SerializeField] private float      _animLength = 0.5f;
    [SerializeField] private float      _pressResetDelay = 0.2f;

    [SerializeField] private AudioSource    _audioSource;
    [SerializeField] private AudioClip      _audioClip;

    [SerializeField] private bool _localPosition = true;


    private Vector3 _ogPositionLocal;
    private Vector3 _ogPositionGlobal;
    private bool    _pressed = false;


    private void Awake()
    {
        _ogPositionLocal = transform.localPosition;
        _ogPositionGlobal = transform.position;
    }

    public string HighlightText => "Push Button";

    public void Interact(GameObject interacter)
    {
        if (_pressed)
            return;

        _pressed = true;
        StartCoroutine(_localPosition ? PlayLocalPressAnimation() : PlayGlobalPressAnimation());
    }

    private IEnumerator PlayLocalPressAnimation()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _animLength)
        {
            gameObject.transform.localPosition = Vector3.Lerp(_ogPositionLocal, _pressedPosition, elapsedTime / _animLength);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(_pressResetDelay);

        elapsedTime = 0f;
        while (elapsedTime < _animLength)
        {
            gameObject.transform.localPosition = Vector3.Lerp(_pressedPosition, _ogPositionLocal, elapsedTime / _animLength);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameObject.transform.localPosition = _ogPositionLocal;
        _pressed = false;
    }

    private IEnumerator PlayGlobalPressAnimation()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _animLength)
        {
            gameObject.transform.position = Vector3.Lerp(_ogPositionGlobal, _pressedPosition, elapsedTime / _animLength);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(_pressResetDelay);

        elapsedTime = 0f;
        while (elapsedTime < _animLength)
        {
            gameObject.transform.position = Vector3.Lerp(_pressedPosition, _ogPositionGlobal, elapsedTime / _animLength);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameObject.transform.position = _ogPositionGlobal;
        _pressed = false;
    }
}
