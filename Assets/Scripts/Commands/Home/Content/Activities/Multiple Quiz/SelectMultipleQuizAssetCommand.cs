using System.Collections.Generic;
using TDL.Models;
using TDL.Server;
using Zenject;

namespace TDL.Commands
{
    public class SelectMultipleQuizAssetCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;

        public void Execute(ISignal signal)
        {
            var parameter = (StartMultipleQuizCommandSignal) signal;

            _contentModel.SelectedAsset = new ClientAssetModel
            {
                Quiz = new List<ActivityItem> {_contentModel.GetMultipleQuizById(parameter.Id).ActivityItem},
                IsMultipleQuizSelected = true
            };
        }
    }
}