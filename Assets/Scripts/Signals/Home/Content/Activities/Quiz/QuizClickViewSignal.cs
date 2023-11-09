
namespace TDL.Signals
{
	public class QuizClickViewSignal : ISignal
	{
		public int AssetId { get; private set; }
		public int GradeId { get; private set; }
		public int SelectedItemId { get; private set; }

		public QuizClickViewSignal(int assetId, int gradeId, int selectedItemId = 0)
		{
			AssetId = assetId;
			GradeId = gradeId;
			SelectedItemId = selectedItemId;
		}
	}
}