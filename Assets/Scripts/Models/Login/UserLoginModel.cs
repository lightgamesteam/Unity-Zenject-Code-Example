using System;
using System.Collections.Generic;

namespace TDL.Models
{
    public class UserLoginModel
    {   
        public Action OnLoginSuccess;
        public Action OnGuestLoginSuccess;
        public Action OnGuestLoginError;
        public Action OnLoginError;
        public Action<bool> OnRememberUserChanged;
        public Action<string> OnUserLoginChanged;
        public Action<string> OnUserPasswordChanged;
        public Action OnUserLogout;
        public Action OnLoginScreenReady;

#if UI_IOS || UI_ANDROID
        
        public bool isDebugLogin = true;
        
#endif

        public string GuestAuthorizationToken { get; set; }
        public string AuthorizationToken { get; set; }
        public string RefreshToken { get; set; }
        public string IdentityToken { get; set; }
        
        public bool IsLoggedAsUser { get; set; }
        public bool IsTeacher { get; private set; }
        public string FirstLetter { get; private set; }
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }
        public bool TermAccepted { get; set; }
        
        private bool _rememberUser;
        public bool RememberUser
        {
            get { return _rememberUser; }
            set
            {
                if (_rememberUser == value) return;
                _rememberUser = value;
                OnRememberUserChanged?.Invoke(_rememberUser);
            }
        }

        private string _userLogin;
        public string UserLogin
        {
            get { return _userLogin; }
            set
            {
                if (_userLogin == value) return;
                _userLogin = value;
                OnUserLoginChanged?.Invoke(_userLogin);
            }
        }
        
        private string _userPassword;
        public string UserPassword
        {
            get { return _userPassword; }
            set
            {
                if (_userPassword == value) return;
                _userPassword = value;
                OnUserPasswordChanged?.Invoke(_userPassword);
            }
        }

        private Dictionary<string, string> _errorMessages;
        public Dictionary<string, string> ErrorMessages
        {
            get { return _errorMessages; }
            set
            {
                if (_errorMessages == value) return;
                _errorMessages = value;
                OnLoginError?.Invoke();
            }
        }

        public bool IsMicrosoftLogin { get; set; }
        public bool IsSigningOut { get; set; }

        public bool IsLoginScreenReady
        {
            set
            {
                OnLoginScreenReady?.Invoke();
            }
        }

        public void Update(UserLoginStruct userLoginStruct)
        {
            IsLoggedAsUser = userLoginStruct.IsLoggedAsUser;
            IsTeacher = userLoginStruct.IsTeacher;
            AuthorizationToken = userLoginStruct.AuthorizationToken;
            //RefreshToken = userLoginStruct.RefreshToken;
            GuestAuthorizationToken = userLoginStruct.GuestAuthorizationToken;
            FirstLetter = userLoginStruct.FirstLetter;
            Firstname = userLoginStruct.Firstname;
            Lastname = userLoginStruct.Lastname;
            TermAccepted = userLoginStruct.TermAccepted;

            if (!IsLoggedAsUser)
                OnGuestLoginSuccess?.Invoke();
            else
                OnLoginSuccess?.Invoke();
        }
        
        public struct UserLoginStruct
        {
            public bool IsLoggedAsUser;
            public bool IsTeacher;
            public string AuthorizationToken;
            public string GuestAuthorizationToken;
            //public string RefreshToken;
            public string FirstLetter;
            public string Firstname;
            public string Lastname;
            public bool TermAccepted;
        }

        public void Reset()
        {
            RememberUser = false;
            UserLogin = string.Empty;
            UserPassword = string.Empty;
            IsLoggedAsUser = false;
            IsTeacher = false;
            TermAccepted = false;
            ErrorMessages?.Clear();

#if UI_IOS || UI_ANDROID

            isDebugLogin = true;
            
#endif
            
            OnUserLogout?.Invoke();
        }
    }
}