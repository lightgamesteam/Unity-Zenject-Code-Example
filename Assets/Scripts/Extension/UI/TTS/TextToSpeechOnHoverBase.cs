using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public abstract class TextToSpeechOnHoverBase : ViewBase, IPointerEnterHandler, IPointerExitHandler
{
    protected Coroutine _сoroutine;
    private const float _delayBeforePlay = 1.0f;
    protected readonly WaitForSeconds _delayCoroutine = new WaitForSeconds(_delayBeforePlay);
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        _сoroutine = StartCoroutine(PlayTTSAfterDelay());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_сoroutine != null)
        {
            StopCoroutine(_сoroutine);
            _сoroutine = null;
        }
    }

    protected abstract IEnumerator PlayTTSAfterDelay();
    
    public SignalBus GetSignal()
    {
        return Signal;
    }
}