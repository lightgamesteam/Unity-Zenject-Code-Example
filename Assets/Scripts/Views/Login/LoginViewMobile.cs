using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginViewMobile : LoginViewBase
{
    public Button _languageButton;
    public TextMeshProUGUI _languageDropdownText;
    
    public override void InitUiComponents()
    {
        Debug.Log("LoginViewMobile => InitUiComponents");
        languageDropdown = transform.Get<TMP_DropdownV2>("X_Panel_Login3DL/LanguageDropdown");
        _languageButton = transform.Get<Button>("X_Panel_Login3DL/LanguageDropdown/LanguageButton");
        _languageDropdownText = transform.Get<TextMeshProUGUI>("X_Panel_Login3DL/LanguageDropdown/Label");
        
        userLogin = transform.Get<InputField>("X_Panel_Login3DL/UserDataContainer/UserName/input_Login");
        userPassword = transform.Get<InputField>("X_Panel_Login3DL/UserDataContainer/UserPassword/input_Password");
        errorHandlerText = transform.Get<TextMeshProUGUI>("X_Panel_Login3DL/txt_ErrorHandler");
        rememberUser = transform.Get<Toggle>("X_Panel_Login3DL/tgl_Remember");
        _forgotPasswordButton = transform.Get<Button>("X_Panel_Login3DL/btn_forgotPassword");
        _loginButton = transform.Get<Button>("X_Panel_Login3DL/btn_LoginButton");
        _closeAppButton = transform.Get<Button>("CloseAppButton");
        
        versionText = transform.Get<TextMeshProUGUI>("VersionText");
        versionText.SetText($"ver: {Application.version}");

        // localization texts
        signInTitleText = transform.Get<TextMeshProUGUI>("X_Panel_Login3DL/SignInTitleText");
        signInButtonText = transform.Get<TextMeshProUGUI>("X_Panel_Login3DL/btn_LoginButton/SignInButtonText");
        rememberText = transform.Get<TextMeshProUGUI>("X_Panel_Login3DL/tgl_Remember/RememberText");
        placeHolderLogin = UserLogin.transform.Get<Text>("Text Area/Placeholder");
        placeHolderPassword = userPassword.transform.Get<Text>("Text Area/Placeholder");
        forgotPasswordText = transform.Get<TextMeshProUGUI>("X_Panel_Login3DL/btn_forgotPassword/ForgotPasswordText");
        
        login3DLPanel = transform.Get("X_Panel_Login3DL");
        loginOtherButton = transform.Get<Button>("X_Panel_Login3DL/btn_LoginOther");

        loginOtherPanel = transform.Get("Panel_LoginOther");
        login3DLButton = transform.Get<Button>("Panel_LoginOther/Button_3DL");
        loginFeideButton = transform.Get<Button>("Panel_LoginOther/Button_Feide");
        privacyAndTermsButton = transform.Get<Button>("Panel_LoginOther/Button_PrivacyAndTerms");
        privacyAndTermsButtonText = PrivacyAndTermsButton.GetComponentInChildren<TextMeshProUGUI>();
        
        InitShowPasswordToggle();
        HideBackground();
    }

    private void InitShowPasswordToggle()
    {
        showPasswordToggle = transform.Get<Toggle>("X_Panel_Login3DL/UserDataContainer/UserPassword/input_Password/Toggle_ShowPassword");
        showPasswordToggle.onValueChanged.AddListener(ShowPassword);
    }
}