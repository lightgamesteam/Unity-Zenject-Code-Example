using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
	public class SelectQuizAssetCommand : ICommandWithParameters
	{
		[Inject] 
		private ContentModel _contentModel;
    
		public void Execute(ISignal signal)
		{
			var parameter = (StartQuizCommandSignal) signal;

			_contentModel.SelectedAsset = _contentModel.GetAssetById(parameter.Id);
			_contentModel.SelectedAsset.IsQuizSelected = true;
			_contentModel.SelectedAsset.SelectedQuizItemId = parameter.SelectedQuizItemId;
		}
	}
}