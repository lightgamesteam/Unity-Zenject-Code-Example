
namespace TDL.Signals
{
    public class CreateTeacherContentModelCommandSignal : ISignal
    {
        public string ContentResponse { get; private set; }

        public CreateTeacherContentModelCommandSignal(string contentResponse)
        {
            ContentResponse = contentResponse;
        }
    }
}