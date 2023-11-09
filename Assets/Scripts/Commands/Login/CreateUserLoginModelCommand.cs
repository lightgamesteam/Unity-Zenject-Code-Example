using System.Linq;
using Managers;
using Newtonsoft.Json;
using TDL.Constants;
using TDL.Models;
using TDL.Server;
using Zenject;
using TDL.Signals;
using UnityEngine;

namespace TDL.Commands
{
    public class CreateUserLoginModelCommand : ICommandWithParameters
    {
        [Inject] private UserLoginModel _userLoginModel;
        [Inject] private SignalBus _signal;
        [Inject] private ExternalCallManager _externalCallManager;

        public void Execute(ISignal signal)
        {
            Debug.Log("CreateUserLoginModelCommand");
            var parameter = (CreateUserLoginModelSignal) signal;
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(parameter.LoginResponse);

            if (loginResponse.Success)
            {
                Debug.Log("CreateUserLoginModelCommand: loginResponse.Success");
                UserLoginModel.UserLoginStruct userLoginStruct;
                userLoginStruct.IsLoggedAsUser = !parameter.AsGuest;

                // ToDo handle multiple Roles
                // ToDo: Rewrite authorization, allow usage of refresh token outside of Teams
                userLoginStruct.IsTeacher = !string.IsNullOrEmpty(loginResponse.User.Roles.First()) &&
                                            loginResponse.User.Roles.First().Equals(LoginConstants.LoginTypeTeacher);

                userLoginStruct.AuthorizationToken = loginResponse.AuthorizationToken;

                userLoginStruct.GuestAuthorizationToken = _userLoginModel.GuestAuthorizationToken != string.Empty
                    ? _userLoginModel.GuestAuthorizationToken : loginResponse.AuthorizationToken;
                
                userLoginStruct.FirstLetter = GetUserFirstLetter(loginResponse.User.Firstname);
                userLoginStruct.Firstname = loginResponse.User.Firstname;
                userLoginStruct.Lastname = loginResponse.User.Lastname;
                userLoginStruct.TermAccepted = loginResponse.User.TermAccepted;

                _userLoginModel.Update(userLoginStruct);
                _userLoginModel.IsSigningOut = false;
            }
            else
            {
                Debug.Log("CreateUserLoginModelCommand : loginResponse.NotSuccess");
                _signal.Fire(new PopupOverlaySignal(false));
                
                if(loginResponse.LocalizedError.Count > 0) // TODO: Fix > Feide login error return empty: LocalizedError
                    _userLoginModel.ErrorMessages = loginResponse.LocalizedError;
            }
        }

        private string GetUserFirstLetter(string userFirstName)
        {
            var res = (userFirstName?.First() ?? '?').ToString().ToUpper();
            return (userFirstName?.First() ?? '?').ToString().ToUpper();
        }
    }
}