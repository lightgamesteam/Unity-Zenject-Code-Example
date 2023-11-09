using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using UnityEngine.SceneManagement;
using Zenject;

namespace TDL.Commands
{
    public class RunModuleCommand : ICommandWithParameters
    {
        [Inject] private readonly SignalBus _signalBus;
        [Inject] private readonly LocalizationModel _localizationModel;

        public void Execute(ISignal signal)
        {
            var parameter = (RunModuleCommandSignal) signal;

            ShowLoadingPopup();
            LoadModule(parameter.ModuleName);
        }

        private void ShowLoadingPopup()
        {
            _signalBus.Fire(new PopupOverlaySignal(true, _localizationModel.CurrentSystemTranslations[LocalizationConstants.LoadingKey]));
        }

        private void LoadModule(string moduleName)
        {
            SceneManager.LoadSceneAsync(ModuleConstants.Module_Prefix + moduleName, LoadSceneMode.Additive);
        }
    }
}