using System.Collections;
using TDL.Constants;
using UnityEngine;

namespace Module.PeriodicTable
{
    public class PeriodicElementView : MonoBehaviour
    {
        private Coroutine _сoroutine;

        private void OnMouseDown()
        {
            EventController.ClickOnElement(gameObject);
        }

        private void OnMouseEnter()
        {
            if (PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.AccessibilityTextToAudio))
            {
                _сoroutine = StartCoroutine(WaitForSeconds());
            }
        }

        private void OnMouseExit()
        {
            if (_сoroutine != null)
            {
                StopCoroutine(_сoroutine);
                _сoroutine = null;
            }
        }

        private IEnumerator WaitForSeconds()
        {
            yield return new WaitForSeconds(0.5f);
            var periodicElement = PeriodicTableController.GetElement(gameObject);
            var nameElement = PeriodicElementsController.GetNameElement(periodicElement.name);
        }
    }
}