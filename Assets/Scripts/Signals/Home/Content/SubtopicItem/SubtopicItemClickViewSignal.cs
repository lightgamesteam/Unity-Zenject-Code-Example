
namespace TDL.Signals
{
    public class SubtopicItemClickViewSignal : ISignal
    {
        public int ParentId { get; private set; }
        public int Id { get; private set; }

        public SubtopicItemClickViewSignal(int parentId, int id)
        {
            ParentId = parentId;
            Id = id;
        }
    }
}