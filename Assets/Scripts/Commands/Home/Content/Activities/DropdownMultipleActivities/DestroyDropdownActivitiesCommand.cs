using TDL.Signals;
using UnityEngine;

namespace TDL.Commands
{
    public class DestroyDropdownActivitiesCommand : ICommandWithParameters
    {
        public void Execute(ISignal signal)
        {
            var parameter = (DestroyDropdownActivitiesCommandSignal) signal;
            var foundedView = parameter.AssetItem;

            if (foundedView.HasDropdownMultipleQuiz)
            {
                DestroyDropdownItems(foundedView.DropdownMultipleQuizContainer);
            }

            if (foundedView.HasDropdownMultiplePuzzle)
            {
                DestroyDropdownItems(foundedView.DropdownMultiplePuzzleContainer);
            }
        }

        private void DestroyDropdownItems(Transform dropdownContainer)
        {
            foreach (Transform puzzleItem in dropdownContainer.transform)
            {
                dropdownContainer.GetComponent<CanvasGroup>().alpha = 0;
                GameObject.Destroy(puzzleItem.gameObject);
            }
        }
    }
}