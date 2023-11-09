using System.Linq;
using TDL.Models;
using TDL.Server;
using TDL.Services;
using TDL.Views;
using Zenject;

namespace TDL.Commands
{
    public class CreateMultipleClassificationAssetsCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private ICacheService _cacheService;
        [Inject] private readonly ContentModel _contentModel;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private readonly ClassificationAssetItemView.Pool _pool;

        public void Execute()
        {
            var classifications = _contentModel.GetClassificationsInCurrentCategory();

            foreach (var asset in classifications)
            {
                var assetView = _pool.Spawn(_homeModel.AssetsContent);
                assetView.transform.SetParent(_homeModel.AssetsContent, false);
                assetView.transform.SetAsLastSibling();
                assetView.Id = asset.ActivityItem.itemId;

                SetLocalizedName(asset.ActivityItem, assetView);
                SetDownloadStatus(assetView);
                SaveAssetOnHomeScreen(assetView);

                if (DeviceInfo.IsPCInterface())
                {
                    SetTooltip(assetView);
                }
            }
        }

        private void SetLocalizedName(ActivityItem activityItem, ClassificationAssetItemView assetView)
        {
            if (activityItem.activityLocal.Length > 0)
            {
                assetView.Title.text = GetLocale(activityItem);
            }
            else
            {
                assetView.Title.text = activityItem.itemName + " NO TRANSLATION";
            }
        }

        private string GetLocale(ActivityItem activityItem)
        {
            var currentLocale = activityItem.activityLocal.FirstOrDefault(assetLocal => assetLocal.Culture.Equals(_localizationModel.CurrentLanguageCultureCode));

            if (currentLocale != null)
            {
                return currentLocale.Name;
            }

            var defaultLocale = activityItem.activityLocal.FirstOrDefault(assetLocal => assetLocal.Culture.Equals(_localizationModel.FallbackCultureCode));
            
            // ToDo add to localization constants
            return defaultLocale != null ? defaultLocale.Name : "NO TRANSLATION";
        }

        private void SetDownloadStatus(ClassificationAssetItemView assetView)
        {
            var isDownloading = _homeModel.MultipleAssetIdQueueToDownload.ContainsKey(assetView.Id);
            assetView.ShowProgressSlider(isDownloading);
        }

        private void SaveAssetOnHomeScreen(ClassificationAssetItemView assetView)
        {
            _homeModel.ShownClassificationAssetsOnHome.Add(assetView);
        }
        
        private void SetTooltip(ClassificationAssetItemView classificationView)
        {
            var tooltip = classificationView.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(classificationView.Title.text);
        }
    }
}