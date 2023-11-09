using TDL.Constants;
using TDL.Models;
using TDL.Views;
using Zenject;

namespace TDL.Commands
{
    public class CreateScreenWithActivityAssetsCommand : ICommand
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly ActivityQuizView.Pool _quizPool;
        [Inject] private readonly ActivityPuzzleView.Pool _puzzlePool;
        [Inject] private readonly ActivityMultipleQuizView.Pool _multipleQuizPool;
        [Inject] private readonly ActivityMultiplePuzzlesView.Pool _multiplePuzzlePool;
        [Inject] private readonly ActivityClassificationView.Pool _classificationPool;
        [Inject] private LocalizationModel _localizationModel;
        
        public void Execute()
        {
            if (_contentModel.HasCategoryAnyQuiz())
            {
                CreateQuizActivity();
            }

            if (_contentModel.HasCategoryAnyPuzzle())
            {
                CreatePuzzleActivity();
            }

            if (_contentModel.HasCategoryAnyMultipleQuiz())
            {
                CreateMultipleQuizActivity();
            }

            if (_contentModel.HasCategoryAnyMultiplePuzzle())
            {
                CreateMultiplePuzzleActivity();
            }
            
            if (_contentModel.HasCategoryAnyClassifications())
            {
                CreateClassificationActivity();
            }
        }

        private void CreateQuizActivity()
        {
            var activityItemView = _quizPool.Spawn(_homeModel.TopicsSubtopicsContent);
            activityItemView.transform.SetParent(_homeModel.TopicsSubtopicsContent, false);
            activityItemView.transform.SetAsLastSibling();
            
            var localizedTitle = _localizationModel.CurrentSystemTranslations[LocalizationConstants.ActivityQuizzesKey];
            activityItemView.Title.text = localizedTitle;

            if (DeviceInfo.IsPCInterface())
            {
                var tooltip = activityItemView.gameObject.GetComponent<DynamicTooltipEvents>();
                tooltip.SetHint(localizedTitle);   
            }

            _homeModel.ShownActivitiesOnHome.Add(typeof(ActivityQuizView).Name, activityItemView);
        }

        private void CreatePuzzleActivity()
        {
            var activityItemView = _puzzlePool.Spawn(_homeModel.TopicsSubtopicsContent);
            activityItemView.transform.SetParent(_homeModel.TopicsSubtopicsContent, false);
            activityItemView.transform.SetAsLastSibling();
            
            var localizedTitle = _localizationModel.CurrentSystemTranslations[LocalizationConstants.ActivityPuzzlesKey];
            activityItemView.Title.text = localizedTitle;
            
            if (DeviceInfo.IsPCInterface())
            {
                var tooltip = activityItemView.gameObject.GetComponent<DynamicTooltipEvents>();
                tooltip.SetHint(localizedTitle); 
            }
            
            _homeModel.ShownActivitiesOnHome.Add(typeof(ActivityPuzzleView).Name, activityItemView);
        }

        private void CreateMultipleQuizActivity()
        {
            var activityItemView = _multipleQuizPool.Spawn(_homeModel.TopicsSubtopicsContent);
            activityItemView.transform.SetParent(_homeModel.TopicsSubtopicsContent, false);
            activityItemView.transform.SetAsLastSibling();
            
            var localizedTitle = _localizationModel.CurrentSystemTranslations[LocalizationConstants.ActivityMultipleQuizzesKey];
            activityItemView.Title.text = localizedTitle;
            
            if (DeviceInfo.IsPCInterface())
            {
                var tooltip = activityItemView.gameObject.GetComponent<DynamicTooltipEvents>();
                tooltip.SetHint(localizedTitle);
            }
            
            _homeModel.ShownActivitiesOnHome.Add(typeof(ActivityMultipleQuizView).Name, activityItemView);
        }

        private void CreateMultiplePuzzleActivity()
        {
            var activityItemView = _multiplePuzzlePool.Spawn(_homeModel.TopicsSubtopicsContent);
            activityItemView.transform.SetParent(_homeModel.TopicsSubtopicsContent, false);
            activityItemView.transform.SetAsLastSibling();
            
            var localizedTitle = _localizationModel.CurrentSystemTranslations[LocalizationConstants.ActivityMultiplePuzzlesKey];
            activityItemView.Title.text = localizedTitle;
            
            if (DeviceInfo.IsPCInterface())
            {
                var tooltip = activityItemView.gameObject.GetComponent<DynamicTooltipEvents>();
                tooltip.SetHint(localizedTitle);
            }
            
            _homeModel.ShownActivitiesOnHome.Add(typeof(ActivityMultiplePuzzlesView).Name, activityItemView);
        }
        
        private void CreateClassificationActivity()
        {
            var activityItemView = _classificationPool.Spawn(_homeModel.TopicsSubtopicsContent);
            activityItemView.transform.SetParent(_homeModel.TopicsSubtopicsContent, false);
            activityItemView.transform.SetAsLastSibling();

            var localizedTitle = _localizationModel.CurrentSystemTranslations[LocalizationConstants.ActivityClassificationsKey];
            activityItemView.Title.text = localizedTitle;
            
            if (DeviceInfo.IsPCInterface())
            {
                var tooltip = activityItemView.gameObject.GetComponent<DynamicTooltipEvents>();
                tooltip.SetHint(localizedTitle);
            }
            
            _homeModel.ShownActivitiesOnHome.Add(typeof(ActivityClassificationView).Name, activityItemView);
        }
    }
}