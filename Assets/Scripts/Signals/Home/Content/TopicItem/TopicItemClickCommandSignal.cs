
namespace TDL.Signals
{
    public class TopicItemClickCommandSignal : ISignal
    {
        public int ParentId { get; private set; }
        public int Id { get; private set; }

        public TopicItemClickCommandSignal(int parentId, int id)
        {
            ParentId = parentId;
            Id = id;
        }
    }
}