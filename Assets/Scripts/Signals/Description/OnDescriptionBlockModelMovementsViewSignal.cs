
namespace TDL.Signals
{
    public class OnDescriptionBlockModelMovementsViewSignal : ISignal
    {
        public bool Status { get; }

        public OnDescriptionBlockModelMovementsViewSignal(bool status)
        {
            Status = status;
        }
    } 
}