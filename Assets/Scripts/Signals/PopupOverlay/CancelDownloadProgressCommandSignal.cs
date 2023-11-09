
namespace TDL.Signals
{
    public class CancelDownloadProgressCommandSignal : ISignal
    {
        public int Id { get; private set; }

        public CancelDownloadProgressCommandSignal(int id)
        {
            Id = id;
        }
    }
}