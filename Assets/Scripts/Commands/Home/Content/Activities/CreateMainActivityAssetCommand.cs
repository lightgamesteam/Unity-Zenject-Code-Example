using System.Linq;
using TDL.Constants;
using TDL.Models;
using TDL.Views;
using Zenject;

namespace TDL.Commands
{
    public class CreateMainActivityAssetCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly ActivityItemView.Pool _activityPool;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private ContentModel _contentModel;

        public void Execute()
        {
            var shouldCreateActivity = _homeModel.ShownActivitiesOnHome.Values.All(item => item.GetType() != typeof(ActivityItemView));

            if (shouldCreateActivity && _contentModel.HasCategoryAnyActivity())
            {
                var activitiesItemView = _activityPool.Spawn(_homeModel.TopicsSubtopicsContent);
                activitiesItemView.transform.SetParent(_homeModel.TopicsSubtopicsContent, false);
                activitiesItemView.transform.SetAsLastSibling();

                var localizedTitle = _localizationModel.CurrentSystemTranslations[LocalizationConstants.ActivitiesKey];
                activitiesItemView.Title.text = localizedTitle;

                if (DeviceInfo.IsPCInterface())
                {
                    SetTooltip(activitiesItemView, localizedTitle);
                }

                _homeModel.ShownActivitiesOnHome.Add(typeof(ActivityItemView).Name, activitiesItemView);
            }
        }
    
        private void SetTooltip(ActivityItemView view, string localizedTitle)
        {
            var tooltip = view.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(localizedTitle);
        }
    }
}