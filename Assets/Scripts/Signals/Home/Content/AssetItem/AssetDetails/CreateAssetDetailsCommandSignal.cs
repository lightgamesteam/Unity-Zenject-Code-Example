
namespace TDL.Signals
{
    public class CreateAssetDetailsCommandSignal : ISignal
    {
        public int Id { get; private set; }
        public int GradeId { get; private set; }
        public string Response { get; private set; }

        public CreateAssetDetailsCommandSignal(int id, int gradeId, string response)
        {
            Id = id;
            GradeId = gradeId;
            Response = response;
        }
    }
}