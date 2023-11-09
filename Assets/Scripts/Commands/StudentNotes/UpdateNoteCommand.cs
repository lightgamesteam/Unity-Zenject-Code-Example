using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class UpdateNoteCommand : ICommandWithParameters
    {
        [Inject] private readonly ServerService _serverService;

        public void Execute(ISignal signal)
        {
            var parameter = (UpdateNoteCommandSignal) signal;

            _serverService.UpdateNote(parameter.NoteId, parameter.AssetId, parameter.Note);
        }
    }
}

public class UpdateNoteCommandSignal : ISignal {

    public int NoteId { get; private set; }
    public int AssetId { get; private set; }
    public string Note { get; private set; }
    
    public UpdateNoteCommandSignal(int noteId, int assetId, string note)
    {
        NoteId = noteId;
        AssetId = assetId;
        Note = note;
    }
}