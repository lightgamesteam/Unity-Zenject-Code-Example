using System.Linq;
using TDL.Models;
using TDL.Server;
using TDL.Signals;
using TDL.Views;
using Zenject;

namespace TDL.Commands
{
    public class CreateDropdownMultiplePuzzleCommand : ICommandWithParameters
    {
        [Inject] private readonly ContentModel _contentModel;
        [Inject] private readonly LocalizationModel _localizationModel;
        [Inject] private readonly DropdownActivityItemView.Factory _activityItemPool;

        public void Execute(ISignal signal)
        {
            var parameter = (CreateDropdownMultiplePuzzleCommandSignal) signal;

            var asset = _contentModel.GetAssetById(parameter.AssetItem.AssetId);
            var foundedView = parameter.AssetItem;

            foreach (var puzzleModel in asset.Puzzle)
            {
                var puzzleItemView = _activityItemPool.Create();
                puzzleItemView.transform.SetParent(foundedView.DropdownMultiplePuzzleContainer, false);
                puzzleItemView.SetActivityItemClickSource(new PuzzleClickViewSignal(asset.Asset.Id, puzzleModel.itemId));
                
                SetLocalizedTitle(puzzleModel, puzzleItemView);
            }

            RepositionDropdown(foundedView);
        }

        private void SetLocalizedTitle(ActivityItem puzzle, DropdownActivityItemView activityItemView)
        {
            // ToDo temporary take "defaultLocale" unless the server won't have all locals
            var locale = puzzle.activityLocal.FirstOrDefault(assetLocal => assetLocal.Culture.Equals(_localizationModel.CurrentLanguageCultureCode));
            var defaultLocale = puzzle.activityLocal.FirstOrDefault(assetLocal => assetLocal.Culture.Equals(_localizationModel.FallbackCultureCode));
            activityItemView.SetTitle(locale != null ? locale.Name : defaultLocale.Name);
        }

        private void RepositionDropdown(AssetItemView foundedView)
        {
            var repositionComponent = foundedView.DropdownMultiplePuzzleContainer.GetComponent<FlipOutsideLayoutDynamicDropdown>();
            if (repositionComponent != null)
            {
                repositionComponent.RepositionDropdownManually();
            }
        }
    }
}