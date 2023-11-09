
namespace TDL.Signals
{
    public class TopicItemClickViewSignal : ISignal
    {
        public int ParentId { get; private set; }
        public int Id { get; private set; }

        public TopicItemClickViewSignal(int parentId, int id)
        {
            ParentId = parentId;
            Id = id;
        }
    }
}