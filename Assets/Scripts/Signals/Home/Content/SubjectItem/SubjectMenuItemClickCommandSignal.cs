
namespace TDL.Signals
{
    public class SubjectMenuItemClickCommandSignal : ISignal
    {
        public int ParentId { get; private set; }
        public int Id { get; private set; }
        public bool IsFromBreadcrumbs { get; private set; }

        public SubjectMenuItemClickCommandSignal(int parentId, int id, bool isFromBreadcrumbs = false)
        {
            ParentId = parentId;
            Id = id;
            IsFromBreadcrumbs = isFromBreadcrumbs;
        }
    }
}