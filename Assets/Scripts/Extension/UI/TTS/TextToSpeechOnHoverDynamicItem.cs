using System.Collections;
using UnityEngine;

public class TextToSpeechOnHoverDynamicItem : TextToSpeechOnHoverBase
{
    [SerializeField] private TextToSpeechOnHoverBase _anyNotDynamicItem;

    protected override IEnumerator PlayTTSAfterDelay()
    {
        yield return _delayCoroutine;

        if (Signal != null)
        {
            Signal.Fire(new AccessibilityTTSPlayOnHoverViewSignal(gameObject));
        }
        else if (_anyNotDynamicItem != null)
        {
            _anyNotDynamicItem.GetSignal()?.Fire(new AccessibilityTTSPlayOnHoverViewSignal(gameObject));
        } 
        else
        {
            Debug.LogWarning(typeof(TextToSpeechOnHoverBase) + " is null. Please use the 'SetTextToSpeechOnHoverController' method to initialize the controller.");
        }
    }

    public void SetTextToSpeechOnHoverController(TextToSpeechOnHoverBase textToSpeechOnHoverBase) {
        _anyNotDynamicItem = textToSpeechOnHoverBase;
    }
}