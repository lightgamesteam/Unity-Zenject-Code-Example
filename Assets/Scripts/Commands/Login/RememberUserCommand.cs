using System;
using TDL.Constants;
using UnityEngine;
using Zenject;
using TDL.Models;

namespace TDL.Commands
{
    public class RememberUserCommand : ICommand
    {
        [Inject] private UserLoginModel _userLoginModel;

        public void Execute()
        {
            PlayerPrefs.SetInt(LoginConstants.RememberUser, Convert.ToInt32(_userLoginModel.RememberUser));
            PlayerPrefs.SetString(LoginConstants.UserLogin, _userLoginModel.RememberUser ? _userLoginModel.UserLogin : string.Empty);
            PlayerPrefs.SetString(LoginConstants.UserPassword, _userLoginModel.RememberUser ? _userLoginModel.UserPassword : string.Empty);
        }
    }
}