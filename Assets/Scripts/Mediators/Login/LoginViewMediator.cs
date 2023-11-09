using System;
using System.Collections.Generic;
using Signals;
using Signals.Login;
using TDL.Constants;
using Zenject;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using UnityEngine;

namespace TDL.Views
{
    public class LoginViewMediator : IInitializable, IDisposable, IMediator
    {
        protected LoginViewBase _loginView;

        [Inject] protected LoginViewBase.Factory _loginViewFactory;
        [Inject] protected readonly SignalBus _signal;
        [Inject] protected UserLoginModel _userLoginModel;
        [Inject] protected LocalizationModel _localizationModel;
        [Inject] protected IWindowService _windowService;
        [Inject] protected readonly AccessibilityModel _accessibilityModel;
        [Inject] protected readonly ServerService _serverService;

        public void Initialize()
        {
            try
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                if (DeviceInfo.IsMobile())
                {
                    _signal.Fire(new PopupOverlaySignal(true, "The 3DL WEBGL application is available on PC, desktop, and iPad. Please choose one of the following options to enjoy the best user experience."));
                    return;
                }
#endif
                CreateScreen();
                SubscribeOnListeners();
                LoadDiscoveryDocument();

                if (DeviceInfo.IsMobile())
                {
                    CreateLanguageBox();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("IN catch: stack trace"+e.StackTrace);
                Debug.LogError(e.ToString());
                throw;
            }

        }

        protected virtual void SubscribeOnListeners()
        {

#if !DEBUG_POPUP

            _signal.Subscribe<LoginStateSignal>(OnLoginStateChange);

#endif

            _signal.Subscribe<OnLoginActivatedViewSignal>(OnViewEnable);
            _signal.Subscribe<OnLoginDeactivatedViewSignal>(OnViewDisable);

            _userLoginModel.OnGuestLoginError += OnGuestLoginError;
            _userLoginModel.OnGuestLoginSuccess += OnGuestLoginSuccess;
            _userLoginModel.OnLoginSuccess += OnLoginSuccess;
            _userLoginModel.OnLoginError += ShowLoginErrorMessage;
            _userLoginModel.OnRememberUserChanged += OnRememberUserChanged;
            _userLoginModel.OnUserLoginChanged += OnUserLoginChanged;
            _userLoginModel.OnUserPasswordChanged += OnUserPasswordChanged;
            _userLoginModel.OnUserLogout += UpdateLoginLocalizationAfterLogout;

            _signal.Subscribe<LoginClickViewSignal>(OnLoginClick);
            _signal.Subscribe<CloseAppClickViewSignal>(OnCloseAppClick);
            _signal.Subscribe<SignOutClickViewSignal>(OnSignOutClick);


            _loginView.LoginFeideButton.onClick.AddListener(OnLoginFeideClick);
            _loginView.LoginSkolonButton.onClick.AddListener(OnLoginSkolon);
            _loginView.LoginMicrosoftButton.onClick.AddListener(OnMicrosoftLoginClick);
            _loginView.PrivacyAndTermsButton.onClick.AddListener(OnPrivacyAndTermsClick);
            _loginView.ContactUsButton.onClick.AddListener(OnContactUsClick);

            _accessibilityModel.OnFontSizeChanged += OnFontSizeChanged;
        }

        private void OnContactUsClick()
        {
            if (_localizationModel.CurrentLanguageCultureCode=="nb-NO" || _localizationModel.CurrentLanguageCultureCode=="nn-NO")
            {
                Application.OpenURL("https://www.3dl.no/contact-us/");
            }
            else
            {
                Application.OpenURL("https://www.3dl.no/contact-us-en/");
            }
        }

        private void OnLoginSkolon()
        {
            _signal.Fire<LoginSkolonClickCommandSignal>();
        }

        private void OnMicrosoftLoginClick()
        {
            _serverService.GetLoginMicrosoft();
        }

        public void OnViewEnable()
        {
            _loginView.SetActiveOtherLoginPanel(true);
            OnLanguageChanged();
            _localizationModel.OnLanguageChanged += OnLanguageChanged;
            _signal.Subscribe<OnChangeLanguageClickViewSignal>(OnChangeLanguageClick);
        }

        public void OnViewDisable()
        {
            if (_localizationModel != null)
            {
                _localizationModel.OnLanguageChanged -= OnLanguageChanged;
            }

            _signal.TryUnsubscribe<OnChangeLanguageClickViewSignal>(OnChangeLanguageClick);
        }

        private void LoadDiscoveryDocument()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            //_signal.Fire<SetCurrentHostCommandSignal>();
#endif
            _signal.Fire<CreateDiscoveryDocumentModelCommandSignal>();
        }

        private void OnSignOutClick()
        {
            _loginView.SetActiveOtherLoginPanel(true);
        }

        private void OnLoginStateChange(LoginStateSignal signal)
        {
            if (!_windowService.IsCurrentWindow(WindowConstants.Login))
                return;

            switch (signal.loginState)
            {
                case LoginState.Authenticating:
                    _signal.Fire(new PopupOverlaySignal(true,
                        _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.AuthenticatingKey), false,
                        PopupOverlayType.LoadingBar));

                    break;

                case LoginState.UploadRecentlyViewedAssetsRequest:
                    _signal.Fire(new PopupOverlaySignal(true,
                        _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.DownloadingAssetsKey),
                        false,
                        PopupOverlayType.LoadingBar));

                    break;

                case LoginState.UploadContentAssetsRequest:
                    _signal.Fire(new PopupOverlaySignal(true,
                        _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.CreatingAssetsKey), false,
                        PopupOverlayType.LoadingBar));

                    break;
            }
        }

        private void CreateLanguageBox()
        {
            if (DeviceInfo.IsMobile())
            {
                var lvm = _loginView as LoginViewMobile;
                if (lvm != null)
                {
                    lvm._languageButton.onClick.AddListener(() =>
                        _signal.Fire(new PopupOverlaySignal(true, "", false, PopupOverlayType.LanguageBox)));
                }
            }
        }

        public void Dispose()
        {
            if (_userLoginModel != null)
            {
                _userLoginModel.OnGuestLoginError -= OnGuestLoginError;
                _userLoginModel.OnGuestLoginSuccess -= OnGuestLoginSuccess;
                _userLoginModel.OnLoginSuccess -= OnLoginSuccess;
                _userLoginModel.OnLoginError -= ShowLoginErrorMessage;
                _userLoginModel.OnRememberUserChanged -= OnRememberUserChanged;
                _userLoginModel.OnUserLoginChanged -= OnUserLoginChanged;
                _userLoginModel.OnUserPasswordChanged -= OnUserPasswordChanged;
                _userLoginModel.OnUserLogout -= UpdateLoginLocalizationAfterLogout;
            }

            if (_accessibilityModel != null)
            {
                _accessibilityModel.OnFontSizeChanged -= OnFontSizeChanged;
            }

#if !DEBUG_POPUP

            _signal.Unsubscribe<LoginStateSignal>(OnLoginStateChange);

#endif

            _signal.Unsubscribe<LoginClickViewSignal>(OnLoginClick);
            _signal.Unsubscribe<CloseAppClickViewSignal>(OnCloseAppClick);

            _signal.Unsubscribe<OnLoginActivatedViewSignal>(OnViewEnable);
            _signal.Unsubscribe<OnLoginDeactivatedViewSignal>(OnViewDisable);

            _loginView.LoginFeideButton.onClick.RemoveListener(OnLoginFeideClick);
            _loginView.LoginSkolonButton.onClick.RemoveListener(OnLoginSkolon);
            _loginView.LoginMicrosoftButton.onClick.RemoveListener(OnMicrosoftLoginClick);
            _loginView.PrivacyAndTermsButton.onClick.RemoveListener(OnPrivacyAndTermsClick);
            _loginView.ContactUsButton.onClick.RemoveListener(OnContactUsClick);
        }

        private void CreateScreen()
        {
            _loginView = _loginViewFactory.Create();
            _loginView.InitUiComponents();
            _signal.Fire(new RegisterScreenCommandSignal(WindowConstants.Login, _loginView.gameObject));
        }

        private void OnRememberUserChanged(bool rememberUser)
        {
            if (_loginView.RememberUser.isOn != rememberUser)
            {
                _loginView.RememberUser.isOn = rememberUser;
            }
        }

        private void OnUserLoginChanged(string userLogin)
        {
            if (!_loginView.UserLogin.text.Equals(userLogin))
            {
                _loginView.UserLogin.text = userLogin;
            }
        }

        private void OnUserPasswordChanged(string userPassword)
        {
            if (!_loginView.UserPassword.text.Equals(userPassword))
            {
                _loginView.UserPassword.text = userPassword;
            }
        }

        private void ShowLoginErrorMessage()
        {
            var localizedError = GetLocalizedError();
            _loginView.ErrorHandlerText.text = localizedError;
        }

        private string GetLocalizedError()
        {
            var serverErrors = _userLoginModel.ErrorMessages;

            return serverErrors.ContainsKey(_localizationModel.CurrentLanguageCultureCode)
                ? serverErrors[_localizationModel.CurrentLanguageCultureCode]
                : serverErrors[_localizationModel.FallbackCultureCode];
        }

        private void OnLoginClick()
        {
            HideErrorMessageIfShown();

            _signal.Fire(new LoginClickCommandSignal(_loginView.UserLogin.text, _loginView.UserPassword.text,
                _loginView.RememberUser.isOn));
        }

        private void OnPrivacyAndTermsClick()
        {
            _signal.Fire(new OpenTermCommandSignal(false));
        }

        private void OnLoginFeideClick()
        {
            _signal.Fire<LoginFeideClickCommandSignal>();
        }

        private void OnLoginTeamsClick()
        {
            _signal.Fire<LoginTeamsClickCommandSignal>();
        }

        private void OnCloseAppClick()
        {
            _signal.Fire<CloseAppCommandSignal>();
        }

        private void OnGuestLoginSuccess()
        {
            _signal.Fire<LoadAssetsAsGuestLoginScreenSignal>();
        }

        private void OnGuestLoginError()
        {
            Debug.Log("An error occured while logging in as Guest.");
        }

        private void OnLoginSuccess()
        {
            _signal.Fire<OnLoginSuccessCommandSignal>();
        }
        

        private void HideErrorMessageIfShown()
        {
            if (!string.IsNullOrEmpty(_loginView.ErrorHandlerText.text))
            {
                _loginView.ErrorHandlerText.text = string.Empty;
            }
        }

        #region Localization

        private void OnChangeLanguageClick(OnChangeLanguageClickViewSignal signal)
        {
            _signal.Fire(new ChangeLanguageCommandSignal(signal.LanguageIndex));
        }

        private void UpdateLoginLocalizationAfterLogout()
        {
            CreateLanguageUI();
            UpdateLanguageUI();
            ChangeUiInterface();
        }

        private void OnLanguageChanged()
        {
            CreateLanguageUI();
            UpdateLanguageUI();
            ChangeUiInterface();
        }

        protected virtual void CreateLanguageUI()
        {
            if (_localizationModel.AvailableLanguages.Count > 0)
            {
                var allLanguages = new List<string>();

                foreach (var language in _localizationModel.AvailableLanguages)
                {
                    allLanguages.Add(language.Name);
                }

                _loginView.LanguageDropdown.ClearOptions();
                _loginView.LanguageDropdown.AddOptions(allLanguages);
            }
        }

        protected virtual void UpdateLanguageUI()
        {
            var foundedLanguage = _localizationModel.AvailableLanguages.Find(item => item.Culture.Equals(_localizationModel.CurrentLanguageCultureCode));
            if (foundedLanguage != null)
            {
                var foundedIndex = _loginView.LanguageDropdown.options.FindIndex(item => item.text.Equals(foundedLanguage.Name));
                _loginView.UpdateLanguageDropdownManually(foundedIndex);
            }
        }

        protected virtual void ChangeUiInterface()
        {
            if (DeviceInfo.IsMobile())
            {
                var lvm = _loginView as LoginViewMobile;
                if (lvm != null)
                {
                    lvm._languageDropdownText.text = _localizationModel.GetLanguageNameByCultureCode(_localizationModel.CurrentLanguageCultureCode);
                }
            }


            _loginView.SignInTitleText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.SignInKey);
            _loginView.SignInButtonText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.SignInKey);
            _loginView.RememberText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.RememberKey);
            _loginView.PlaceHolderLogin.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.PlaceholderLoginKey);
            _loginView.PlaceHolderPassword.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.PlaceholderPasswordKey);
            _loginView.ForgotPasswordText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.IForgotMyPasswordKey);
            _loginView.PrivacyAndTermsButtonText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.LinkPrivacyTermKey);
            _loginView.ContactUsButtonText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextButtonContacktUs);
            _loginView.SignInWithText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.SignInWith);

            if (_userLoginModel.ErrorMessages != null && _userLoginModel.ErrorMessages.Count > 0)
            {
                _loginView.ErrorHandlerText.text = GetLocalizedError();
            }
        }

        #endregion
        
        #region Accessibility Font Size

        private void OnFontSizeChanged(float fontSizeScaleFactor)
        {
            if (fontSizeScaleFactor != AccessibilityConstants.FontSizeDefaultScaleFactor)
            {
                if (DeviceInfo.IsMobile())
                {
                    var lvm = _loginView as LoginViewMobile;
                    if (lvm != null)
                    {
                        lvm._languageDropdownText.fontSize = Mathf.RoundToInt(lvm._languageDropdownText.fontSize / fontSizeScaleFactor);
                    }
                }
        
                // common
                _loginView.LanguageDropdown.itemText.fontSize = Mathf.RoundToInt(_loginView.LanguageDropdown.itemText.fontSize / fontSizeScaleFactor);
                _loginView.LanguageDropdownKey.fontSize = Mathf.RoundToInt(_loginView.LanguageDropdownKey.fontSize / fontSizeScaleFactor);
                _loginView.VersionText.fontSize = Mathf.RoundToInt(_loginView.VersionText.fontSize / fontSizeScaleFactor);
                
                // feide
                // sign in
                _loginView.SignInTitleText.fontSize = Mathf.RoundToInt(_loginView.SignInTitleText.fontSize / fontSizeScaleFactor);
                _loginView.SignInButtonText.fontSize = Mathf.RoundToInt(_loginView.SignInButtonText.fontSize / fontSizeScaleFactor);
                _loginView.UserLogin.textComponent.fontSize = Mathf.RoundToInt(_loginView.UserLogin.textComponent.fontSize / fontSizeScaleFactor);
                _loginView.UserPassword.textComponent.fontSize = Mathf.RoundToInt(_loginView.UserPassword.textComponent.fontSize / fontSizeScaleFactor);
                _loginView.RememberText.fontSize = Mathf.RoundToInt(_loginView.RememberText.fontSize / fontSizeScaleFactor);
                _loginView.PlaceHolderLogin.fontSize = Mathf.RoundToInt(_loginView.PlaceHolderLogin.fontSize / fontSizeScaleFactor);
                _loginView.PlaceHolderPassword.fontSize = Mathf.RoundToInt(_loginView.PlaceHolderPassword.fontSize / fontSizeScaleFactor);
                _loginView.ForgotPasswordText.fontSize = Mathf.RoundToInt(_loginView.ForgotPasswordText.fontSize / fontSizeScaleFactor);
                _loginView.ErrorHandlerText.fontSize = Mathf.RoundToInt(_loginView.ErrorHandlerText.fontSize / fontSizeScaleFactor);
            }
        }
        
        #endregion
    }
}