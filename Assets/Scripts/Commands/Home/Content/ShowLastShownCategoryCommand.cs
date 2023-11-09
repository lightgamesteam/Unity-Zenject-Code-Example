using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class ShowLastShownCategoryCommand : ICommand
    {
        [Inject] private readonly HomeModel _homeModel;
        [Inject] private ContentModel _contentModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private readonly UserContentAppModel _userContentAppModel;

        public void Execute()
        {
            ShowLastCategory();
            ShowLastActivity();
        }

        private void ShowLastCategory()
        {
            var lastShownCategorySignal = _homeModel.LastShownCategory;

            if (lastShownCategorySignal is CreateSubjectsContentCommandSignal)
            {
                _signal.Fire<CreateSubjectsContentCommandSignal>();
            }
            else if (lastShownCategorySignal is CreateTopicsContentCommandSignal)
            {
                _signal.Fire<CreateTopicsContentCommandSignal>();
            }
            else if (lastShownCategorySignal is CreateSubtopicsContentCommandSignal)
            {
                _signal.Fire<CreateSubtopicsContentCommandSignal>();
            }
            else if (lastShownCategorySignal is CreateAssetsCommandSignal)
            {
                _signal.Fire<CreateAssetsCommandSignal>();
            }
            else if (lastShownCategorySignal is ShowRecentlyViewedCommandSignal)
            {
                _signal.Fire<ShowRecentlyViewedCommandSignal>();
            }
            else if (lastShownCategorySignal is ShowFavouritesCommandSignal)
            {
                _signal.Fire<ShowFavouritesCommandSignal>();
            }
            else if (lastShownCategorySignal is CreateActivitiesScreenCommandSignal)
            {
                _signal.Fire<CreateActivitiesScreenCommandSignal>();
            }
            else if (lastShownCategorySignal is CreateQuizAssetsCommandSignal)
            {
                _signal.Fire<CreateQuizAssetsCommandSignal>();
            }
            else if (lastShownCategorySignal is CreatePuzzleAssetsCommandSignal)
            {
                _signal.Fire<CreatePuzzleAssetsCommandSignal>();
            }
            else if (lastShownCategorySignal is CreateMultipleQuizAssetsCommandSignal)
            {
                _signal.Fire<CreateMultipleQuizAssetsCommandSignal>();
            }
            else if (lastShownCategorySignal is CreateMultiplePuzzleAssetsCommandSignal)
            {
                _signal.Fire<CreateMultiplePuzzleAssetsCommandSignal>();
            }
            else if (lastShownCategorySignal is CreateClassificationAssetsCommandSignal)
            {
                _signal.Fire<CreateClassificationAssetsCommandSignal>();
            } 
            else if (lastShownCategorySignal is ShowMyContentViewedCommandSignal)
            {
                if (_userContentAppModel.IsTeacherContent)
                {
                    _signal.Fire<GetTeacherContentCommandSignal>();
                }
                else
                {
                    _signal.Fire<GetUserContentCommandSignal>();
                }
            }
        }

        private void ShowLastActivity()
        {
            var lastShownActivitySignal = _homeModel.LastShownActivity;

            if (lastShownActivitySignal is CreateActivityItemCommandSignal)
            {
                _signal.Fire<CreateActivityItemCommandSignal>();
            }
            else if (lastShownActivitySignal is CreateActivitiesScreenWithHeadersCommandSignal)
            {
                _signal.Fire<CreateActivitiesScreenWithHeadersCommandSignal>();
            }
        }
    }
}