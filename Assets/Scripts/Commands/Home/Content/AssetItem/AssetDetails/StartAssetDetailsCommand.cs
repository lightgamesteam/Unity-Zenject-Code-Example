using TDL.Constants;
using TDL.Models;
using UnityEngine;
using Zenject;

namespace TDL.Signals
{
    public class StartAssetDetailsCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private SignalBus _signal;
        [Inject] private LocalizationModel _localizationModel;

        public void Execute(ISignal signal)
        {
            var parameter = (StartAssetDetailsCommandSignal) signal;

            foreach (var id in parameter.Ids)
            {
                _signal.Fire(new PopupOverlaySignal(true, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.LoadingDetailsKey)));

                if (_contentModel.SelectedGrade == null)
                {
                    Debug.Log($"@@@ AssetDetails: AssetId: {id.AssetId} - GradeId: {id.GradeId}");
                    _signal.Fire(new DownloadAssetDetailsCommandSignal(id.AssetId, id.GradeId));
                }
                else
                {
                    Debug.Log($"@@@ AssetDetails: AssetId: {id.AssetId} - SelectedGrade.Id: {_contentModel.SelectedGrade.Id}");
                    _signal.Fire(new DownloadAssetDetailsCommandSignal(id.AssetId, _contentModel.SelectedGrade.Id));
                }
            }
        }
    }
}