
namespace TDL.Signals
{
    public class StartQuizCommandSignal : ISignal
    {
        public int Id { get; private set; }
        public int SelectedQuizItemId { get; private set; }

        public StartQuizCommandSignal(int id, int selectedQuizItemId)
        {
            Id = id;
            SelectedQuizItemId = selectedQuizItemId;
        }
    }
}