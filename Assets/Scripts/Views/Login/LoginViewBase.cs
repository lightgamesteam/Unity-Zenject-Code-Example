using Module.Core;
using TDL.Constants;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class LoginViewBase : ViewBase
{
    [Header(nameof(LoginViewBase))]

    [SerializeField] protected TMP_DropdownV2 languageDropdown;
    [SerializeField] protected TextMeshProUGUI languageDropdownKey;
    [SerializeField] protected InputField userLogin;
    [SerializeField] protected InputField userPassword;
    [SerializeField] protected TextMeshProUGUI signInTitleText;
    [SerializeField] protected TextMeshProUGUI signInButtonText;
    [SerializeField] protected TextMeshProUGUI rememberText;
    [SerializeField] protected Text placeHolderLogin;
    [SerializeField] protected Text placeHolderPassword;
    [SerializeField] protected TextMeshProUGUI forgotPasswordText;
    [SerializeField] protected TextMeshProUGUI versionText;
    [SerializeField] protected TextMeshProUGUI errorHandlerText;
    [SerializeField] protected Toggle rememberUser;
    [SerializeField] protected GameObject loginOtherPanel;
    [SerializeField] protected Button login3DLButton;
    [SerializeField] protected Button loginFeideButton;
    [SerializeField] protected Button loginSkolonButton;
    [SerializeField] protected Button loginMicrosoftButton;
    [SerializeField] protected Button _close3DLLoginButton;
    [SerializeField] protected GameObject TopMenu;

    [SerializeField] protected SelectableKeyboardKeyEvent _userLoginOnSubmit;
    [SerializeField] protected SelectableKeyboardKeyEvent _userUserPasswordOnSubmit;

    [SerializeField] protected GameObject login3DLPanel;
    [SerializeField] protected Button loginOtherButton;



    [SerializeField] protected Button privacyAndTermsButton;

    [SerializeField] protected TextMeshProUGUI privacyAndTermsButtonText;
    [SerializeField] protected TextMeshProUGUI signInWithButtonText;

    [SerializeField] protected Button _forgotPasswordButton;
    [SerializeField] protected Button _loginButton;
    [SerializeField] protected Button _closeAppButton;
    [SerializeField] protected Button _contactUsButton;
    [SerializeField] protected TextMeshProUGUI _contactUsButtonText;

    [SerializeField] protected Toggle showPasswordToggle;
    
    public override void InitUiComponents()
    {
        if (languageDropdown == null)
        {
            languageDropdown = transform.Get<TMP_DropdownV2>("Panel_Top/LanguageDropdown");
        }
        if (languageDropdownKey == null)
        {
            languageDropdownKey = languageDropdown.transform.Get<TextMeshProUGUI>("Label");
        }
        if (userLogin == null)
        {
            userLogin = transform.Get<InputField>("Panel/X_Panel_Login3DL/UserDataContainer/UserName/input_Login");
        }
        if (_userLoginOnSubmit == null)
        {
            _userLoginOnSubmit = userLogin.GetComponent<SelectableKeyboardKeyEvent>();
        }
        if (userPassword == null)
        {
            userPassword = transform.Get<InputField>("Panel/X_Panel_Login3DL/UserDataContainer/UserPassword/input_Password");
        }
        if (_userUserPasswordOnSubmit == null)
        {
            _userUserPasswordOnSubmit = userPassword.GetComponent<SelectableKeyboardKeyEvent>();
        }
        if (errorHandlerText == null)
        {
            errorHandlerText = transform.Get<TextMeshProUGUI>("Panel/X_Panel_Login3DL/txt_ErrorHandler");
        }
        if (rememberUser == null)
        {
            rememberUser = transform.Get<Toggle>("Panel/X_Panel_Login3DL/tgl_Remember");
        }
        if (_forgotPasswordButton == null)
        {
            _forgotPasswordButton = transform.Get<Button>("Panel/X_Panel_Login3DL/btn_forgotPassword");
        }
        if (_loginButton == null)
        {
            _loginButton = transform.Get<Button>("Panel/X_Panel_Login3DL/btn_LoginButton");
        }
        if (_closeAppButton == null)
        {
            _closeAppButton = transform.Get<Button>("Panel_Top/CloseAppButton");
        }
        if (versionText == null)
        {
            versionText = transform.Get<TextMeshProUGUI>("Panel/VersionText");
        }
        versionText.SetText($"ver: {Application.version}");

        // localization texts
        if (signInTitleText == null)
        {
            signInTitleText = transform.Get<TextMeshProUGUI>("Panel/X_Panel_Login3DL/SignInTitleText");
        }
        if (signInButtonText == null)
        {
            signInButtonText = transform.Get<TextMeshProUGUI>("Panel/X_Panel_Login3DL/btn_LoginButton/SignInButtonText");
        }
        if (rememberText == null)
        {
            rememberText = transform.Get<TextMeshProUGUI>("Panel/X_Panel_Login3DL/tgl_Remember/RememberText");
        }
        if (placeHolderLogin == null)
        {
            placeHolderLogin = userLogin.transform.Get<Text>("Text Area/Placeholder");
        }
        if (placeHolderPassword == null)
        {
            placeHolderPassword = userPassword.transform.Get<Text>("Text Area/Placeholder");
        }
        if (forgotPasswordText == null)
        {
            forgotPasswordText = transform.Get<TextMeshProUGUI>("Panel/X_Panel_Login3DL/btn_forgotPassword/ForgotPasswordText");
        }
        if (login3DLPanel == null)
        {
            login3DLPanel = transform.Get("Panel/X_Panel_Login3DL");
        }
        if (loginOtherButton == null)
        {
            loginOtherButton = transform.Get<Button>("Panel/X_Panel_Login3DL/btn_LoginOther");
        }
        if (loginOtherPanel == null)
        {
            loginOtherPanel = transform.Get("Panel/Panel_LoginOther");
        }
        if (login3DLButton == null)
        {
            login3DLButton = transform.Get<Button>("Panel/Panel_LoginOther/Button_3DL");
        }
        if (loginFeideButton == null)
        {
            loginFeideButton = transform.Get<Button>("Panel/Panel_LoginOther/Button_Feide");
        }

        if (privacyAndTermsButton == null)
        {
            privacyAndTermsButton = transform.Get<Button>("Panel/Panel_LoginOther/Button_PrivacyAndTerms");
        }
        if (privacyAndTermsButtonText == null)
        {
            privacyAndTermsButtonText = privacyAndTermsButton.GetComponentInChildren<TextMeshProUGUI>();
        }
        
        
        #if UNITY_WEBGL
        _closeAppButton.gameObject.SetActive(false);
        #endif
    }
    
    protected void ShowPassword(bool status)
    {
        userPassword.contentType = status 
            ? InputField.ContentType.Standard 
            : InputField.ContentType.Password;

        userPassword.ForceLabelUpdate();
    }

    protected void HideTopMenu(bool status)
    {
        if (TopMenu != null)
        {
            TopMenu.gameObject.SetActive(!status);
            SetActiveOtherLoginPanel(!status);
        }
    }

    public override void HideMobileInput()
    {
        if(userLogin != null)
            userLogin.shouldHideMobileInput = true;
        
        if(userPassword != null)
            userPassword.shouldHideMobileInput = true;
    }

    public override void SubscribeOnListeners()
    {
        Signal.Fire(new OnLoginActivatedViewSignal());
        SetActiveOtherLoginPanel(true);
        HideTopMenu(false);
        showPasswordToggle.isOn = false;
        languageDropdown.onValueChanged.AddListener(OnLanguageDropdownChanged);
        _loginButton.onClick.AddListener(OnLoginClick);
        _forgotPasswordButton.onClick.AddListener(OnForgotPasswordClick);
        _closeAppButton.onClick.AddListener(OnCloseAppClick);
        _close3DLLoginButton.onClick.AddListener(Close3DlLogin);
        
        login3DLButton.onClick.AddListener(() =>
        {
            SetActiveOtherLoginPanel(false);
            HideTopMenu(true);
            userLogin.Select();
        });

        loginOtherButton.onClick.AddListener(() => SetActiveOtherLoginPanel(true));
    }


    private void Close3DlLogin()
    {
        HideTopMenu(false);
        SetActiveOtherLoginPanel(true);
    }

    public override void UnsubscribeFromListeners()
    {

        Signal.Fire(new OnLoginDeactivatedViewSignal());

        languageDropdown.onValueChanged.RemoveAllListeners();
        _loginButton.onClick.RemoveAllListeners();
        _forgotPasswordButton.onClick.RemoveAllListeners();
        _closeAppButton.onClick.RemoveAllListeners();
        _close3DLLoginButton.onClick.RemoveAllListeners();
        _contactUsButton.onClick.RemoveAllListeners();
        
        login3DLButton.onClick.RemoveAllListeners();
        loginOtherButton.onClick.RemoveAllListeners();
    }

    public void SetActiveOtherLoginPanel(bool isActive)
    {
        loginOtherPanel.SetActive(isActive);
        login3DLPanel.SetActive(!isActive);
    }

    public void UpdateLanguageDropdownManually(int itemIndex)
    {
        languageDropdown.value = itemIndex;
    }
    
    protected void HideBackground() {
        var uiBackgroundCanvasGroup = GameObject.Find("UIBackground")?.GetComponent<CanvasGroup>();
        if (uiBackgroundCanvasGroup != null) {
            Utilities.Component.SetActiveCanvasGroup(uiBackgroundCanvasGroup, false);
        }
    }
    
    private void OnLanguageDropdownChanged(int itemIndex)
    {
        Signal.Fire(new OnChangeLanguageClickViewSignal(itemIndex));
    }
    
    protected void OnLoginClick()
    {
        
        Signal.Fire<LoginClickViewSignal>();
    }
    
    private void OnForgotPasswordClick()
    {
        Signal.Fire(new OpenUrlCommandSignal(ServerConstants.ForgotPasswordUrl));
    }
    
    private void OnCloseAppClick()
    {
        Signal.Fire(new CloseAppClickViewSignal());
    }

    public class Factory : PlaceholderFactory<LoginViewBase>
    {
    }


    public TMP_DropdownV2 LanguageDropdown { get => languageDropdown; }
    public TextMeshProUGUI LanguageDropdownKey { get => languageDropdownKey; }
    public InputField UserLogin { get => userLogin; }
    public InputField UserPassword { get => userPassword; }

    public TextMeshProUGUI SignInTitleText { get => signInTitleText; }
    public TextMeshProUGUI SignInButtonText { get => signInButtonText; }
    public TextMeshProUGUI RememberText { get => rememberText; }
    public Text PlaceHolderLogin { get => placeHolderLogin; }
    public Text PlaceHolderPassword { get => placeHolderPassword; }

    public TextMeshProUGUI ForgotPasswordText { get => forgotPasswordText; }
    public TextMeshProUGUI VersionText { get => versionText; }

    public TextMeshProUGUI ErrorHandlerText { get => errorHandlerText; }
    public Toggle RememberUser { get => rememberUser; }
    public Button PrivacyAndTermsButton { get => privacyAndTermsButton; }
    public Button ContactUsButton { get => _contactUsButton; }
    public TextMeshProUGUI PrivacyAndTermsButtonText { get => privacyAndTermsButtonText; }
    public TextMeshProUGUI ContactUsButtonText { get => _contactUsButtonText; }
    public TextMeshProUGUI SignInWithText { get => signInWithButtonText; }
    public Button LoginFeideButton { get => loginFeideButton; }
    public Button LoginSkolonButton { get => loginSkolonButton; }
    public Button LoginMicrosoftButton { get => loginMicrosoftButton; }
    protected GameObject LoginOtherPanel { get => loginOtherPanel; }
    protected Button Login3DLButton { get => login3DLButton; }
    protected GameObject Login3DLPanel { get => login3DLPanel; }
    protected Button LoginOtherButton { get => loginOtherButton; }
}