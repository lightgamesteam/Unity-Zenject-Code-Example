using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class SelectSubtopicCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;

        public void Execute(ISignal signal)
        {
            var parameter = (SubtopicItemClickCommandSignal) signal;

            _contentModel.SelectedSubtopic = _contentModel.GetSubtopicById(parameter.Id);
        }
    }
}