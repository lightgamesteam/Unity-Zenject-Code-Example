using Newtonsoft.Json;
using TDL.Server;
using TDL.Services;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class AddNoteCommand : ICommandWithParameters
    {
        [Inject] private readonly ServerService _serverService;

        public void Execute(ISignal signal)
        {
            var parameter = (AddNoteCommandSignal) signal;

            _serverService.AddNote(parameter.Note, parameter.AssetId, parameter.GradeId, parameter.SubjectId, parameter.TopicId, parameter.SubtopicId);
        }
    }
    
    public class AddNoteResponseCommand : ICommandWithParameters
    {
        [Inject] private readonly SignalBus _signal;
        
        public void Execute(ISignal signal)
        {
            var parameter = (AddNoteResponseCommandSignal) signal;

            var response = JsonConvert.DeserializeObject<NoteEditResponse>(parameter.Response);

            if (response != null)
            {
                if (response.Success)
                {
                    _signal.Fire(new AddNoteSpawnSignal(response.assetNote.assetId, response.assetNote.noteId));
                }
                else
                {
                    _signal.Fire(new PopupOverlaySignal(false));
                    _signal.Fire(new SendCrashAnalyticsCommandSignal("AddNoteResponseCommand server response | " + response.ErrorMessage));
                }
            }
            else
            {
                _signal.Fire(new PopupOverlaySignal(false));
                _signal.Fire(new SendCrashAnalyticsCommandSignal("AddNoteResponseCommand server response | NULL"));
            }
        }
    }
}

namespace TDL.Signals
{
    public class AddNoteCommandSignal : ISignal 
    {
        public string Note { get; private set; }
        public int AssetId { get; private set; }
        public int GradeId { get; private set; }
        public int SubjectId { get; private set; }
        public int TopicId { get; private set; }
        public int SubtopicId { get; private set; }
 
        public AddNoteCommandSignal(string note, int assetId,  int gradeId = -1, int subjectId = -1, int topicId = -1, int subtopicId = -1)
        {
            Note = note;
            AssetId = assetId;
            GradeId = gradeId;
            SubjectId = subjectId;
            TopicId = topicId;
            SubtopicId = subtopicId;
        }
    }
 
    public class AddNoteResponseCommandSignal : ISignal 
    {
        public string Response { get; private set; }

        public AddNoteResponseCommandSignal(string response)
        {
            Response = response;
        }
    }

    public class AddNoteSpawnSignal : ISignal 
    {
        public int AssetId { get; private set; }

        public int NoteId { get; private set; }
    
        public AddNoteSpawnSignal(int assetId, int noteId)
        {
            AssetId = assetId;
            NoteId = noteId;
        }
    }
}
