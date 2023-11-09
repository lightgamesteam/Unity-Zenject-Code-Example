using TDL.Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class PopupInternetConnectionView : ViewBase
    {
        private CanvasGroup _container;
        private Button _retryButton;
        private Button _exitButton;

        public override void InitUiComponents()
        {
            _container = GetComponent<CanvasGroup>();
            _retryButton = transform.Get<Button>("Panel/Buttons_Panel/RetryButton");
            _exitButton = transform.Get<Button>("Panel/Buttons_Panel/ExitButton");
        }

        public override void SubscribeOnListeners()
        {
            Signal.Fire(new FocusKeyboardNavigationSignal(_container, true));

            _retryButton.onClick.AddListener(OnRetryClick);
            _exitButton.onClick.AddListener(OnExitClick);
        }

        public override void UnsubscribeFromListeners()
        {
            Signal.Fire(new FocusKeyboardNavigationSignal(_container, false));

            _retryButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
        }

        private void OnRetryClick()
        {
            Signal.Fire<PopupInternetConnectionRetryViewSignal>();
        }

        private void OnExitClick()
        {
            Signal.Fire<PopupInternetConnectionExitViewSignal>();
        }

        public class Factory : PlaceholderFactory<PopupInternetConnectionView>
        {
        }
    }
}