using System.Linq;
using TDL.Constants;
using TDL.Models;
using TDL.Server;
using TDL.Signals;
using TDL.Views;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class CreateDropdownMultipleQuizMobileCommand : ICommandWithParameters
    {
        [Inject] private readonly ContentModel _contentModel;
        [Inject] private readonly DropdownActivityHeaderMobileView.Factory _headerPool;
        [Inject] private readonly DropdownActivityContainerMobileView.Factory _containerPool;
        [Inject] private readonly DropdownActivityItemMobileView.Factory _activityItemPool;
        [Inject] private readonly LocalizationModel _localizationModel;
        
        private DropdownActivityContainerMobileView _dropdownContainer;

        public void Execute(ISignal signal)
        {
            var parameter = (CreateDropdownMultipleQuizMobileCommandSignal) signal;

            CreateHeader(parameter.ContentPanel.container);
            CreateItemsContainer(parameter.ContentPanel.container);
            CreateActivityItems(parameter);
        }
        
        private void CreateHeader(Transform parentContainer)
        {
            var header = _headerPool.Create();
            header.transform.SetParent(parentContainer, false);
            SetLocalizedHeader(header);
        }

        private void CreateItemsContainer(Transform parentContainer)
        {
            _dropdownContainer = _containerPool.Create();
            _dropdownContainer.transform.SetParent(parentContainer, false);
        }

        private void CreateActivityItems(CreateDropdownMultipleQuizMobileCommandSignal parameter)
        {
            var asset = _contentModel.GetAssetById(parameter.AssetId);
            foreach (var activityItem in asset.Quiz)
            {
                var activityView = _activityItemPool.Create();
                activityView.transform.SetParent(_dropdownContainer.transform, false);
                activityView.SetCanvas(parameter.ContentPanel.canvasPanel);
                activityView.SetActivityItemClickSource(new QuizAssetItemClickViewSignal(asset.Asset.Id, activityItem.itemId));
                SetLocalizedItem(activityItem, activityView);
            }
        }

        private void SetLocalizedHeader(DropdownActivityHeaderMobileView headerView)
        {
            headerView.SetTitle(_localizationModel.CurrentSystemTranslations[LocalizationConstants.MoreQuizKey]);
        }
        
        private void SetLocalizedItem(ActivityItem activity, DropdownActivityItemMobileView activityItemView)
        {
            // ToDo temporary take "defaultLocale" unless the server won't have all locals
            var locale = activity.activityLocal.FirstOrDefault(assetLocal => assetLocal.Culture.Equals(_localizationModel.CurrentLanguageCultureCode));
            var defaultLocale = activity.activityLocal.FirstOrDefault(assetLocal => assetLocal.Culture.Equals(_localizationModel.FallbackCultureCode));
            activityItemView.SetTitle(locale != null ? locale.Name : defaultLocale.Name);
        }
    }
}