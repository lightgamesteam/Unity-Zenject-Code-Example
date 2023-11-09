
namespace TDL.Signals
{
	public class StartPuzzleCommandSignal : ISignal
	{
		public int Id { get; private set; }
		public int SelectedPuzzleItemId { get; private set; }

		public StartPuzzleCommandSignal(int id, int selectedPuzzleItemId)
		{
			Id = id;
			SelectedPuzzleItemId = selectedPuzzleItemId;
		}
	}
}