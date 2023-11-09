
namespace TDL.Signals
{
    public class DownloadAssetDetailsCommandSignal : ISignal
    {
        public int Id { get; private set; }
        public  int GradeId { get; private set; }

        public DownloadAssetDetailsCommandSignal(int id,  int gradeId)
        {
            Id = id;
            GradeId = gradeId;
        }
    }
}