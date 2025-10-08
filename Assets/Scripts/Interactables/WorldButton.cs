using EditorAttributes;
using Interactable;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WorldButton : MonoBehaviour, IInteractable
{
    [SerializeField] private Vector3    _pressedPosition;
    [SerializeField] private float      _animLength = 0.5f;
    [SerializeField] private float      _pressResetDelay = 0.2f;

    [SerializeField] private AudioSource    _audioSource;
    [SerializeField] private AudioClip      _audioClip;

    public UnityEvent OnPressed;

    private Vector3 _ogPosition;
    private bool    _pressed = false;


    private void Awake()
    {
        _ogPosition = transform.localPosition;
    }

    public string HighlightText => "Push Button";

    public void Interact(GameObject interacter)
    {
        if (_pressed)
            return;

        _pressed = true;
        OnPressed?.Invoke();
        StartCoroutine(PlayPressAnimation());
    }

    private IEnumerator PlayPressAnimation()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _animLength)
        {
            gameObject.transform.localPosition = Vector3.Lerp(_ogPosition, _pressedPosition, elapsedTime / _animLength);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(_pressResetDelay);

        elapsedTime = 0f;
        while (elapsedTime < _animLength)
        {
            gameObject.transform.localPosition = Vector3.Lerp(_pressedPosition, _ogPosition, elapsedTime / _animLength);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameObject.transform.localPosition = _ogPosition;
        _pressed = false;
    }
}
