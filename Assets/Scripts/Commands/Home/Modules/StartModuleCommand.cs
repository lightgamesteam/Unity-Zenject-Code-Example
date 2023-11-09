using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class StartModuleCommand : ICommand
    {
        [Inject] private readonly ContentModel _contentModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private LocalizationModel _localizationModel;

        public void Execute()
        {
            var assetType = _contentModel.SelectedAsset.Asset.Type;

            switch (assetType.ToLower())
            {
                case AssetTypeConstants.Type_3D_Model:
                    if (_contentModel.SelectedAsset.IsPuzzleSelected)
                    {
                        _signal.Fire(new StartModuleAssetContentCommandSignal(ModuleConstants.Module_Prefix + ModuleConstants.Module_Puzzle,
                            _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.LoadingPuzzleKey)));
                    }
                    else if (_contentModel.SelectedAsset.IsQuizSelected)
                    {
                        _signal.Fire(new StartModuleAssetContentCommandSignal(ModuleConstants.Module_Prefix + ModuleConstants.Module_Quiz,
                            _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.LoadingQuizKey)));
                    }
                    else
                    {
                        _signal.Fire(new StartModuleAssetContentCommandSignal(ModuleConstants.Module_Prefix + ModuleConstants.Module_3D_Model,
                            _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.Loading3DModelKey)));
                    }

                    break;

                case AssetTypeConstants.Type_3D_Video:
                    _signal.Fire(new StartModuleAssetContentCommandSignal(ModuleConstants.Module_Prefix + ModuleConstants.Module_3D_Video,
                        _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.Loading3DVideoKey)));
                    break;

                case AssetTypeConstants.Type_2D_Video:
                    _signal.Fire(new StartModuleAssetContentCommandSignal(ModuleConstants.Module_Prefix + ModuleConstants.Module_2D_Video,
                        _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.Loading2DVideoKey)));
                    break;
                
                case AssetTypeConstants.Type_Multilayered:
                    _signal.Fire(new StartModuleAssetContentCommandSignal(ModuleConstants.Module_Prefix + ModuleConstants.Module_Ultimate,
                        _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.Loading3DModelKey)));
                    break;

                case AssetTypeConstants.Type_Module:

                    // ToDo temporary decision unless all modules will be externalized
                    if (IsExternalized())
                    {
                        _signal.Fire(new StartModuleAssetContentCommandSignal(ModuleConstants.Module_Prefix + _contentModel.SelectedAsset.Asset.Name,
                            _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.LoadingKey)));
                    }
                    else if (!_contentModel.SelectedAsset.Asset.Name.Equals(ModuleConstants.Module_ECG))
                    {
                        _signal.Fire(new RunModuleCommandSignal(_contentModel.SelectedAsset.Asset.Name));
                    }
                    
                    break;
                
                case AssetTypeConstants.Type_Rigged_Model:
                    _signal.Fire(new StartModuleAssetContentCommandSignal(ModuleConstants.Module_Prefix + ModuleConstants.Module_3D_Model,
                        _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.Loading3DModelKey)));
                    break;
                case AssetTypeConstants.Type_360_Model:
                    _signal.Fire(new StartModuleAssetContentCommandSignal(ModuleConstants.Module_Prefix + ModuleConstants.Module_3D_Model,
                        _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.Loading3DModelKey)));
                    break;
            }
        }

        private bool IsExternalized()
        {
            return _contentModel.SelectedAsset.Asset.Name.Equals(ModuleConstants.Module_PeriodicTable)
                   || _contentModel.SelectedAsset.Asset.Name.Equals(ModuleConstants.Module_SolarSystem)
                   || _contentModel.SelectedAsset.Asset.Name.Equals(ModuleConstants.Module_Astronomy);
        }
    }
}