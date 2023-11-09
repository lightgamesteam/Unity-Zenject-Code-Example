using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class StartMultipleModuleCommand : ICommand
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private readonly ContentModel _contentModel;
        [Inject] private LocalizationModel _localizationModel;

        public void Execute()
        {
            if (_contentModel.SelectedAsset.IsMultipleQuizSelected)
            {
                _signal.Fire(new StartMultipleModuleAssetContentCommandSignal(ModuleConstants.Module_Prefix + ModuleConstants.Module_Quiz,
                    _localizationModel.CurrentSystemTranslations[LocalizationConstants.LoadingMultipleQuizKey]));
            }
            else if (_contentModel.SelectedAsset.IsMultiplePuzzleSelected)
            {
                _signal.Fire(new StartMultipleModuleAssetContentCommandSignal(ModuleConstants.Module_Prefix + ModuleConstants.Module_Puzzle,
                    _localizationModel.CurrentSystemTranslations[LocalizationConstants.LoadingMultiplePuzzleKey]));
            }
            else if (_contentModel.SelectedAsset.IsClassificationSelected)
            {
                _signal.Fire(new StartMultipleModuleAssetContentCommandSignal(ModuleConstants.Module_Prefix + ModuleConstants.Module_Classification,
                    _localizationModel.CurrentSystemTranslations[LocalizationConstants.LoadingClassificationKey]));
            }
        }
    }
}