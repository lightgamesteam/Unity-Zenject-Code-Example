using UnityEngine;
using UnityEngine.UI;

namespace TDL.Views
{
    public class LoginViewPC : LoginViewBase
    {
        public override void InitUiComponents() {
            base.InitUiComponents();
            InitShowPasswordToggle();
            HideBackground();
        }

        private void InitShowPasswordToggle()
        {
            if (showPasswordToggle == null)
            {
                showPasswordToggle = transform.Get<Toggle>("Panel/X_Panel_Login3DL/UserDataContainer/UserPassword/input_Password/Toggle_ShowPassword");
            }
            showPasswordToggle.onValueChanged.AddListener(ShowPassword);
        }
        

        public override void SubscribeOnListeners()
        {
            base.SubscribeOnListeners();
            
            _userLoginOnSubmit.onKeyDown.AddListener((keyCode) => OnLoginClick());
            _userUserPasswordOnSubmit.onKeyDown.AddListener((keyCode) => OnLoginClick());
        }

        public override void UnsubscribeFromListeners()
        {
            base.UnsubscribeFromListeners();
            
            _userLoginOnSubmit.onKeyDown.RemoveAllListeners();
            _userUserPasswordOnSubmit.onKeyDown.RemoveAllListeners();
        }
    }
}