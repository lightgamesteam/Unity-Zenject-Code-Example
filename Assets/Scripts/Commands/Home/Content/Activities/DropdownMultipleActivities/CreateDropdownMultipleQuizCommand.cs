using System.Linq;
using TDL.Models;
using TDL.Server;
using TDL.Signals;
using TDL.Views;
using Zenject;

namespace TDL.Commands
{
    public class CreateDropdownMultipleQuizCommand : ICommandWithParameters
    {
        [Inject] private readonly ContentModel _contentModel;
        [Inject] private readonly LocalizationModel _localizationModel;
        [Inject] private readonly DropdownActivityItemView.Factory _activityItemPool;

        public void Execute(ISignal signal)
        {
            var parameter = (CreateDropdownMultipleQuizCommandSignal) signal;

            var asset = _contentModel.GetAssetById(parameter.AssetItem.AssetId);
            var foundedView = parameter.AssetItem;

            foreach (var quizModel in asset.Quiz)
            {
                var quizItemView = _activityItemPool.Create();
                quizItemView.transform.SetParent(foundedView.DropdownMultipleQuizContainer, false);
                quizItemView.SetActivityItemClickSource(new QuizClickViewSignal(asset.Asset.Id, quizModel.itemId));
                
                SetLocalizedTitle(quizModel, quizItemView);
            }

            RepositionDropdown(foundedView);
        }

        private void SetLocalizedTitle(ActivityItem quiz, DropdownActivityItemView activityItemView)
        {
            // ToDo temporary take "defaultLocale" unless the server won't have all locals
            var locale = quiz.activityLocal.FirstOrDefault(assetLocal => assetLocal.Culture.Equals(_localizationModel.CurrentLanguageCultureCode));
            var defaultLocale = quiz.activityLocal.FirstOrDefault(assetLocal => assetLocal.Culture.Equals(_localizationModel.FallbackCultureCode));
            activityItemView.SetTitle(locale != null ? locale.Name : defaultLocale.Name);
        }

        private void RepositionDropdown(AssetItemView foundedView)
        {
            var repositionComponent = foundedView.DropdownMultipleQuizContainer.GetComponent<FlipOutsideLayoutDynamicDropdown>();
            if (repositionComponent != null)
            {
                repositionComponent.RepositionDropdownManually();
            }
        }
    }
}