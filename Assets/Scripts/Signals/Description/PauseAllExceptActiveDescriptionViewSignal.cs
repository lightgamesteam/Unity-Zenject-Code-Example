
namespace TDL.Signals
{
    public class PauseAllExceptActiveDescriptionViewSignal : ISignal
    {
        public string Id { get; }

        public PauseAllExceptActiveDescriptionViewSignal(string id)
        {
            Id = id;
        }
    } 
}