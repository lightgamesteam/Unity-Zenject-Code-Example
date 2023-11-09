namespace TDL.Signals
{
    public class AssetItemClickCommandSignal : ISignal
    {
        public int GradeId { get; private set; }
        public int Id { get; private set; }

        public AssetItemClickCommandSignal(int id, int gradeId)
        {
            Id = id;
            GradeId = gradeId;
        }
    }
}