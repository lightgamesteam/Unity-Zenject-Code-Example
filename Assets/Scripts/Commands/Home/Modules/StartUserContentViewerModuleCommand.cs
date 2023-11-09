using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using UnityEngine.SceneManagement;
using Zenject;

namespace TDL.Commands
{
    public class StartUserContentViewerModuleCommand : ICommandWithParameters
    {
        [Inject] private readonly SignalBus _signalBus;
        [Inject] private readonly LocalizationModel _localizationModel;
        [Inject] private UserContentAppModel _userContentAppModel;

        public void Execute(ISignal signal)
        {
            var parameter = (StartUserContentViewerModuleCommandSignal) signal;

            _userContentAppModel.SetCurrentUserContentId(parameter.Id);
            ShowLoadingPopup();
            LoadModule();
        }

        private void ShowLoadingPopup()
        {
            _signalBus.Fire(new PopupOverlaySignal(true, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.LoadingKey)));
        }

        private void LoadModule()
        {
            SceneManager.LoadSceneAsync(ModuleConstants.Module_Prefix + ModuleConstants.Module_UserContentViewer, LoadSceneMode.Additive);
        }
    }
}

namespace TDL.Signals
{
    public class StartUserContentViewerModuleCommandSignal : ISignal
    {
        public int Id { get; private set; }
        
        public StartUserContentViewerModuleCommandSignal(int id)
        {
            Id = id;
        }
    }
}