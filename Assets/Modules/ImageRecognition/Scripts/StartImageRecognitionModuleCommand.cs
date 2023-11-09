using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using UnityEngine.SceneManagement;
using Zenject;

public class StartImageRecognitionModuleCommand : ICommand
{
    [Inject] private LocalizationModel _localizationModel;
    [Inject] private readonly SignalBus _signal;

    public void Execute()
    {
        StartModule();
        ShowPopupOverlay();
    }
    
    private void StartModule()
    {
        SceneManager.LoadSceneAsync(SceneNameConstants.ModuleImageRecognition, LoadSceneMode.Additive);
    }
    
    private void ShowPopupOverlay()
    {
        _signal.Fire(new PopupOverlaySignal(true, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.LoadingKey)));
    }
}

public class StartImageRecognitionModuleCommandSignal : ISignal
{
}