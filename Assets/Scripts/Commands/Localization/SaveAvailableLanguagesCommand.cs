using System.IO;
using System.Text;
using CI.TaskParallel;
using Managers;
using Newtonsoft.Json;
using TDL.Models;
using TDL.Server;
using TDL.Services;
using TDL.Signals;
using TDL.Views;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class SaveAvailableLanguagesCommand : ICommandWithParameters
    {
        [Inject] private ICacheService _cacheService;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private UserLoginModel _userLoginModel;
        [Inject] private readonly SignalBus _signal;

        public void Execute(ISignal signal)
        {
#if UNITY_WEBGL
            RunInBackgroundWebGL(signal);
#else
            RunInBackground(signal);
#endif
        }

        private void RunInBackground(ISignal signal)
        {
            var pathToSave = _cacheService.GetPathToFile(_localizationModel.FileAvailableLanguages);
            UnityTask.Run(() =>
            {
                var parameter = (SaveAvailableLanguagesCommandSignal) signal;
                var languagesResponse = JsonConvert.DeserializeObject<ResourceResponse>(parameter.LanguagesResponse);

                if (languagesResponse.Success)
                {
                    _localizationModel.AvailableLanguages = languagesResponse.Resources.Languages;
                    var allLanguages = JsonConvert.SerializeObject(_localizationModel.AvailableLanguages);
                    var jsonLanguagesBytes = Encoding.UTF8.GetBytes(allLanguages);

                    using (var fs = new FileStream(pathToSave, FileMode.OpenOrCreate))
                    {
                        fs.Write(jsonLanguagesBytes, 0, jsonLanguagesBytes.Length);
                    }
                }
                else
                {
                    _signal.Fire(new PopupOverlaySignal(false));
                    _signal.Fire(new SendCrashAnalyticsCommandSignal("SaveAvailableLanguagesCommand server response | " + languagesResponse.ErrorMessage));
                }
                
            }).ContinueOnUIThread(task =>
            {
                _userLoginModel.IsLoginScreenReady = true;
            });
        }
        
        private void RunInBackgroundWebGL(ISignal signal)
        {
            var pathToSave = _cacheService.GetPathToFile(_localizationModel.FileAvailableLanguages);

            var parameter = (SaveAvailableLanguagesCommandSignal) signal;
            var languagesResponse = JsonConvert.DeserializeObject<ResourceResponse>(parameter.LanguagesResponse);

            if (languagesResponse.Success)
            {
                _localizationModel.AvailableLanguages = languagesResponse.Resources.Languages;
                var allLanguages = JsonConvert.SerializeObject(_localizationModel.AvailableLanguages);
                var jsonLanguagesBytes = Encoding.UTF8.GetBytes(allLanguages);

                using (var fs = new FileStream(pathToSave, FileMode.OpenOrCreate))
                {
                    fs.Write(jsonLanguagesBytes, 0, jsonLanguagesBytes.Length);
                }
            }
            else
            {
                _signal.Fire(new PopupOverlaySignal(false));
                _signal.Fire(new SendCrashAnalyticsCommandSignal("SaveAvailableLanguagesCommand server response | " + languagesResponse.ErrorMessage));
            }
                
            _userLoginModel.IsLoginScreenReady = true;

        }
    }
}