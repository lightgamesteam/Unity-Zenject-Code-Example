using System;
using TDL.Constants;
using UnityEngine;
using Zenject;
using TDL.Models;

namespace TDL.Commands
{
    public class SetUserInfoIfRememberedCommand : ICommand
    {
        [Inject] private UserLoginModel _userLogin;

        public void Execute()
        {
            var isRemembered = Convert.ToBoolean(PlayerPrefs.GetInt(LoginConstants.RememberUser, 0));
            var userLogin = PlayerPrefs.GetString(LoginConstants.UserLogin, string.Empty);
            var userPassword = PlayerPrefs.GetString(LoginConstants.UserPassword, string.Empty);

            _userLogin.RememberUser = isRemembered;
            _userLogin.UserLogin = userLogin;
            _userLogin.UserPassword = userPassword;
        }
    }
}