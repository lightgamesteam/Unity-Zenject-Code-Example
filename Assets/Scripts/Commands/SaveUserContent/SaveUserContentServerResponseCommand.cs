using Newtonsoft.Json;
using TDL.Server;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class SaveUserContentServerResponseCommand : ICommandWithParameters
    {
        [Inject] private readonly SignalBus _signal;

        public void Execute(ISignal signal)
        {
            var parameter = (SaveUserContentServerResponseCommandSignal) signal;

            ResponseBase response = JsonConvert.DeserializeObject<ResponseBase>(parameter.Response);

            if (response != null)
            {
                _signal.Fire(new SaveUserContentServerResponseSignal(response, true, 100));
            }
        }
    }
}

namespace TDL.Signals
{
    public class SaveUserContentServerResponseCommandSignal : ISignal 
    {
        public string Response { get; private set; }

        public SaveUserContentServerResponseCommandSignal(string response)
        {
            Response = response;
        }
    }
    
    public class SaveUserContentServerResponseSignal : ISignal 
    {
        public ResponseBase Response { get; private set; }
        public bool IsUploaded { get; private set; }

        public int Progress  { get; private set; }

        public SaveUserContentServerResponseSignal(ResponseBase response, bool isUploaded, int progress)
        {
            Response = response;
            IsUploaded = isUploaded;
            Progress = progress;
        }
    }
}