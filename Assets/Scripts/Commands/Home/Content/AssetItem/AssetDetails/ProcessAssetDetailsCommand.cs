using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class ProcessAssetDetailsCommand : ICommand
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private readonly ContentModel _contentModel;

        public void Execute()
        {
            HidePopupOverlay();

            var signalSource = _contentModel.AssetDetailsSignalSource;
            
            switch (signalSource)
            {
                case AssetItemClickCommandSignal targetSignal:
                    _signal.Fire(targetSignal);
                    break;
                
                case StartPuzzleCommandSignal targetSignal:
                    _signal.Fire(targetSignal);
                    break;
                
                case StartQuizCommandSignal targetSignal:
                    _signal.Fire(targetSignal);
                    break;
                
                case StartDownloadAssetCommandSignal targetSignal:
                    _signal.Fire(targetSignal);
                    break;
                
                case CancelDownloadProgressCommandSignal targetSignal:
                    _signal.Fire(targetSignal);
                    break;
                
                case ShowMainFeedbackPanelCommandSignal targetSignal:
                    _signal.Fire(targetSignal);
                    break;
                
                case StartMultipleQuizCommandSignal targetSignal:

                    if (IsMultipleAssetsDownloaded())
                    {
                        _signal.Fire(targetSignal);
                    }
                    
                    break;
                
                case StartMultiplePuzzleCommandSignal targetSignal:

                    if (IsMultipleAssetsDownloaded())
                    {
                        _signal.Fire(targetSignal);
                    }
                    
                    break;
                
                case GetClassificationDetailsCommandSignal targetSignal:

                    if (IsMultipleAssetsDownloaded())
                    {
                        _signal.Fire(targetSignal);
                    }
                    
                    break;
            }
        }

        private void HidePopupOverlay()
        {
            _signal.Fire(new PopupOverlaySignal(false));
        }

        private bool IsMultipleAssetsDownloaded()
        {
            return _contentModel.MultipleAssetDetailsIds.Count == 0;
        }
    }
}