using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class SelectTopicCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;

        public void Execute(ISignal signal)
        {
            var parameter = (TopicItemClickCommandSignal) signal;

            _contentModel.SelectedTopic = _contentModel.GetTopicById(parameter.Id);
        }
    }
}