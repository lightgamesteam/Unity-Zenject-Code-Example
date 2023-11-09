using Newtonsoft.Json;
using TDL.Server;
using TDL.Services;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class GetAllNotesCommand : ICommandWithParameters
    {
        [Inject] private readonly ServerService _serverService;

        public void Execute(ISignal signal)
        {
            var parameter = (GetAllNotesCommandSignal) signal;

            _serverService.GetAllNotes(parameter.AssetId);
        }
    }
    
    public class GetAllNotesResponseCommand : ICommandWithParameters
    {
        [Inject] private readonly SignalBus _signal;

        public void Execute(ISignal signal)
        {
            var parameter = (GetAllNotesResponseCommandSignal) signal;

            var response = JsonConvert.DeserializeObject<NoteResponse>(parameter.Response);

            if (response != null)
            {
                if (response.Success)
                {
                    _signal.Fire(new GetAllNotesSpawnSignal(response));
                }
                else
                {
                    _signal.Fire(new PopupOverlaySignal(false));
                    _signal.Fire(new SendCrashAnalyticsCommandSignal("GetAllNotesResponseCommand server response | " + response.ErrorMessage));
                }
            }
            else
            {
                _signal.Fire(new PopupOverlaySignal(false));
                _signal.Fire(new SendCrashAnalyticsCommandSignal("GetAllNotesResponseCommand server response | NULL"));
            }
        }
    }
}

    public class GetAllNotesCommandSignal : ISignal {

        public int AssetId { get; private set; }

        public GetAllNotesCommandSignal(int assetId, string note = "")
        {
            AssetId = assetId;
        }
    }

    public class GetAllNotesResponseCommandSignal : ISignal {

        public string Response { get; private set; }

        public GetAllNotesResponseCommandSignal(string response)
        {
            Response = response;
        }
    }

    public class  GetAllNotesSpawnSignal : ISignal
    {
        public NoteResponse NoteResponse { get; private set; }
        
        public GetAllNotesSpawnSignal(NoteResponse noteResponse)
        {
            NoteResponse = noteResponse;
        }
    }