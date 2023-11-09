using System.Collections;
using UnityEngine.EventSystems;
using Zenject;

namespace Module.PeriodicTable
{
    public class PeriodicTableSpeechOnHoverNewUI : TextToSpeechOnHoverBase, IPointerDownHandler
    {
        public SignalBus SignalBus;

        protected override IEnumerator PlayTTSAfterDelay()
        {
            yield return _delayCoroutine;
            SignalBus = Signal ?? ApplicationView.instance.Signal;
            SignalBus.Fire(new AccessibilityTTSPlayOnHoverViewSignal(gameObject));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StopAllCoroutines();
        }
    }
}