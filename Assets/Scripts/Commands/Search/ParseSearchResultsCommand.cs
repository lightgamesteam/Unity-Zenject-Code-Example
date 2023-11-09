using Newtonsoft.Json;
using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class ParseSearchResultsCommand : ICommandWithParameters
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private ContentModel _contentModel;
        
        public void Execute(ISignal signal)
        {
            var parameter = (ParseSearchResultsCommandSignal) signal;

            var searchResponse = JsonConvert.DeserializeObject<int[]>(parameter.Response);
            _signal.Fire(new ShowSearchAssetsCommandSignal(searchResponse));
        }
    }
}