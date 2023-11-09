using TDL.Models;
using System.Collections;
using TDL.Constants;
using TDL.Signals;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class StartDrawingToolModuleCommand : ICommandWithParameters 
{
    [Inject] private readonly LocalizationModel _localizationModel;
    [Inject] private readonly SignalBus _signal;
    [Inject] private AsyncProcessorService _asyncProcessor;

    private string _cultureCode;
    
    public void Execute(ISignal signal) {
        StartDrawingToolModuleCommandSignal _signal = (StartDrawingToolModuleCommandSignal) signal;
        _cultureCode = _signal.CultureCode;
        
        ShowPopupOverlay();
        _asyncProcessor.StartCoroutine(StartModule());
    }
    
    IEnumerator StartModule()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneNameConstants.ModuleDrawingTool, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        FindComponentExtension.GetFirstInScene<TooltipPanel>(SceneNameConstants.ModuleDrawingTool).FeedTooltipEvents(_cultureCode, _localizationModel.AllSystemTranslations[_cultureCode]);
    }
    
    private void ShowPopupOverlay() {
        _signal.Fire(new PopupOverlaySignal(true, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.LoadingKey)));
    }
}

public class StartDrawingToolModuleCommandSignal : ISignal 
{
    public string CultureCode { get; }
    
    public StartDrawingToolModuleCommandSignal(string cultureCode)
    {
        CultureCode = cultureCode;
    }
}