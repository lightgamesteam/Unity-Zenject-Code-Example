using System.Collections.Generic;
using TDL.Models;
using TDL.Server;
using Zenject;

namespace TDL.Commands
{
    public class SelectMultiplePuzzleAssetCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;

        public void Execute(ISignal signal)
        {
            var parameter = (StartMultiplePuzzleCommandSignal) signal;

            _contentModel.SelectedAsset = new ClientAssetModel
            {
                Puzzle = new List<ActivityItem> {_contentModel.GetMultiplePuzzleById(parameter.Id).ActivityItem},
                IsMultiplePuzzleSelected = true
            };
        }
    }
}