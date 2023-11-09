
namespace TDL.Signals
{
    public class SubjectMenuItemClickViewSignal : ISignal
    {
        public int ParentId { get; private set; }
        public int Id { get; private set; }
    
        public SubjectMenuItemClickViewSignal(int parentId, int id)
        {
            ParentId = parentId;
            Id = id;
        }
    }
}