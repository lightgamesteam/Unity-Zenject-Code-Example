
namespace TDL.Signals
{
    public class ProcessAssetDetailsCommandSignal : ISignal
    {
        public int Id { get; private set; }

        public ProcessAssetDetailsCommandSignal(int id)
        {
            Id = id;
        }
    }
}