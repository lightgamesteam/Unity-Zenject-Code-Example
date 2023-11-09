namespace TDL.Signals
{
    public class MainScreenAssetItemClickCommandSignal : ISignal
    {
        public int GradeId { get; private set; }
        public int Id { get; private set; }

        public MainScreenAssetItemClickCommandSignal(int id, int gradeId)
        {
            Id = id;
            GradeId = gradeId;
        }
    }
}