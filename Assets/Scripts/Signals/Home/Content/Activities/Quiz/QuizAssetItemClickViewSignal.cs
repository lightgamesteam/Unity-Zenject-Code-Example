
namespace TDL.Signals
{
    public class QuizAssetItemClickViewSignal : ISignal
    {
        public int Id { get; private set; }
        public int SelectedItemId { get; private set; }

        public QuizAssetItemClickViewSignal(int id, int selectedItemId)
        {
            Id = id;
            SelectedItemId = selectedItemId;
        }
    }
}