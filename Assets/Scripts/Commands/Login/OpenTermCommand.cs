using System;
using TDL.Constants;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class OpenTermCommand : ICommandWithParameters
    {
        [Inject] private UserLoginModel _userLoginModel;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private SignalBus _signal;

        private bool _isAcceptable;
        public void Execute(ISignal signal)
        {
            var parameter = (OpenTermCommandSignal) signal;
            _isAcceptable = parameter.IsAcceptable;
            AsyncProcessorService.Instance.Wait(0, Check);
        }

        private void Check()
        {
            if (_userLoginModel.TermAccepted && _userLoginModel.IsLoggedAsUser)
                return;

            _signal.Fire(new PopupOverlaySignal(true, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.LoadingKey)));
            
            if (!_isAcceptable)
            {
                _signal.Fire(new GetTermTextCommandSignal(_localizationModel.CurrentLanguageCultureCode, termText =>
                {
                    _signal.Fire(new PopupOverlaySignal(false));
                    _signal.Fire(new PopupInfoViewSignal(termText, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.OkKey), "", "", b => {}, () => {}));
                }));
            }
            else
            {
                _signal.Fire(new GetTermTextCommandSignal(_localizationModel.CurrentLanguageCultureCode, termText =>
                {
                    _signal.Fire(new PopupOverlaySignal(false));
                    
                    _signal.Fire(new PopupInfoViewSignal(termText, 
                        _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.OkKey), 
                        "", 
                        _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.CheckboxPrivacyTermKey),
                        isAccept =>
                        {
                            if(isAccept)
                                _signal.Fire(new AcceptTermCommandSignal());
                            else
                                _signal.Fire<SignOutClickCommandSignal>();
                        }, 
                        () => {})
                    );
                }));
            }
        }
    }
    
    public class ValidateTermCommand : ICommandWithParameters
    {
        [Inject] private ServerService _serverService;

        public void Execute(ISignal signal)
        {
            var parameter = (ValidateTermCommandSignal) signal;

            _serverService.ValidateTerm(parameter.ValidateTermAction);
        }
    }
    
    public class AcceptTermCommand : ICommand
    {
        [Inject] private ServerService _serverService;
        
        public void Execute()
        {
            _serverService.AcceptTerm();
        }
    }
    
    public class GetTermTextCommand : ICommandWithParameters
    {
        [Inject] private ServerService _serverService;
        
        public void Execute(ISignal signal)
        {
            var parameter = (GetTermTextCommandSignal) signal;

            _serverService.GetTermText(parameter.CultureCode, parameter.Callback);
        }
    }
}

namespace TDL.Signals
{
    public class GetTermTextCommandSignal : ISignal
    {
        public string CultureCode { get; private set; }
        public Action<string> Callback { get; private set; }

        public GetTermTextCommandSignal(string cultureCode, Action<string> callback)
        {
            CultureCode = cultureCode;
            Callback = callback;
        }
    }
    
    public class OpenTermCommandSignal : ISignal
    {
        public bool IsAcceptable { get; private set; }
        public OpenTermCommandSignal(bool isAcceptable)
        {
            IsAcceptable = isAcceptable;
        }
    }
    
    public class ValidateTermCommandSignal : ISignal
    {
        public Action<bool>  ValidateTermAction { get; private set; }

        public ValidateTermCommandSignal(Action<bool>  validateTermAction)
        {
            ValidateTermAction = validateTermAction;
        }
    }
    
    public class AcceptTermCommandSignal : ISignal
    {
        
    }
}