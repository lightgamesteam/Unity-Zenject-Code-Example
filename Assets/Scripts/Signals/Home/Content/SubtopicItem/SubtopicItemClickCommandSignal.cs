
namespace TDL.Signals
{
    public class SubtopicItemClickCommandSignal : ISignal
    {
        public int ParentId { get; private set; }
        public int Id { get; private set; }

        public SubtopicItemClickCommandSignal(int parentId, int id)
        {
            ParentId = parentId;
            Id = id;
        }
    }
}