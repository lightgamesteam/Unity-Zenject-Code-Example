using System.Collections;
using TDL.Signals;
using UnityEngine;

namespace TDL.Views
{
    public class SplashScreenView : ViewBase
    {
        private static float MinimalShowingTime = 1.0f;
        
        private void Start()
        {
            StartCoroutine(ShowSpecificTime());
        }

        private IEnumerator ShowSpecificTime()
        {
            yield return new WaitForSeconds(MinimalShowingTime);
        
            Signal.Fire<SplashScreenRequiredTimeEndedViewSignal>();
        }

        public void SetVisibility(bool status)
        {
            StartCoroutine(DisableOnNextFrame(status));
        }

        private IEnumerator DisableOnNextFrame(bool status)
        {
            yield return null;
            gameObject.SetActive(status);
        }
    }
}