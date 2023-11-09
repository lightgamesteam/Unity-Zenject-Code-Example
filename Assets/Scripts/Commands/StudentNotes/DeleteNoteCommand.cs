using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class DeleteNoteCommand : ICommandWithParameters
    {
        [Inject] private readonly ServerService _serverService;

        public void Execute(ISignal signal)
        {
            var parameter = (DeleteNoteCommandSignal) signal;

            _serverService.DeleteNote(parameter.NoteId);
        }
    }
}

public class DeleteNoteCommandSignal : ISignal {

    public int NoteId { get; private set; }

    public DeleteNoteCommandSignal(int noteId)
    {
        NoteId = noteId;
    }
}