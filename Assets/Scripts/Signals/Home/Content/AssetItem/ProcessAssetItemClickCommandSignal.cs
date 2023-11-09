
namespace TDL.Signals
{
    public class ProcessAssetItemClickCommandSignal : ISignal
    {
        public int Id { get; private set; }

        public ProcessAssetItemClickCommandSignal(int id)
        {
            Id = id;
        }
    }
}