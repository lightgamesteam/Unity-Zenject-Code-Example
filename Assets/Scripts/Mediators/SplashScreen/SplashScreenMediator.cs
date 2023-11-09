using System;
using System.Runtime.InteropServices;
using Commands.Common;
using Signals.Localization;
using Signals.Login;
using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using UnityEngine;
using Zenject;

namespace TDL.Views
{
    public class SplashScreenMediator : IInitializable, IDisposable
    {
        [Inject] private readonly SplashScreenView _view;
        [Inject] private readonly SignalBus _signal;
        [Inject] private readonly UserLoginModel _userLoginModel;
        [Inject] private readonly LocalizationModel _localizationModel;

        [DllImport("__Internal", EntryPoint = @"FullScreenAvailable")]
        public static extern int FullScreenAvailable();

        private bool _isRequiredTimeEnded;
        private bool _isLoginScreenReady;

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
            _signal.Subscribe<SplashScreenRequiredTimeEndedViewSignal>(OnRequiredTimeEnded);

            _userLoginModel.OnLoginScreenReady += OnLoginScreenReady;
        }

        private void UnsubscribeFromListeners()
        {
            _signal.Unsubscribe<SplashScreenRequiredTimeEndedViewSignal>(OnRequiredTimeEnded);

            if (_userLoginModel != null)
            {
                _userLoginModel.OnLoginScreenReady -= OnLoginScreenReady;
            }
        }

        private void OnRequiredTimeEnded()
        {
            _isRequiredTimeEnded = true;
            
            TryToShowLoginScreen();
        }

        private void OnLoginScreenReady()
        {
            _isLoginScreenReady = true;
            
#if UNITY_WEBGL && !UNITY_EDITOR

            if (!DeviceInfo.IsMobile())
            {
                var flag = FullScreenAvailable();

                var canSetFullScreen = Convert.ToBoolean(flag);
                if (canSetFullScreen)
                {
                    _signal.Fire(new PopupOverlaySignal(true, "Would you like to enter a Fullscreen mode?"
                        , type: PopupOverlayType.TextBox,
                        okCallback: () => { _signal.Fire(new SetFullscreenWebglCommandSignal()); },
                        closeCallback: () => { _signal.Fire(new PopupOverlaySignal(false)); }));
                }
            }

#endif
            TryToShowLoginScreen();
        }

        private void TryToShowLoginScreen()
        {
            if (_isLoginScreenReady && _isRequiredTimeEnded)
            {
                _signal.Fire<HandleApplicationLanguageSignal>();
                _signal.Fire<ShowLoginScreenCommandSignal>();
                _signal.Fire<TryTeamsSSOSignal>();
                _view.SetVisibility(false);
            }
        }
    }
}