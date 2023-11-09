
namespace TDL.Signals
{
    public class GradeMenuItemClickViewSignal : ISignal
    {
        public int ParentId { get; private set; }
        public int Id { get; private set; }
    
        public GradeMenuItemClickViewSignal(int parentId, int id)
        {
            ParentId = parentId;
            Id = id;
        }
    }
}