using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class ShowMainFeedbackPanelCommand : ICommandWithParameters
    {
        [Inject] private FeedbackModel _feedbackModel;
        [Inject] private ContentModel _contentModel;
        [Inject] private LocalizationModel _localizationModel;

        public void Execute(ISignal signal)
        {
            var parameter = (ShowMainFeedbackPanelCommandSignal) signal;
            var assetId = parameter.Id;
            var currentFeedbackType = parameter.Type;

            var asset = _contentModel.GetAssetById(assetId);

            _feedbackModel.AssetId = assetId;

            if (asset != null)
            {
                _feedbackModel.AssetContentId = asset.AssetDetail.AssetContentPlatform.Id;

                _feedbackModel.Title = asset.LocalizedName.ContainsKey(_localizationModel.CurrentLanguageCultureCode)
                    ? asset.LocalizedName[_localizationModel.CurrentLanguageCultureCode]
                    : asset.LocalizedName[_localizationModel.FallbackCultureCode];
            }
            else
            {
                _feedbackModel.AssetContentId = -1;
                _feedbackModel.Title = _localizationModel.CurrentSystemTranslations[LocalizationConstants.ActivityIdNotFoundKey];
            }

            _feedbackModel.Type = currentFeedbackType;
            _feedbackModel.IsMainFeedbackActive = true;
        }
    }
}