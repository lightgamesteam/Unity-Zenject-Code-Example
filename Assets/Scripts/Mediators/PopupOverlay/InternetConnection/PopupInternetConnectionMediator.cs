using System;
using TDL.Models;
using TDL.Signals;
using UnityEngine;
using Zenject;

namespace TDL.Views
{
    public class PopupInternetConnectionMediator : IInitializable, IDisposable
    {
        private PopupInternetConnectionView _view;

        [Inject] private PopupModel _popupModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private PopupInternetConnectionView.Factory _factory;

        public void Initialize()
        {
            SubscribeOnListeners();
        }

        public void Dispose()
        {
            UnsubscribeFromListeners();
        }
        
        private void SubscribeOnListeners()
        {
            _popupModel.OnShowInternetConnectionChanged += ShowInternetConnectionChanged;
            _signal.Subscribe<PopupInternetConnectionRetryViewSignal>(OnRetryClick);
            _signal.Subscribe<PopupInternetConnectionExitViewSignal>(OnExitClick);
        }

        private void UnsubscribeFromListeners()
        {
            if (_popupModel != null)
            {
                _popupModel.OnShowInternetConnectionChanged -= ShowInternetConnectionChanged;
            }
            
            _signal.Unsubscribe<PopupInternetConnectionRetryViewSignal>(OnRetryClick);
            _signal.Unsubscribe<PopupInternetConnectionExitViewSignal>(OnExitClick);
        }

        private void ShowInternetConnectionChanged(bool status)
        {
            if (status)
            {
                _view = _factory.Create();
            }
            else
            {
                if (_view != null)
                {
                    GameObject.Destroy(_view.gameObject);
                }
            }
        }

        private void OnRetryClick(PopupInternetConnectionRetryViewSignal signal)
        {
            _signal.Fire<HidePopupInternetConnectionCommandSignal>();
            _signal.Fire(_popupModel.InternetConnectionRetrySource);
        }

        private void OnExitClick(PopupInternetConnectionExitViewSignal signal)
        {
            Application.Quit();
        }
    }
}