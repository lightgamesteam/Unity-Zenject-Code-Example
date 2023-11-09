
namespace TDL.Signals
{
    public class GradeMenuItemClickCommandSignal : ISignal
    {
        public int ParentId { get; private set; }
        public int Id { get; private set; }
        public bool IsFromBreadcrumbs { get; private set; }

        public GradeMenuItemClickCommandSignal(int parentId, int id, bool isFromBreadcrumbs = false)
        {
            ParentId = parentId;
            Id = id;
            IsFromBreadcrumbs = isFromBreadcrumbs;
        }
    }
}