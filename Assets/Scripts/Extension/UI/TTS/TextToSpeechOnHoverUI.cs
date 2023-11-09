using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextToSpeechOnHoverUI : TextToSpeechOnHoverBase, IPointerDownHandler
{
    protected override IEnumerator PlayTTSAfterDelay()
    {
        yield return _delayCoroutine;

        if (Signal != null)
        {
            Signal.Fire(new AccessibilityTTSPlayOnHoverViewSignal(gameObject));
        }
        else
        {
            AsyncProcessorService.Instance.Signal.Fire(new AccessibilityTTSPlayOnHoverViewSignal(gameObject));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_сoroutine != null)
        {
            StopCoroutine(_сoroutine);
            _сoroutine = null;
        }
    }
}