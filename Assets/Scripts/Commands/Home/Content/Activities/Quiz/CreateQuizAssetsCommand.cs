using System.Linq;
using Zenject;
using TDL.Models;
using TDL.Server;
using TDL.Views;

namespace TDL.Commands
{
    public class CreateQuizAssetsCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly ContentModel _contentModel;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private readonly QuizAssetView.Pool _pool;
        [Inject] private UserLoginModel _userLoginModel;

        public void Execute()
        {
            var quizAssets = _contentModel.GetQuizzesInCurrentCategory(_localizationModel.CurrentLanguageCultureCode, _localizationModel.FallbackCultureCode);
            var isTeacher = _userLoginModel.IsTeacher;

            foreach (var quizAsset in quizAssets)
            {
                foreach (var quiz in quizAsset.Quiz)
                {
                    var assetView = _pool.Spawn(_homeModel.AssetsContent);
                    assetView.transform.SetParent(_homeModel.AssetsContent, false);
                    assetView.transform.SetAsLastSibling();
                    assetView.Id = quizAsset.Asset.Id;
                    assetView.SelectedItemId = quiz.itemId;

                    SetLocalizedName(quiz, assetView);
                    SetDownloadStatus(assetView);
                    SaveAssetOnHomeScreen(assetView);

                    if (DeviceInfo.IsPCInterface())
                    {
                        SetTooltip(assetView);
                        SetFeedbackAvailability(isTeacher, assetView);
                    }   
                }
            }
        }

        private void SetLocalizedName(ActivityItem assetDetailModel, QuizAssetView assetView)
        {
            var localizedName = assetDetailModel.activityLocal.ToDictionary(assetLocale => assetLocale.Culture, assetLocale => assetLocale.Name); 
            
            if (localizedName.Count > 0)
            {
                assetView.Title.text = localizedName.ContainsKey(_localizationModel.CurrentLanguageCultureCode)
                    ? localizedName[_localizationModel.CurrentLanguageCultureCode]
                    // ToDo remove this later, server should always have FallbackCultureCode (en-US)
                    : localizedName.ContainsKey(_localizationModel.FallbackCultureCode) 
                        ? localizedName[_localizationModel.FallbackCultureCode]
                        : localizedName.First().Value;
            }
            else
            {
                assetView.Title.text = localizedName.First().Value + " NO TRANSLATION";
            }
        }

        private void SetDownloadStatus(QuizAssetView assetView)
        {
            var isDownloading = _homeModel.AssetsToDownload.Any(item => item.Id == assetView.Id);
            assetView.ShowProgressSlider(isDownloading);
        }

        private void SaveAssetOnHomeScreen(QuizAssetView assetView)
        {
            _homeModel.ShownQuizAssetsOnHome.Add(assetView);
        }
        
        private void SetTooltip(QuizAssetView quizView)
        {
            var tooltip = quizView.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(quizView.Title.text);
        }
        
        private void SetFeedbackAvailability(bool isAvailable, QuizAssetView assetView)
        {
            assetView.SetFeedbackAvailability(isAvailable);
        }
    }   
}